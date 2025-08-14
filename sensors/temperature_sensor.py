#!/usr/bin/env python3
"""
Temperature Sensor Reader for Pi Plant Monitoring System
This script reads temperature data from a physical sensor and returns it as JSON.
"""

import json
import sys
import time
import random  # For testing purposes - replace with actual sensor library

def read_temperature_sensor():
    """
    Read temperature from the physical sensor.
    Returns temperature in Celsius.
    """
    try:
        # TODO: Replace this with actual sensor reading code
        # Example for testing - replace with your actual sensor implementation
        # import board
        # import adafruit_dht  # or your specific temperature sensor library
        
        # dht = adafruit_dht.DHT22(board.D4)  # or DHT11, depending on your sensor
        # temperature = dht.temperature
        
        # For now, simulate sensor reading
        temperature = random.uniform(18, 28)  # Random value between 18-28°C
        
        return {
            "success": True,
            "temperature": round(temperature, 2),
            "unit": "celsius",
            "timestamp": time.time(),
            "error": None
        }
        
    except Exception as e:
        return {
            "success": False,
            "temperature": None,
            "unit": "celsius",
            "timestamp": time.time(),
            "error": str(e)
        }

def main():
    """Main function to read sensor and output JSON."""
    if len(sys.argv) > 1 and sys.argv[1] == "--test":
        # Test mode - read multiple times
        readings = []
        for i in range(5):
            reading = read_temperature_sensor()
            readings.append(reading)
            time.sleep(1)
        print(json.dumps(readings, indent=2))
    else:
        # Normal mode - single reading
        reading = read_temperature_sensor()
        print(json.dumps(reading, indent=2))

if __name__ == "__main__":
    main() 