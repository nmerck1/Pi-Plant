package database

import (
    "database/sql"
    "encoding/json"
    "fmt"
    "log"
    "time"
    
    _ "github.com/mattn/go-sqlite3"
)

// Database struct holds the database connection
type DB struct {
    conn *sql.DB
}

// Plant represents a plant in the system
type Plant struct {
    ID        int       `json:"id"`
    Name      string    `json:"name"`
    Species   string    `json:"species"`
    Location  string    `json:"location"`
    CreatedAt time.Time `json:"created_at"`
    UpdatedAt time.Time `json:"updated_at"`
}

// SensorReading represents a sensor data point
type SensorReading struct {
    ID           int       `json:"id"`
    PlantID      int       `json:"plant_id"`
    Temperature  *float64  `json:"temperature"`
    Humidity     *float64  `json:"humidity"`
    SoilMoisture *float64  `json:"soil_moisture"`
    LightLevel   *float64  `json:"light_level"`
    PHLevel      *float64  `json:"ph_level"`
    RecordedAt   time.Time `json:"recorded_at"`
}

// HealthStatus represents the calculated health of a plant
type HealthStatus struct {
    ID              int       `json:"id"`
    PlantID         int       `json:"plant_id"`
    OverallHealth   string    `json:"overall_health"`
    HealthScore     int       `json:"health_score"`
    Alerts          []string  `json:"alerts"`
    Recommendations []string  `json:"recommendations"`
    CalculatedAt    time.Time `json:"calculated_at"`
}

// SensorThresholds represents optimal sensor ranges for a plant
type SensorThresholds struct {
    ID               int     `json:"id"`
    PlantID          int     `json:"plant_id"`
    TemperatureMin   *float64 `json:"temperature_min"`
    TemperatureMax   *float64 `json:"temperature_max"`
    HumidityMin      *float64 `json:"humidity_min"`
    HumidityMax      *float64 `json:"humidity_max"`
    SoilMoistureMin  *float64 `json:"soil_moisture_min"`
    SoilMoistureMax  *float64 `json:"soil_moisture_max"`
    LightLevelMin    *float64 `json:"light_level_min"`
    LightLevelMax    *float64 `json:"light_level_max"`
    PHMin            *float64 `json:"ph_min"`
    PHMax            *float64 `json:"ph_max"`
    CreatedAt        time.Time `json:"created_at"`
    UpdatedAt        time.Time `json:"updated_at"`
}

// WateringLog represents a watering event
type WateringLog struct {
    ID        int       `json:"id"`
    PlantID   int       `json:"plant_id"`
    AmountML  *float64  `json:"amount_ml"`
    Method    string    `json:"method"`
    Notes     string    `json:"notes"`
    WateredAt time.Time `json:"watered_at"`
}

// NewDB creates a new database connection
func NewDB(dbPath string) (*DB, error) {
    conn, err := sql.Open("sqlite3", dbPath)
    if err != nil {
        return nil, fmt.Errorf("failed to open database: %v", err)
    }
    
    // Enable foreign key constraints
    if _, err := conn.Exec("PRAGMA foreign_keys = ON"); err != nil {
        return nil, fmt.Errorf("failed to enable foreign keys: %v", err)
    }
    
    db := &DB{conn: conn}
    return db, nil
}

// Close closes the database connection
func (db *DB) Close() error {
    return db.conn.Close()
}

// InitializeSchema creates the database tables
func (db *DB) InitializeSchema() error {
    schema := `
    CREATE TABLE IF NOT EXISTS plants (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT NOT NULL,
        species TEXT,
        location TEXT,
        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
        updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
    );

    CREATE TABLE IF NOT EXISTS sensor_readings (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        plant_id INTEGER NOT NULL,
        temperature REAL,
        humidity REAL,
        soil_moisture REAL,
        light_level REAL,
        ph_level REAL,
        recorded_at DATETIME DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (plant_id) REFERENCES plants(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS health_status (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        plant_id INTEGER NOT NULL,
        overall_health TEXT CHECK(overall_health IN ('excellent', 'good', 'fair', 'poor', 'critical')),
        health_score INTEGER CHECK(health_score >= 0 AND health_score <= 100),
        alerts TEXT,
        recommendations TEXT,
        calculated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (plant_id) REFERENCES plants(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS sensor_thresholds (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        plant_id INTEGER NOT NULL,
        temperature_min REAL,
        temperature_max REAL,
        humidity_min REAL,
        humidity_max REAL,
        soil_moisture_min REAL,
        soil_moisture_max REAL,
        light_level_min REAL,
        light_level_max REAL,
        ph_min REAL,
        ph_max REAL,
        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
        updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (plant_id) REFERENCES plants(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS watering_log (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        plant_id INTEGER NOT NULL,
        amount_ml REAL,
        method TEXT,
        notes TEXT,
        watered_at DATETIME DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (plant_id) REFERENCES plants(id) ON DELETE CASCADE
    );

    CREATE INDEX IF NOT EXISTS idx_sensor_readings_plant_id ON sensor_readings(plant_id);
    CREATE INDEX IF NOT EXISTS idx_sensor_readings_recorded_at ON sensor_readings(recorded_at);
    CREATE INDEX IF NOT EXISTS idx_health_status_plant_id ON health_status(plant_id);
    CREATE INDEX IF NOT EXISTS idx_health_status_calculated_at ON health_status(calculated_at);
    CREATE INDEX IF NOT EXISTS idx_watering_log_plant_id ON watering_log(plant_id);
    `
    
    _, err := db.conn.Exec(schema)
    if err != nil {
        return fmt.Errorf("failed to initialize schema: %v", err)
    }
    
    log.Println("Database schema initialized successfully")
    return nil
}

// AddPlant adds a new plant to the database
func (db *DB) AddPlant(name, species, location string) (*Plant, error) {
    query := `INSERT INTO plants (name, species, location) VALUES (?, ?, ?) RETURNING id, created_at, updated_at`
    
    var plant Plant
    plant.Name = name
    plant.Species = species
    plant.Location = location
    
    err := db.conn.QueryRow(query, name, species, location).Scan(&plant.ID, &plant.CreatedAt, &plant.UpdatedAt)
    if err != nil {
        return nil, fmt.Errorf("failed to add plant: %v", err)
    }
    
    return &plant, nil
}

// GetPlants retrieves all plants
func (db *DB) GetPlants() ([]Plant, error) {
    query := `SELECT id, name, species, location, created_at, updated_at FROM plants ORDER BY created_at DESC`
    
    rows, err := db.conn.Query(query)
    if err != nil {
        return nil, fmt.Errorf("failed to get plants: %v", err)
    }
    defer rows.Close()
    
    var plants []Plant
    for rows.Next() {
        var plant Plant
        err := rows.Scan(&plant.ID, &plant.Name, &plant.Species, &plant.Location, &plant.CreatedAt, &plant.UpdatedAt)
        if err != nil {
            return nil, fmt.Errorf("failed to scan plant: %v", err)
        }
        plants = append(plants, plant)
    }
    
    return plants, nil
}

// AddSensorReading adds a new sensor reading
func (db *DB) AddSensorReading(reading SensorReading) error {
    query := `INSERT INTO sensor_readings (plant_id, temperature, humidity, soil_moisture, light_level, ph_level, recorded_at) 
              VALUES (?, ?, ?, ?, ?, ?, ?)`
    
    recordedAt := reading.RecordedAt
    if recordedAt.IsZero() {
        recordedAt = time.Now()
    }
    
    _, err := db.conn.Exec(query, reading.PlantID, reading.Temperature, reading.Humidity, 
                          reading.SoilMoisture, reading.LightLevel, reading.PHLevel, recordedAt)
    if err != nil {
        return fmt.Errorf("failed to add sensor reading: %v", err)
    }
    
    return nil
}

// GetSensorReadings retrieves sensor readings for a plant within a time range
func (db *DB) GetSensorReadings(plantID int, since time.Time, limit int) ([]SensorReading, error) {
    query := `SELECT id, plant_id, temperature, humidity, soil_moisture, light_level, ph_level, recorded_at 
              FROM sensor_readings 
              WHERE plant_id = ? AND recorded_at >= ? 
              ORDER BY recorded_at DESC LIMIT ?`
    
    rows, err := db.conn.Query(query, plantID, since, limit)
    if err != nil {
        return nil, fmt.Errorf("failed to get sensor readings: %v", err)
    }
    defer rows.Close()
    
    var readings []SensorReading
    for rows.Next() {
        var reading SensorReading
        err := rows.Scan(&reading.ID, &reading.PlantID, &reading.Temperature, &reading.Humidity,
                        &reading.SoilMoisture, &reading.LightLevel, &reading.PHLevel, &reading.RecordedAt)
        if err != nil {
            return nil, fmt.Errorf("failed to scan sensor reading: %v", err)
        }
        readings = append(readings, reading)
    }
    
    return readings, nil
}

// AddHealthStatus adds a health status record
func (db *DB) AddHealthStatus(status HealthStatus) error {
    alertsJSON, _ := json.Marshal(status.Alerts)
    recommendationsJSON, _ := json.Marshal(status.Recommendations)
    
    query := `INSERT INTO health_status (plant_id, overall_health, health_score, alerts, recommendations, calculated_at) 
              VALUES (?, ?, ?, ?, ?, ?)`
    
    calculatedAt := status.CalculatedAt
    if calculatedAt.IsZero() {
        calculatedAt = time.Now()
    }
    
    _, err := db.conn.Exec(query, status.PlantID, status.OverallHealth, status.HealthScore, 
                          string(alertsJSON), string(recommendationsJSON), calculatedAt)
    if err != nil {
        return fmt.Errorf("failed to add health status: %v", err)
    }
    
    return nil
}

// GetLatestHealthStatus gets the most recent health status for a plant
func (db *DB) GetLatestHealthStatus(plantID int) (*HealthStatus, error) {
    query := `SELECT id, plant_id, overall_health, health_score, alerts, recommendations, calculated_at 
              FROM health_status 
              WHERE plant_id = ? 
              ORDER BY calculated_at DESC LIMIT 1`
    
    var status HealthStatus
    var alertsJSON, recommendationsJSON string
    
    err := db.conn.QueryRow(query, plantID).Scan(&status.ID, &status.PlantID, &status.OverallHealth,
                                                 &status.HealthScore, &alertsJSON, &recommendationsJSON, 
                                                 &status.CalculatedAt)
    if err != nil {
        if err == sql.ErrNoRows {
            return nil, nil
        }
        return nil, fmt.Errorf("failed to get health status: %v", err)
    }
    
    json.Unmarshal([]byte(alertsJSON), &status.Alerts)
    json.Unmarshal([]byte(recommendationsJSON), &status.Recommendations)
    
    return &status, nil
}

// LogWatering records a watering event
func (db *DB) LogWatering(log WateringLog) error {
    query := `INSERT INTO watering_log (plant_id, amount_ml, method, notes, watered_at) VALUES (?, ?, ?, ?, ?)`
    
    wateredAt := log.WateredAt
    if wateredAt.IsZero() {
        wateredAt = time.Now()
    }
    
    _, err := db.conn.Exec(query, log.PlantID, log.AmountML, log.Method, log.Notes, wateredAt)
    if err != nil {
        return fmt.Errorf("failed to log watering: %v", err)
    }
    
    return nil
}