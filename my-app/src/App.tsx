import React from 'react';

// Define types for better TypeScript support
type PlantStatus = 'healthy' | 'warning' | 'critical';

interface Plant {
  id: number;
  name: string;
  status: PlantStatus;
  location: string;
}

interface SensorData {
  soilMoisture: number;
  temperature: number;
  humidity: number;
  lightLevel: number;
  ph: number;
  lastUpdate: string;
}

function App() {
  // Mock sensor data - you'll replace this with real data later
  const sensorData: SensorData = {
    soilMoisture: 45,
    temperature: 72,
    humidity: 58,
    lightLevel: 850,
    ph: 6.8,
    lastUpdate: "2 minutes ago"
  };

  const plants: Plant[] = [
    { id: 1, name: "Basil Plant #1", status: "healthy", location: "Kitchen Window" },
    { id: 2, name: "Tomato Plant #2", status: "warning", location: "Greenhouse A" },
    { id: 3, name: "Lettuce Plant #3", status: "healthy", location: "Hydroponic Bay" }
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

  return (
    <div>
      <div className="min-h-screen bg-gray-900 p-6">
        <div className="max-w-7xl mx-auto">
          <div className="mb-8">
            <h1 className="text-4xl font-bold text-white mb-2">
              🌱 Pi-Plant Monitor
            </h1>
            <p className="text-gray-400">
              Real-time plant health monitoring system
            </p>
            <p className="text-sm text-gray-500">
              Last updated: {sensorData.lastUpdate}
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5 gap-4 mb-8">
            <div className="bg-gray-800 p-6 rounded-lg border border-gray-700">
              <h3 className="text-gray-400 text-sm mb-2">SOIL MOISTURE</h3>
              <p className={`text-3xl font-bold ${getSensorStatus(sensorData.soilMoisture, 30, 70)}`}>
                {sensorData.soilMoisture}%
              </p>
              <div className="w-full bg-gray-700 rounded-full h-2 mt-3">
                <div 
                  className="bg-blue-500 h-2 rounded-full" 
                  style={{ width: `${sensorData.soilMoisture}%` }}
                ></div>
              </div>
            </div>

            <div className="bg-gray-800 p-6 rounded-lg border border-gray-700">
              <h3 className="text-gray-400 text-sm mb-2">TEMPERATURE</h3>
              <p className={`text-3xl font-bold ${getSensorStatus(sensorData.temperature, 65, 85)}`}>
                {sensorData.temperature}°F
              </p>
              <p className="text-gray-500 text-sm mt-1">Optimal: 65-85°F</p>
            </div>

            <div className="bg-gray-800 p-6 rounded-lg border border-gray-700">
              <h3 className="text-gray-400 text-sm mb-2">HUMIDITY</h3>
              <p className={`text-3xl font-bold ${getSensorStatus(sensorData.humidity, 40, 80)}`}>
                {sensorData.humidity}%
              </p>
              <p className="text-gray-500 text-sm mt-1">Optimal: 40-80%</p>
            </div>

            <div className="bg-gray-800 p-6 rounded-lg border border-gray-700">
              <h3 className="text-gray-400 text-sm mb-2">LIGHT LEVEL</h3>
              <p className={`text-3xl font-bold ${getSensorStatus(sensorData.lightLevel, 500, 2000)}`}>
                {sensorData.lightLevel} lux
              </p>
              <p className="text-gray-500 text-sm mt-1">Optimal: 500-2000 lux</p>
            </div>

            <div className="bg-gray-800 p-6 rounded-lg border border-gray-700">
              <h3 className="text-gray-400 text-sm mb-2">SOIL pH</h3>
              <p className={`text-3xl font-bold ${getSensorStatus(sensorData.ph, 6.0, 7.5)}`}>
                {sensorData.ph}
              </p>
              <p className="text-gray-500 text-sm mt-1">Optimal: 6.0-7.5</p>
            </div>
          </div>

          <div className="bg-gray-800 rounded-lg border border-gray-700 p-6">
            <h2 className="text-2xl font-bold text-white mb-6">Plant Status</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {plants.map((plant) => (
                <div key={plant.id} className="bg-gray-700 p-4 rounded-lg border border-gray-600">
                  <h3 className="text-lg font-bold text-white mb-2">{plant.name}</h3>
                  <p className="text-gray-400 text-sm mb-3">{plant.location}</p>
                  <div className="flex items-center">
                    <span className="text-sm text-gray-400 mr-2">Status:</span>
                    <span className={`font-bold uppercase text-sm ${getStatusColor(plant.status)}`}>
                      {plant.status}
                    </span>
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