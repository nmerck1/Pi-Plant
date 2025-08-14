package services

import (
	"encoding/json"
	"fmt"
	"log"
	"os/exec"
	"path/filepath"
	"runtime"
	"strconv"
	"time"
)

// SensorReading represents the JSON output from Python sensor scripts
type SensorReading struct {
	Success   bool    `json:"success"`
	Error     *string `json:"error"`
	Timestamp float64 `json:"timestamp"`
	Unit      string  `json:"unit"`
	
	// Sensor-specific fields
	LightLevel     *float64 `json:"light_level,omitempty"`
	Temperature    *float64 `json:"temperature,omitempty"`
	SoilMoisture   *float64 `json:"soil_moisture,omitempty"`
	Humidity       *float64 `json:"humidity,omitempty"`
}

// SensorService handles communication with Python sensor scripts
type SensorService struct {
	sensorsPath string
}

// NewSensorService creates a new sensor service instance
func NewSensorService() *SensorService {
	// Get the path to the sensors directory relative to the backend
	_, filename, _, _ := runtime.Caller(0)
	backendDir := filepath.Dir(filepath.Dir(filename)) // Go up from services/ to back-end/
	sensorsPath := filepath.Join(filepath.Dir(backendDir), "sensors")
	
	return &SensorService{
		sensorsPath: sensorsPath,
	}
}

// ReadLightSensor reads light level from the physical sensor
func (s *SensorService) ReadLightSensor() (*SensorReading, error) {
	return s.runSensorScript("light_sensor.py")
}

// ReadTemperatureSensor reads temperature from the physical sensor
func (s *SensorService) ReadTemperatureSensor() (*SensorReading, error) {
	return s.runSensorScript("temperature_sensor.py")
}

// ReadSoilMoistureSensor reads soil moisture from the physical sensor
func (s *SensorService) ReadSoilMoistureSensor() (*SensorReading, error) {
	return s.runSensorScript("soil_moisture_sensor.py")
}

// ReadHumiditySensor reads humidity from the physical sensor
func (s *SensorService) ReadHumiditySensor() (*SensorReading, error) {
	return s.runSensorScript("humidity_sensor.py")
}

// ReadAllSensors reads data from all available sensors
func (s *SensorService) ReadAllSensors() (map[string]*SensorReading, error) {
	readings := make(map[string]*SensorReading)
	
	// Read each sensor type
	sensors := map[string]func() (*SensorReading, error){
		"light":        s.ReadLightSensor,
		"temperature":  s.ReadTemperatureSensor,
		"soil_moisture": s.ReadSoilMoistureSensor,
		"humidity":     s.ReadHumiditySensor,
	}
	
	for sensorType, readFunc := range sensors {
		reading, err := readFunc()
		if err != nil {
			log.Printf("Error reading %s sensor: %v", sensorType, err)
			readings[sensorType] = &SensorReading{
				Success:   false,
				Error:     &err.Error(),
				Timestamp: float64(time.Now().Unix()),
			}
		} else {
			readings[sensorType] = reading
		}
	}
	
	return readings, nil
}

// runSensorScript executes a Python sensor script and parses the JSON output
func (s *SensorService) runSensorScript(scriptName string) (*SensorReading, error) {
	scriptPath := filepath.Join(s.sensorsPath, scriptName)
	
	// Determine the Python command based on the OS
	var cmd *exec.Cmd
	if runtime.GOOS == "windows" {
		cmd = exec.Command("python", scriptPath)
	} else {
		cmd = exec.Command("python3", scriptPath)
	}
	
	// Execute the script
	output, err := cmd.Output()
	if err != nil {
		return nil, fmt.Errorf("failed to execute sensor script %s: %v", scriptName, err)
	}
	
	// Parse the JSON output
	var reading SensorReading
	if err := json.Unmarshal(output, &reading); err != nil {
		return nil, fmt.Errorf("failed to parse sensor output from %s: %v", scriptName, err)
	}
	
	// Check if the sensor reading was successful
	if !reading.Success {
		errorMsg := "unknown error"
		if reading.Error != nil {
			errorMsg = *reading.Error
		}
		return nil, fmt.Errorf("sensor reading failed: %s", errorMsg)
	}
	
	return &reading, nil
}

// TestSensorScript tests a sensor script with the --test flag
func (s *SensorService) TestSensorScript(scriptName string) ([]*SensorReading, error) {
	scriptPath := filepath.Join(s.sensorsPath, scriptName)
	
	// Determine the Python command based on the OS
	var cmd *exec.Cmd
	if runtime.GOOS == "windows" {
		cmd = exec.Command("python", scriptPath, "--test")
	} else {
		cmd = exec.Command("python3", scriptPath, "--test")
	}
	
	// Execute the script
	output, err := cmd.Output()
	if err != nil {
		return nil, fmt.Errorf("failed to execute sensor script %s: %v", scriptName, err)
	}
	
	// Parse the JSON array output
	var readings []*SensorReading
	if err := json.Unmarshal(output, &readings); err != nil {
		return nil, fmt.Errorf("failed to parse sensor test output from %s: %v", scriptName, err)
	}
	
	return readings, nil
}

// GetSensorStatus checks if all sensor scripts are available and executable
func (s *SensorService) GetSensorStatus() map[string]bool {
	status := make(map[string]bool)
	
	sensors := []string{
		"light_sensor.py",
		"temperature_sensor.py",
		"soil_moisture_sensor.py",
		"humidity_sensor.py",
	}
	
	for _, sensor := range sensors {
		scriptPath := filepath.Join(s.sensorsPath, sensor)
		
		// Check if file exists
		if _, err := exec.LookPath(scriptPath); err == nil {
			status[sensor] = true
		} else {
			status[sensor] = false
		}
	}
	
	return status
}

// ConvertToDatabaseFormat converts a sensor reading to the database format
func (s *SensorService) ConvertToDatabaseFormat(plantID int, readings map[string]*SensorReading) map[string]interface{} {
	result := map[string]interface{}{
		"plant_id":     plantID,
		"recorded_at":  time.Now(),
		"temperature":  nil,
		"humidity":     nil,
		"soil_moisture": nil,
		"light_level":  nil,
	}
	
	// Extract values from sensor readings
	if tempReading, exists := readings["temperature"]; exists && tempReading.Success && tempReading.Temperature != nil {
		result["temperature"] = *tempReading.Temperature
	}
	
	if humidityReading, exists := readings["humidity"]; exists && humidityReading.Success && humidityReading.Humidity != nil {
		result["humidity"] = *humidityReading.Humidity
	}
	
	if moistureReading, exists := readings["soil_moisture"]; exists && moistureReading.Success && moistureReading.SoilMoisture != nil {
		result["soil_moisture"] = *moistureReading.SoilMoisture
	}
	
	if lightReading, exists := readings["light"]; exists && lightReading.Success && lightReading.LightLevel != nil {
		result["light_level"] = *lightReading.LightLevel
	}
	
	return result
} 