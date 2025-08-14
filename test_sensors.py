#!/usr/bin/env python3
"""
Test script for Pi Plant sensor scripts
This script tests all sensor scripts to ensure they work correctly.
"""

import json
import subprocess
import sys
import os

def test_sensor_script(script_name):
    """Test a single sensor script."""
    print(f"Testing {script_name}...")
    
    try:
        # Run the script
        result = subprocess.run(
            [sys.executable, f"sensors/{script_name}"],
            capture_output=True,
            text=True,
            timeout=10
        )
        
        if result.returncode != 0:
            print(f"  ❌ Error running {script_name}: {result.stderr}")
            return False
        
        # Parse JSON output
        try:
            data = json.loads(result.stdout.strip())
            print(f"  ✅ {script_name} output: {json.dumps(data, indent=2)}")
            return True
        except json.JSONDecodeError as e:
            print(f"  ❌ Invalid JSON from {script_name}: {e}")
            print(f"  Raw output: {result.stdout}")
            return False
            
    except subprocess.TimeoutExpired:
        print(f"  ❌ {script_name} timed out")
        return False
    except Exception as e:
        print(f"  ❌ Unexpected error testing {script_name}: {e}")
        return False

def test_sensor_script_with_test_flag(script_name):
    """Test a sensor script with the --test flag."""
    print(f"Testing {script_name} with --test flag...")
    
    try:
        # Run the script with test flag
        result = subprocess.run(
            [sys.executable, f"sensors/{script_name}", "--test"],
            capture_output=True,
            text=True,
            timeout=15
        )
        
        if result.returncode != 0:
            print(f"  ❌ Error running {script_name} with --test: {result.stderr}")
            return False
        
        # Parse JSON array output
        try:
            data = json.loads(result.stdout.strip())
            if isinstance(data, list) and len(data) == 5:
                print(f"  ✅ {script_name} test mode: Got {len(data)} readings")
                return True
            else:
                print(f"  ❌ {script_name} test mode: Expected array of 5 readings, got {type(data)}")
                return False
        except json.JSONDecodeError as e:
            print(f"  ❌ Invalid JSON from {script_name} test mode: {e}")
            print(f"  Raw output: {result.stdout}")
            return False
            
    except subprocess.TimeoutExpired:
        print(f"  ❌ {script_name} test mode timed out")
        return False
    except Exception as e:
        print(f"  ❌ Unexpected error testing {script_name} test mode: {e}")
        return False

def main():
    """Main test function."""
    print("🧪 Testing Pi Plant Sensor Scripts")
    print("=" * 40)
    
    # Check if sensors directory exists
    if not os.path.exists("sensors"):
        print("❌ sensors directory not found!")
        print("Make sure you're running this script from the project root directory.")
        return
    
    # List of sensor scripts to test
    sensor_scripts = [
        "light_sensor.py",
        "temperature_sensor.py", 
        "soil_moisture_sensor.py",
        "humidity_sensor.py"
    ]
    
    # Test each script
    results = []
    for script in sensor_scripts:
        if os.path.exists(f"sensors/{script}"):
            # Test normal mode
            normal_result = test_sensor_script(script)
            # Test test mode
            test_result = test_sensor_script_with_test_flag(script)
            results.append((script, normal_result and test_result))
            print()
        else:
            print(f"❌ {script} not found!")
            results.append((script, False))
    
    # Summary
    print("=" * 40)
    print("📊 Test Results Summary:")
    print("=" * 40)
    
    passed = 0
    for script, result in results:
        status = "✅ PASS" if result else "❌ FAIL"
        print(f"{script}: {status}")
        if result:
            passed += 1
    
    print(f"\nOverall: {passed}/{len(results)} scripts passed")
    
    if passed == len(results):
        print("🎉 All sensor scripts are working correctly!")
    else:
        print("⚠️  Some sensor scripts have issues. Check the output above.")
        sys.exit(1)

if __name__ == "__main__":
    main() 