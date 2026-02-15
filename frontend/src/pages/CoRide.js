import React, { useState } from 'react';

const CoRide = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [fromFilter, setFromFilter] = useState('');
  const [toFilter, setToFilter] = useState('');
  const [dateFilter, setDateFilter] = useState('');

  // Mock rides data
  const rides = [
    {
      id: 1,
      driver: 'Thomas Martin',
      driverRating: 4.8,
      from: 'Paris',
      to: 'Lyon',
      date: '15 Fév',
      time: '08:00',
      price: '25€',
      seats: 3,
      seatsAvailable: 3,
      vehicle: 'Renault Mégane',
      distance: '460 km',
      duration: '4h30'
    },
    {
      id: 2,
      driver: 'Sophie Dubois',
      driverRating: 4.9,
      from: 'Marseille',
      to: 'Nice',
      date: '16 Fév',
      time: '14:00',
      price: '15€',
      seats: 2,
      seatsAvailable: 1,
      vehicle: 'Peugeot 308',
      distance: '200 km',
      duration: '2h30'
    },
    {
      id: 3,
      driver: 'Lucas Bernard',
      driverRating: 4.7,
      from: 'Toulouse',
      to: 'Bordeaux',
      date: '17 Fév',
      time: '09:30',
      price: '20€',
      seats: 4,
      seatsAvailable: 4,
      vehicle: 'Citroën C4',
      distance: '245 km',
      duration: '2h45'
    },
    {
      id: 4,
      driver: 'Marie Leroux',
      driverRating: 5.0,
      from: 'Paris',
      to: 'Bordeaux',
      date: '18 Fév',
      time: '07:00',
      price: '35€',
      seats: 3,
      seatsAvailable: 2,
      vehicle: 'Volkswagen Golf',
      distance: '580 km',
      duration: '5h45'
    },
    {
      id: 5,
      driver: 'Antoine Petit',
      driverRating: 4.6,
      from: 'Lyon',
      to: 'Grenoble',
      date: '15 Fév',
      time: '16:00',
      price: '12€',
      seats: 3,
      seatsAvailable: 3,
      vehicle: 'Renault Clio',
      distance: '105 km',
      duration: '1h20'
    },
    {
      id: 6,
      driver: 'Camille Rousseau',
      driverRating: 4.8,
      from: 'Nantes',
      to: 'Paris',
      date: '19 Fév',
      time: '06:30',
      price: '30€',
      seats: 2,
      seatsAvailable: 2,
      vehicle: 'Peugeot 2008',
      distance: '385 km',
      duration: '3h50'
    }
  ];

  const handleJoinRide = (rideId) => {
    console.log('Join ride:', rideId);
    // Navigate to ride details or booking page
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Covoiturages disponibles</h1>
            <p className="text-gray-600 mt-1">Trouvez un trajet ou proposez le vôtre</p>
          </div>
          <a
            href="/share-ride"
            className="bg-orange-600 text-white px-6 py-2 rounded-md hover:bg-orange-700 transition-colors font-medium"
          >
            Proposer un trajet
          </a>
        </div>

        {/* Search Bar */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-6">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            {/* From */}
            <div className="relative">
              <input
                type="text"
                placeholder="Départ..."
                value={fromFilter}
                onChange={(e) => setFromFilter(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              />
              <svg 
                className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400"
                fill="none" 
                stroke="currentColor" 
                viewBox="0 0 24 24"
              >
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
            </div>

            {/* To */}
            <div className="relative">
              <input
                type="text"
                placeholder="Destination..."
                value={toFilter}
                onChange={(e) => setToFilter(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              />
              <svg 
                className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400"
                fill="none" 
                stroke="currentColor" 
                viewBox="0 0 24 24"
              >
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
            </div>

            {/* Date */}
            <div className="relative">
              <input
                type="date"
                value={dateFilter}
                onChange={(e) => setDateFilter(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              />
              <svg 
                className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400"
                fill="none" 
                stroke="currentColor" 
                viewBox="0 0 24 24"
              >
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
            </div>

            {/* Search Button */}
            <button className="bg-orange-600 text-white py-2 px-6 rounded-md hover:bg-orange-700 transition-colors font-medium">
              Rechercher
            </button>
          </div>
        </div>

        {/* Results Count */}
        <p className="text-sm text-gray-600 mb-4">
          {rides.length} trajets disponibles
        </p>

        {/* Rides List */}
        <div className="space-y-4">
          {rides.map(ride => (
            <div key={ride.id} className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 hover:shadow-md transition-shadow">
              <div className="flex gap-4">
                {/* Driver Avatar */}
                <div className="flex-shrink-0">
                  <div className="w-16 h-16 bg-orange-100 rounded-full flex items-center justify-center">
                    <span className="text-orange-600 font-semibold text-lg">
                      {ride.driver.split(' ').map(n => n[0]).join('')}
                    </span>
                  </div>
                  <div className="mt-2 text-center">
                    <div className="flex items-center justify-center gap-1 text-sm">
                      <svg className="w-4 h-4 text-yellow-400 fill-current" viewBox="0 0 20 20">
                        <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                      </svg>
                      <span className="font-medium">{ride.driverRating}</span>
                    </div>
                  </div>
                </div>

                {/* Ride Details */}
                <div className="flex-1 min-w-0">
                  {/* Driver Name */}
                  <h3 className="font-semibold text-gray-900 mb-3">{ride.driver}</h3>

                  {/* Route */}
                  <div className="flex items-center gap-3 mb-3">
                    <div className="flex items-center gap-3 flex-1">
                      <div className="text-center min-w-[80px]">
                        <p className="font-bold text-gray-900 text-lg">{ride.from}</p>
                        <p className="text-xs text-gray-500">{ride.time}</p>
                      </div>
                      <div className="flex-1 flex items-center gap-2">
                        <div className="flex-1 h-px bg-gray-300"></div>
                        <svg className="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                        </svg>
                        <div className="flex-1 h-px bg-gray-300"></div>
                      </div>
                      <div className="text-center min-w-[80px]">
                        <p className="font-bold text-gray-900 text-lg">{ride.to}</p>
                        <p className="text-xs text-gray-500">{ride.duration}</p>
                      </div>
                    </div>
                  </div>

                  {/* Additional Info */}
                  <div className="flex items-center gap-4 text-sm text-gray-600 mb-3">
                    <span className="flex items-center gap-1">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                      </svg>
                      {ride.date}
                    </span>
                    <span className="flex items-center gap-1">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                      </svg>
                      {ride.distance}
                    </span>
                    <span className="flex items-center gap-1">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
                      </svg>
                      {ride.vehicle}
                    </span>
                    <span className="flex items-center gap-1">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                      </svg>
                      {ride.seatsAvailable}/{ride.seats} places
                    </span>
                  </div>

                  {/* Availability Alert */}
                  {ride.seatsAvailable === 1 && (
                    <div className="bg-orange-50 border border-orange-200 rounded-md p-2 mb-3">
                      <p className="text-sm text-orange-800">⚠️ Plus qu'une place disponible !</p>
                    </div>
                  )}
                </div>

                {/* Price and Action */}
                <div className="flex flex-col items-end justify-between flex-shrink-0">
                  <div className="text-right">
                    <p className="text-2xl font-bold text-orange-600">{ride.price}</p>
                    <p className="text-xs text-gray-500">par personne</p>
                  </div>
                  <button
                    onClick={() => handleJoinRide(ride.id)}
                    className="bg-orange-600 text-white px-6 py-2 rounded-md hover:bg-orange-700 transition-colors font-medium"
                  >
                    Rejoindre
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Empty State */}
        {rides.length === 0 && (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center">
            <svg className="mx-auto h-12 w-12 text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7" />
            </svg>
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Aucun trajet trouvé</h3>
            <p className="text-gray-600 mb-4">Essayez de modifier vos critères de recherche</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default CoRide;
