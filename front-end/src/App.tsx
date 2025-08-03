import React from 'react';

// Define types for better TypeScript support
type PlantStatus = 'healthy' | 'warning' | 'critical';
type NotificationType = 'watering' | 'light' | 'temperature' | 'humidity';

interface Plant {
  id: number;
  name: string;
  status: PlantStatus;
  location: string;
  sensorData: {
    soilMoisture: number;
    temperature: number;
    humidity: number;
    lightLevel: number;
    ph: number;
  };
  lightLevelMin: number;
  lightLevelMax: number;
  temperatureMin: number, // °F
  temperatureMax: number, // °F
  humidityMin: number, // %
  humidityMax: number, // %
 // notifications: NotificationType[];
}

function App() {
  // Mock plant data with individual sensor readings
  const plants: Plant[] = [
    { 
      id: 1, 
      name: "String of Turtles", 
      status: "healthy", 
      location: "Office",
      sensorData: {
        soilMoisture: 65,
        temperature: 72,
        humidity: 60,
        lightLevel: 3000,
        ph: 6.8,
      },
      lightLevelMin: 1000,
      lightLevelMax: 15000,
      temperatureMin: 50, // °F
      temperatureMax: 85, // °F
      humidityMin: 30, // %
      humidityMax: 70, // %
     // notifications: []
    }
  ];

  const getStatusColor = (status: PlantStatus): string => {
    switch (status) {
      case 'healthy': return 'text-green-400';
      case 'warning': return 'text-yellow-400';
      case 'critical': return 'text-red-400';
      default: return 'text-gray-400';
    }
  };

  const getSensorStatus = (value: number, min: number, max: number): string => {
    if (value < min || value > max) return 'text-red-400';
    if (value < min + 10 || value > max - 10) return 'text-yellow-400';
    return 'text-green-400';
  };

  const getNotificationText = (type: NotificationType): string => {
    switch (type) {
      case 'watering': return 'W';
      case 'light': return 'L';
      case 'temperature': return 'T';
      case 'humidity': return 'H';
      default: return '!';
    }
  };

  const getNotificationMessage = (type: NotificationType): string => {
    switch (type) {
      case 'watering': return 'Needs watering';
      case 'light': return 'Needs more light';
      case 'temperature': return 'Temperature needs adjustment';
      case 'humidity': return 'Humidity too low';
      default: return 'Needs attention';
    }
  };

  const handleEdit = (plantId: number) => {
    console.log(`Edit plant ${plantId}`);
    // Implement edit functionality
  };

  const handleDelete = (plantId: number) => {
    console.log(`Delete plant ${plantId}`);
    // Implement delete functionality
  };

  return (
    <div>
      <div className="min-h-screen bg-gray-900 p-6">
        <div className="max-w-7xl mx-auto">
          <div className="mb-8">
            <h1 className="text-4xl font-bold text-white mb-2">
              🌱 Pi-Plant
            </h1>
            <p className="text-gray-400">
              Real-time plant health monitoring system
            </p>
            <p className="text-sm text-gray-500">
              Last updated: 2 minutes ago
            </p>
          </div>

          <div className="bg-gray-800 rounded-lg border border-gray-700 p-6">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-2xl font-bold text-white">Plants</h2>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {plants.map((plant) => (
                <div key={plant.id} className="bg-gray-700 p-6 rounded-lg border border-gray-600 relative">
                  

                  <h3 className="text-lg font-bold text-white mb-2 pr-16">{plant.name} (#{plant.id})</h3>
                  <p className="text-gray-400 text-sm mb-4">{plant.location}</p>
                  
                  <div className="flex items-center mb-4">
                    <span className="text-sm text-gray-400 mr-2">Status:</span>
                    <span className={`font-bold uppercase text-sm ${getStatusColor(plant.status)}`}>
                      {plant.status}
                    </span>
                  </div>

                  {/* Sensor Data Grid */}
                  <div className="grid grid-cols-2 gap-3 mb-4">
                    <div className="bg-gray-800 p-3 rounded border border-gray-600">
                      <h4 className="text-gray-400 text-xs mb-1">SOIL MOISTURE</h4>
                      <p className={`text-lg font-bold ${getSensorStatus(plant.sensorData.soilMoisture, 30, 70)}`}>
                        {plant.sensorData.soilMoisture}%
                      </p>
                      <div className="w-full bg-gray-700 rounded-full h-1 mt-2">
                        <div 
                          className="bg-blue-500 h-1 rounded-full" 
                          style={{ width: `${plant.sensorData.soilMoisture}%` }}
                        ></div>
                      </div>
                    </div>

                    <div className="bg-gray-800 p-3 rounded border border-gray-600">
                      <h4 className="text-gray-400 text-xs mb-1">TEMPERATURE</h4>
                      <p className={`text-lg font-bold ${getSensorStatus(plant.sensorData.temperature, plant.temperatureMin, plant.temperatureMax)}`}>
                        {plant.sensorData.temperature}°F
                      </p>
                      <p className="text-gray-500 text-xs mt-1">{plant.temperatureMin}-{plant.temperatureMax}°F</p>
                    </div>

                    <div className="bg-gray-800 p-3 rounded border border-gray-600">
                      <h4 className="text-gray-400 text-xs mb-1">HUMIDITY</h4>
                      <p className={`text-lg font-bold ${getSensorStatus(plant.sensorData.humidity, plant.humidityMin, plant.humidityMax)}`}>
                        {plant.sensorData.humidity}%
                      </p>
                      <p className="text-gray-500 text-xs mt-1">{plant.humidityMin}-{plant.humidityMax}%</p>
                    </div>

                    <div className="bg-gray-800 p-3 rounded border border-gray-600">
                      <h4 className="text-gray-400 text-xs mb-1">LIGHT LEVEL</h4>
                      <p className={`text-lg font-bold ${getSensorStatus(plant.sensorData.lightLevel, plant.lightLevelMin, plant.lightLevelMax)}`}>
                        {plant.sensorData.lightLevel}
                      </p>
                      <p className="text-gray-500 text-xs mt-1">lux</p>
                    </div>
                  </div>

                  {/* pH as a single row item */}
                  { /*
                  <div className="bg-gray-800 p-3 rounded border border-gray-600">
                    <h4 className="text-gray-400 text-xs mb-1">SOIL pH</h4>
                    <div className="flex justify-between items-center">
                      <p className={`text-lg font-bold ${getSensorStatus(plant.sensorData.ph, 6.0, 7.5)}`}>
                        {plant.sensorData.ph}
                      </p>
                      <p className="text-gray-500 text-xs">Optimal: 6.0-7.5</p>
                    </div>
                  </div>
                  */
                  }
                  {/* Individual plant action buttons */}
                  <div className="flex gap-2 mt-4">
                    <button 
                      className="flex-1 px-3 py-2 bg-gray-600 hover:bg-gray-500 text-white rounded transition-colors text-sm"
                      onClick={() => handleEdit(plant.id)}
                    >
                      Edit
                    </button>
                    <button 
                      className="flex-1 px-3 py-2 bg-red-600 hover:bg-red-700 text-white rounded transition-colors text-sm"
                      onClick={() => handleDelete(plant.id)}
                    >
                      Delete
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="mt-8 bg-gray-800 rounded-lg border border-gray-700 p-6">
            <h2 className="text-2xl font-bold text-white mb-4">System Status</h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="flex items-center">
                <div className="w-3 h-3 bg-green-500 rounded-full mr-3"></div>
                <span className="text-gray-300">Sensors Online</span>
              </div>
              <div className="flex items-center">
                <div className="w-3 h-3 bg-green-500 rounded-full mr-3"></div>
                <span className="text-gray-300">Pi Connection Active</span>
              </div>
              <div className="flex items-center">
                <div className="w-3 h-3 bg-yellow-500 rounded-full mr-3"></div>
                <span className="text-gray-300">Data Sync: 2min delay</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;