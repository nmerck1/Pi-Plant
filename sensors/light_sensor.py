#!/usr/bin/env python3
"""
Light Sensor Reader for Pi Plant Monitoring System
This script reads light level data from a physical sensor and returns it as JSON.
"""

import json
import sys
import time
import random  # For testing purposes - replace with actual sensor library

def read_light_sensor():
    """
    Read light level from the physical sensor.
    Returns light level in lux (lumens per square meter).
    """
    try:
        # TODO: Replace this with actual sensor reading code
        # Example for testing - replace with your actual sensor implementation
        # import board
        # import adafruit_tsl2591  # or your specific light sensor library
        
        # i2c = board.I2C()
        # sensor = adafruit_tsl2591.TSL2591(i2c)
        # light_level = sensor.lux
        
        # For now, simulate sensor reading
        light_level = random.uniform(100, 1000)  # Random value between 100-1000 lux
        
        return {
            "success": True,
            "light_level": round(light_level, 2),
            "unit": "lux",
            "timestamp": time.time(),
            "error": None
        }
        
    except Exception as e:
        return {
            "success": False,
            "light_level": None,
            "unit": "lux",
            "timestamp": time.time(),
            "error": str(e)
        }

def main():
    """Main function to read sensor and output JSON."""
    if len(sys.argv) > 1 and sys.argv[1] == "--test":
        # Test mode - read multiple times
        readings = []
        for i in range(5):
            reading = read_light_sensor()
            readings.append(reading)
            time.sleep(1)
        print(json.dumps(readings, indent=2))
    else:
        # Normal mode - single reading
        reading = read_light_sensor()
        print(json.dumps(reading, indent=2))

if __name__ == "__main__":
    main() 