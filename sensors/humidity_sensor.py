#!/usr/bin/env python3
"""
Humidity Sensor Reader for Pi Plant Monitoring System
This script reads humidity data from a physical sensor and returns it as JSON.
"""

import json
import sys
import time
import random  # For testing purposes - replace with actual sensor library

def read_humidity_sensor():
    """
    Read humidity from the physical sensor.
    Returns humidity as a percentage (0-100%).
    """
    try:
        # TODO: Replace this with actual sensor reading code
        # Example for testing - replace with your actual sensor implementation
        # import board
        # import adafruit_dht  # or your specific humidity sensor library
        
        # dht = adafruit_dht.DHT22(board.D4)  # or DHT11, depending on your sensor
        # humidity = dht.humidity
        
        # For now, simulate sensor reading
        humidity = random.uniform(30, 70)  # Random value between 30-70%
        
        return {
            "success": True,
            "humidity": round(humidity, 2),
            "unit": "percentage",
            "timestamp": time.time(),
            "error": None
        }
        
    except Exception as e:
        return {
            "success": False,
            "humidity": None,
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
            reading = read_humidity_sensor()
            readings.append(reading)
            time.sleep(1)
        print(json.dumps(readings, indent=2))
    else:
        # Normal mode - single reading
        reading = read_humidity_sensor()
        print(json.dumps(reading, indent=2))

if __name__ == "__main__":
    main() 