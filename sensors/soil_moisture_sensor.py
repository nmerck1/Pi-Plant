#!/usr/bin/env python3
"""
Soil Moisture Sensor Reader for Pi Plant Monitoring System
This script reads soil moisture data from a physical sensor and returns it as JSON.
"""

import json
import sys
import time
import random  # For testing purposes - replace with actual sensor library

def read_soil_moisture_sensor():
    """
    Read soil moisture from the physical sensor.
    Returns moisture level as a percentage (0-100%).
    """
    try:
        # TODO: Replace this with actual sensor reading code
        # Example for testing - replace with your actual sensor implementation
        # import board
        # import analogio  # or your specific soil moisture sensor library
        
        # # For capacitive soil moisture sensor
        # analog_in = analogio.AnalogIn(board.A1)
        # raw_value = analog_in.value
        # # Convert raw ADC value to percentage (calibration needed)
        # moisture_percentage = ((65535 - raw_value) / 65535) * 100
        
        # For now, simulate sensor reading
        moisture_percentage = random.uniform(20, 80)  # Random value between 20-80%
        
        return {
            "success": True,
            "soil_moisture": round(moisture_percentage, 2),
            "unit": "percentage",
            "timestamp": time.time(),
            "error": None
        }
        
    except Exception as e:
        return {
            "success": False,
            "soil_moisture": None,
            "unit": "percentage",
            "timestamp": time.time(),
            "error": str(e)
        }

def main():
    """Main function to read sensor and output JSON."""
    if len(sys.argv) > 1 and sys.argv[1] == "--test":
        # Test mode - read multiple times
        readings = []
        for i in range(5):
            reading = read_soil_moisture_sensor()
            readings.append(reading)
            time.sleep(1)
        print(json.dumps(readings, indent=2))
    else:
        # Normal mode - single reading
        reading = read_soil_moisture_sensor()
        print(json.dumps(reading, indent=2))

if __name__ == "__main__":
    main() 