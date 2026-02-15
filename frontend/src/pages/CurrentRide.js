import React, { useState } from 'react';

const CurrentRide = () => {
  // Mock ride data - in reality this would come from props or API
  const [isDriver, setIsDriver] = useState(true); // Toggle to test both views
  
  const rideData = {
    id: 1,
    status: 'upcoming', // upcoming, in_progress, completed
    driver: {
      name: 'Thomas Martin',
      phone: '+33 6 12 34 56 78',
      rating: 4.8
    },
    passengers: [
      { id: 1, name: 'Marie Dubois', phone: '+33 6 23 45 67 89', seats: 1 },
      { id: 2, name: 'Lucas Bernard', phone: '+33 6 34 56 78 90', seats: 2 }
    ],
    from: 'Paris',
    fromAddress: '12 Rue de Rivoli, 75001 Paris',
    to: 'Lyon',
    toAddress: '25 Rue de la République, 69002 Lyon',
    date: '15 Février 2026',
    departureTime: '08:00',
    estimatedArrival: '12:30',
    distance: '460 km',
    duration: '4h30',
    price: '25€',
    vehicle: {
      model: 'Renault Mégane',
      color: 'Gris',
      plate: 'AB-123-CD'
    },
    meetingPoint: 'Parking Porte Maillot',
    notes: 'Prévoir 15 minutes d\'avance. Possibilité d\'un arrêt pause sur l\'autoroute.',
    seatsBooked: 3,
    seatsTotal: 3
  };

  const handleBeginRide = () => {
    if (window.confirm('Commencer le trajet ?')) {
      console.log('Begin ride');
    }
  };

  const handleCompleteRide = () => {
    if (window.confirm('Marquer le trajet comme terminé ?')) {
      console.log('Complete ride');
    }
  };

  const handleCancelRide = () => {
    if (window.confirm('Êtes-vous sûr de vouloir annuler ce trajet ? Cette action est irréversible.')) {
      console.log('Cancel ride');
    }
  };

  const handleLeaveRide = () => {
    if (window.confirm('Êtes-vous sûr de vouloir quitter ce trajet ?')) {
      console.log('Leave ride');
    }
  };

  const handleContactPerson = (name) => {
    console.log('Contact:', name);
  };

  const getStatusBadge = (status) => {
    const statusConfig = {
      upcoming: { label: 'À venir', color: 'bg-blue-100 text-blue-800' },
      in_progress: { label: 'En cours', color: 'bg-orange-100 text-orange-800' },
      completed: { label: 'Terminé', color: 'bg-green-100 text-green-800' }
    };
    const config = statusConfig[status];
    return (
      <span className={`px-3 py-1 rounded-full text-sm font-medium ${config.color}`}>
        {config.label}
      </span>
    );
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-4xl mx-auto px-4 py-6">
        {/* Header */}
        <div className="mb-6">
          <div className="flex items-center gap-2 mb-2">
            <a href="/covoiturages" className="text-gray-600 hover:text-gray-900">
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
              </svg>
            </a>
            <h1 className="text-2xl font-bold text-gray-900">Mon trajet</h1>
            {getStatusBadge(rideData.status)}
          </div>
        </div>

        {/* Main Card */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 mb-6">
          {/* Route Header */}
          <div className="p-6 border-b border-gray-200">
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-3 flex-1">
                <div className="text-center min-w-[100px]">
                  <p className="font-bold text-gray-900 text-2xl">{rideData.from}</p>
                  <p className="text-sm text-gray-500">{rideData.departureTime}</p>
                </div>
                <div className="flex-1 flex items-center gap-2">
                  <div className="flex-1 h-px bg-gray-300"></div>
                  <svg className="w-6 h-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                  </svg>
                  <div className="flex-1 h-px bg-gray-300"></div>
                </div>
                <div className="text-center min-w-[100px]">
                  <p className="font-bold text-gray-900 text-2xl">{rideData.to}</p>
                  <p className="text-sm text-gray-500">{rideData.estimatedArrival}</p>
                </div>
              </div>
              <div className="text-right ml-6">
                <p className="text-3xl font-bold text-orange-600">{rideData.price}</p>
                <p className="text-sm text-gray-500">par personne</p>
              </div>
            </div>

            <div className="flex items-center gap-6 text-sm text-gray-600">
              <span className="flex items-center gap-1">
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                {rideData.date}
              </span>
              <span className="flex items-center gap-1">
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                </svg>
                {rideData.distance}
              </span>
              <span className="flex items-center gap-1">
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                {rideData.duration}
              </span>
            </div>
          </div>

          {/* Meeting Point */}
          <div className="p-6 border-b border-gray-200">
            <h3 className="font-semibold text-gray-900 mb-3">Point de rendez-vous</h3>
            <div className="bg-gray-50 rounded-lg p-4">
              <p className="font-medium text-gray-900 mb-1">{rideData.meetingPoint}</p>
              <p className="text-sm text-gray-600">{rideData.fromAddress}</p>
            </div>
          </div>

          {/* Vehicle Info */}
          <div className="p-6 border-b border-gray-200">
            <h3 className="font-semibold text-gray-900 mb-3">Véhicule</h3>
            <div className="flex items-center gap-4">
              <div className="w-12 h-12 bg-gray-100 rounded-lg flex items-center justify-center">
                <svg className="w-6 h-6 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
                </svg>
              </div>
              <div>
                <p className="font-medium text-gray-900">{rideData.vehicle.model}</p>
                <p className="text-sm text-gray-600">{rideData.vehicle.color} • {rideData.vehicle.plate}</p>
              </div>
            </div>
          </div>

          {/* Driver Section */}
          <div className="p-6 border-b border-gray-200">
            <h3 className="font-semibold text-gray-900 mb-3">Conducteur</h3>
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="w-12 h-12 bg-orange-100 rounded-full flex items-center justify-center">
                  <span className="text-orange-600 font-semibold">
                    {rideData.driver.name.split(' ').map(n => n[0]).join('')}
                  </span>
                </div>
                <div>
                  <p className="font-medium text-gray-900">{rideData.driver.name}</p>
                  <div className="flex items-center gap-1 text-sm">
                    <svg className="w-4 h-4 text-yellow-400 fill-current" viewBox="0 0 20 20">
                      <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                    </svg>
                    <span className="font-medium text-gray-700">{rideData.driver.rating}</span>
                  </div>
                </div>
              </div>
              <button
                onClick={() => handleContactPerson(rideData.driver.name)}
                className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 transition-colors text-sm font-medium"
              >
                Contacter
              </button>
            </div>
          </div>

          {/* Passengers Section */}
          <div className="p-6 border-b border-gray-200">
            <div className="flex items-center justify-between mb-3">
              <h3 className="font-semibold text-gray-900">
                Passagers ({rideData.passengers.length} • {rideData.seatsBooked}/{rideData.seatsTotal} places)
              </h3>
            </div>
            <div className="space-y-3">
              {rideData.passengers.map(passenger => (
                <div key={passenger.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 bg-orange-100 rounded-full flex items-center justify-center">
                      <span className="text-orange-600 font-semibold text-sm">
                        {passenger.name.split(' ').map(n => n[0]).join('')}
                      </span>
                    </div>
                    <div>
                      <p className="font-medium text-gray-900">{passenger.name}</p>
                      <p className="text-sm text-gray-600">{passenger.seats} place{passenger.seats > 1 ? 's' : ''}</p>
                    </div>
                  </div>
                  <button
                    onClick={() => handleContactPerson(passenger.name)}
                    className="px-3 py-1 border border-gray-300 rounded-md text-gray-700 hover:bg-white transition-colors text-sm font-medium"
                  >
                    Contacter
                  </button>
                </div>
              ))}
            </div>
          </div>

          {/* Notes */}
          {rideData.notes && (
            <div className="p-6 border-b border-gray-200">
              <h3 className="font-semibold text-gray-900 mb-2">Informations complémentaires</h3>
              <p className="text-sm text-gray-600">{rideData.notes}</p>
            </div>
          )}

          {/* Action Buttons */}
          <div className="p-6">
            {isDriver ? (
              <div className="space-y-3">
                {rideData.status === 'upcoming' && (
                  <>
                    <button
                      onClick={handleBeginRide}
                      className="w-full bg-orange-600 text-white py-3 rounded-md hover:bg-orange-700 transition-colors font-medium"
                    >
                      Commencer le trajet
                    </button>
                    <button
                      onClick={handleCancelRide}
                      className="w-full border border-red-300 text-red-600 py-3 rounded-md hover:bg-red-50 transition-colors font-medium"
                    >
                      Annuler le trajet
                    </button>
                  </>
                )}
                {rideData.status === 'in_progress' && (
                  <button
                    onClick={handleCompleteRide}
                    className="w-full bg-green-600 text-white py-3 rounded-md hover:bg-green-700 transition-colors font-medium"
                  >
                    Terminer le trajet
                  </button>
                )}
                {rideData.status === 'completed' && (
                  <div className="text-center py-4">
                    <svg className="mx-auto h-12 w-12 text-green-500 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <p className="text-lg font-medium text-gray-900">Trajet terminé</p>
                    <p className="text-sm text-gray-600 mt-1">Merci d'avoir partagé votre trajet !</p>
                  </div>
                )}
              </div>
            ) : (
              <div className="space-y-3">
                {(rideData.status === 'upcoming' || rideData.status === 'in_progress') && (
                  <button
                    onClick={handleLeaveRide}
                    className="w-full border border-red-300 text-red-600 py-3 rounded-md hover:bg-red-50 transition-colors font-medium"
                  >
                    Quitter le trajet
                  </button>
                )}
                {rideData.status === 'completed' && (
                  <div className="text-center py-4">
                    <svg className="mx-auto h-12 w-12 text-green-500 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <p className="text-lg font-medium text-gray-900">Trajet terminé</p>
                    <p className="text-sm text-gray-600 mt-1">Merci d'avoir voyagé avec nous !</p>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>

        {/* Toggle for testing (remove in production) */}
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 text-center">
          <p className="text-sm text-blue-800 mb-2">Mode de test:</p>
          <button
            onClick={() => setIsDriver(!isDriver)}
            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors text-sm font-medium"
          >
            Voir comme {isDriver ? 'Passager' : 'Conducteur'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default CurrentRide;
