package main

import (
    "encoding/json"
    "log"
    "net/http"
    "strconv"
    "time"
    
    "github.com/gorilla/mux"
    "pi-plant-backend/database"
    "pi-plant-backend/services"
)

type Server struct {
    db *database.DB
    sensorService *services.SensorService
}

func main() {
    // Initialize database
    db, err := database.NewDB("./pi_plant.db")
    if err != nil {
        log.Fatal("Failed to connect to database:", err)
    }
    defer db.Close()
    
    // Initialize schema
    if err := db.InitializeSchema(); err != nil {
        log.Fatal("Failed to initialize database schema:", err)
    }
    
    sensorService := services.NewSensorService()
    server := &Server{db: db, sensorService: sensorService}
    
    // Setup routes
    r := mux.NewRouter()
    
    // Plant routes
    r.HandleFunc("/api/plants", server.handleGetPlants).Methods("GET")
    r.HandleFunc("/api/plants", server.handleAddPlant).Methods("POST")
    
    // Sensor data routes
    r.HandleFunc("/api/plants/{id}/sensors", server.handleAddSensorReading).Methods("POST")
    r.HandleFunc("/api/plants/{id}/sensors", server.handleGetSensorReadings).Methods("GET")
    r.HandleFunc("/api/plants/{id}/health", server.handleGetPlantHealth).Methods("GET")
    
    // New sensor reading routes
    r.HandleFunc("/api/plants/{id}/read-sensors", server.handleReadSensors).Methods("POST")
    r.HandleFunc("/api/sensors/status", server.handleGetSensorStatus).Methods("GET")
    r.HandleFunc("/api/sensors/test/{sensor}", server.handleTestSensor).Methods("GET")
    
    // Watering routes
    r.HandleFunc("/api/plants/{id}/water", server.handleLogWatering).Methods("POST")
    
    // Serve static files (your React build)
    r.PathPrefix("/").Handler(http.FileServer(http.Dir("../front-end/build/")))
    
    log.Println("Server starting on :8080...")
    log.Fatal(http.ListenAndServe(":8080", r))
}

// Plant handlers
func (s *Server) handleGetPlants(w http.ResponseWriter, r *http.Request) {
    plants, err := s.db.GetPlants()
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(plants)
}

func (s *Server) handleAddPlant(w http.ResponseWriter, r *http.Request) {
    var req struct {
        Name     string `json:"name"`
        Species  string `json:"species"`
        Location string `json:"location"`
    }
    
    if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
        http.Error(w, "Invalid JSON", http.StatusBadRequest)
        return
    }
    
    plant, err := s.db.AddPlant(req.Name, req.Species, req.Location)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    w.Header().Set("Content-Type", "application/json")
    w.WriteHeader(http.StatusCreated)
    json.NewEncoder(w).Encode(plant)
}

// Sensor data handlers
func (s *Server) handleAddSensorReading(w http.ResponseWriter, r *http.Request) {
    vars := mux.Vars(r)
    plantID, err := strconv.Atoi(vars["id"])
    if err != nil {
        http.Error(w, "Invalid plant ID", http.StatusBadRequest)
        return
    }
    
    var reading database.SensorReading
    if err := json.NewDecoder(r.Body).Decode(&reading); err != nil {
        http.Error(w, "Invalid JSON", http.StatusBadRequest)
        return
    }
    
    reading.PlantID = plantID
    reading.RecordedAt = time.Now()
    
    if err := s.db.AddSensorReading(reading); err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    w.WriteHeader(http.StatusCreated)
    json.NewEncoder(w).Encode(map[string]string{"status": "success"})
}

func (s *Server) handleGetSensorReadings(w http.ResponseWriter, r *http.Request) {
    vars := mux.Vars(r)
    plantID, err := strconv.Atoi(vars["id"])
    if err != nil {
        http.Error(w, "Invalid plant ID", http.StatusBadRequest)
        return
    }
    
    // Parse query parameters
    hoursParam := r.URL.Query().Get("hours")
    hours := 24 // default to 24 hours
    if hoursParam != "" {
        if h, err := strconv.Atoi(hoursParam); err == nil {
            hours = h
        }
    }
    
    limitParam := r.URL.Query().Get("limit")
    limit := 100 // default limit
    if limitParam != "" {
        if l, err := strconv.Atoi(limitParam); err == nil {
            limit = l
        }
    }
    
    since := time.Now().Add(-time.Duration(hours) * time.Hour)
    readings, err := s.db.GetSensorReadings(plantID, since, limit)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(readings)
}

func (s *Server) handleGetPlantHealth(w http.ResponseWriter, r *http.Request) {
    vars := mux.Vars(r)
    plantID, err := strconv.Atoi(vars["id"])
    if err != nil {
        http.Error(w, "Invalid plant ID", http.StatusBadRequest)
        return
    }
    
    health, err := s.db.GetLatestHealthStatus(plantID)
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    if health == nil {
        // No health data yet, return a default response
        defaultHealth := map[string]interface{}{
            "plant_id":        plantID,
            "overall_health":  "unknown",
            "health_score":    0,
            "alerts":          []string{},
            "recommendations": []string{"Add sensor data to calculate health"},
        }
        w.Header().Set("Content-Type", "application/json")
        json.NewEncoder(w).Encode(defaultHealth)
        return
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(health)
}

func (s *Server) handleLogWatering(w http.ResponseWriter, r *http.Request) {
    vars := mux.Vars(r)
    plantID, err := strconv.Atoi(vars["id"])
    if err != nil {
        http.Error(w, "Invalid plant ID", http.StatusBadRequest)
        return
    }
    
    var wateringLog database.WateringLog
    if err := json.NewDecoder(r.Body).Decode(&wateringLog); err != nil {
        http.Error(w, "Invalid JSON", http.StatusBadRequest)
        return
    }
    
    wateringLog.PlantID = plantID
    wateringLog.WateredAt = time.Now()
    
    if err := s.db.LogWatering(wateringLog); err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }
    
    w.WriteHeader(http.StatusCreated)
    json.NewEncoder(w).Encode(map[string]string{"status": "watering logged"})
}

// New sensor reading handlers
func (s *Server) handleReadSensors(w http.ResponseWriter, r *http.Request) {
    vars := mux.Vars(r)
    plantID, err := strconv.Atoi(vars["id"])
    if err != nil {
        http.Error(w, "Invalid plant ID", http.StatusBadRequest)
        return
    }
    
    // Read all sensors
    readings, err := s.sensorService.ReadAllSensors()
    if err != nil {
        http.Error(w, "Failed to read sensors: "+err.Error(), http.StatusInternalServerError)
        return
    }
    
    // Convert to database format
    dbData := s.sensorService.ConvertToDatabaseFormat(plantID, readings)
    
    // Create sensor reading for database
    var reading database.SensorReading
    if temp, ok := dbData["temperature"].(float64); ok {
        reading.Temperature = &temp
    }
    if humidity, ok := dbData["humidity"].(float64); ok {
        reading.Humidity = &humidity
    }
    if moisture, ok := dbData["soil_moisture"].(float64); ok {
        reading.SoilMoisture = &moisture
    }
    if light, ok := dbData["light_level"].(float64); ok {
        reading.LightLevel = &light
    }
    
    reading.PlantID = plantID
    reading.RecordedAt = time.Now()
    
    // Save to database
    if err := s.db.AddSensorReading(reading); err != nil {
        http.Error(w, "Failed to save sensor reading: "+err.Error(), http.StatusInternalServerError)
        return
    }
    
    // Return the readings
    response := map[string]interface{}{
        "plant_id": plantID,
        "readings": readings,
        "saved_to_db": true,
        "timestamp": time.Now(),
    }
    
    w.Header().Set("Content-Type", "application/json")
    w.WriteHeader(http.StatusCreated)
    json.NewEncoder(w).Encode(response)
}

func (s *Server) handleGetSensorStatus(w http.ResponseWriter, r *http.Request) {
    status := s.sensorService.GetSensorStatus()
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(map[string]interface{}{
        "sensor_status": status,
        "timestamp": time.Now(),
    })
}

func (s *Server) handleTestSensor(w http.ResponseWriter, r *http.Request) {
    vars := mux.Vars(r)
    sensorName := vars["sensor"]
    
    // Map sensor names to script names
    sensorScripts := map[string]string{
        "light": "light_sensor.py",
        "temperature": "temperature_sensor.py",
        "soil-moisture": "soil_moisture_sensor.py",
        "humidity": "humidity_sensor.py",
    }
    
    scriptName, exists := sensorScripts[sensorName]
    if !exists {
        http.Error(w, "Invalid sensor type", http.StatusBadRequest)
        return
    }
    
    // Test the sensor
    readings, err := s.sensorService.TestSensorScript(scriptName)
    if err != nil {
        http.Error(w, "Failed to test sensor: "+err.Error(), http.StatusInternalServerError)
        return
    }
    
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(map[string]interface{}{
        "sensor": sensorName,
        "test_readings": readings,
        "timestamp": time.Now(),
    })
}