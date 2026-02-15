import React, { useState } from 'react';
import EquipmentModal from '../components/EquipmentModal';

const MyEquipments = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [selectedEquipment, setSelectedEquipment] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  // Mock equipment data
  const myEquipments = [
    {
      id: 1,
      title: 'Vélo de route Specialized',
      category: 'Vélo',
      price: '15€/jour',
      status: 'available',
      image: 'https://images.unsplash.com/photo-1485965120184-e220f721d03e?w=400&h=300&fit=crop',
      views: 45,
      loans: 3
    },
    {
      id: 2,
      title: 'Tente 4 places Quechua',
      category: 'Camping',
      price: '10€/jour',
      status: 'rented',
      image: 'https://images.unsplash.com/photo-1504280390367-361c6d9f38f4?w=400&h=300&fit=crop',
      views: 32,
      loans: 5,
      rentedUntil: '20 Fév'
    },
    {
      id: 3,
      title: 'Chaussures de randonnée Salomon',
      category: 'Randonnée',
      price: '8€/jour',
      status: 'inactive',
      image: 'https://images.unsplash.com/photo-1551107696-a4b0c5a0d9a2?w=400&h=300&fit=crop',
      views: 12,
      loans: 1
    },
    {
      id: 4,
      title: 'Kayak gonflable 2 places',
      category: 'Nautique',
      price: '20€/jour',
      status: 'available',
      image: 'https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=400&h=300&fit=crop',
      views: 28,
      loans: 2
    }
  ];

  const getStatusBadge = (status) => {
    const statusConfig = {
      available: { label: 'Disponible', color: 'bg-green-100 text-green-800' },
      rented: { label: 'Loué', color: 'bg-orange-100 text-orange-800' },
      inactive: { label: 'Inactif', color: 'bg-gray-100 text-gray-800' }
    };
    const config = statusConfig[status];
    return (
      <span className={`px-2 py-1 rounded-full text-xs font-medium ${config.color}`}>
        {config.label}
      </span>
    );
  };

  const handleEdit = (equipment) => {
    setSelectedEquipment(equipment);
    setIsModalOpen(true);
  };

  const handleDelete = (equipmentId) => {
    if (window.confirm('Êtes-vous sûr de vouloir supprimer cet équipement ?')) {
      console.log('Delete equipment:', equipmentId);
    }
  };

  const handleToggleStatus = (equipmentId) => {
    console.log('Toggle status for:', equipmentId);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Mes équipements</h1>
            <p className="text-gray-600 mt-1">Gérez vos annonces de location</p>
          </div>
          <a
            href="/share-equipment"
            className="bg-orange-600 text-white px-6 py-2 rounded-md hover:bg-orange-700 transition-colors font-medium"
          >
            + Ajouter un équipement
          </a>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
            <p className="text-sm text-gray-600 mb-1">Total équipements</p>
            <p className="text-2xl font-bold text-gray-900">{myEquipments.length}</p>
          </div>
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
            <p className="text-sm text-gray-600 mb-1">Disponibles</p>
            <p className="text-2xl font-bold text-green-600">
              {myEquipments.filter(e => e.status === 'available').length}
            </p>
          </div>
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
            <p className="text-sm text-gray-600 mb-1">Actuellement loués</p>
            <p className="text-2xl font-bold text-orange-600">
              {myEquipments.filter(e => e.status === 'rented').length}
            </p>
          </div>
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
            <p className="text-sm text-gray-600 mb-1">Total locations</p>
            <p className="text-2xl font-bold text-gray-900">
              {myEquipments.reduce((sum, e) => sum + e.loans, 0)}
            </p>
          </div>
        </div>

        {/* Search and Filters */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Search */}
            <div className="relative">
              <input
                type="text"
                placeholder="Rechercher dans mes équipements..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              />
              <svg 
                className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400"
                fill="none" 
                stroke="currentColor" 
                viewBox="0 0 24 24"
              >
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>

            {/* Status Filter */}
            <div>
              <select
                value={statusFilter}
                onChange={(e) => setStatusFilter(e.target.value)}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              >
                <option value="all">Tous les statuts</option>
                <option value="available">Disponible</option>
                <option value="rented">Loué</option>
                <option value="inactive">Inactif</option>
              </select>
            </div>
          </div>
        </div>

        {/* Equipment List */}
        <div className="space-y-4">
          {myEquipments.map(equipment => (
            <div key={equipment.id} className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 hover:shadow-md transition-shadow">
              <div className="flex gap-4">
                {/* Image */}
                <img 
                  src={equipment.image}
                  alt={equipment.title}
                  className="w-32 h-32 object-cover rounded-md flex-shrink-0"
                />

                {/* Content */}
                <div className="flex-1 min-w-0">
                  <div className="flex items-start justify-between mb-2">
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-1">
                        <h3 className="font-semibold text-gray-900 text-lg">{equipment.title}</h3>
                        {getStatusBadge(equipment.status)}
                      </div>
                      <p className="text-sm text-gray-600">{equipment.category}</p>
                    </div>
                    <p className="text-orange-600 font-bold text-lg">{equipment.price}</p>
                  </div>

                  {equipment.status === 'rented' && equipment.rentedUntil && (
                    <div className="bg-orange-50 border border-orange-200 rounded-md p-2 mb-3">
                      <p className="text-sm text-orange-800">
                        <span className="font-medium">Loué jusqu'au {equipment.rentedUntil}</span>
                      </p>
                    </div>
                  )}

                  {/* Stats */}
                  <div className="flex items-center gap-4 text-sm text-gray-600 mb-3">
                    <span className="flex items-center gap-1">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                      </svg>
                      {equipment.views} vues
                    </span>
                    <span className="flex items-center gap-1">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                      {equipment.loans} locations
                    </span>
                  </div>

                  {/* Actions */}
                  <div className="flex items-center gap-2">
                    <button
                      onClick={() => handleEdit(equipment)}
                      className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 transition-colors text-sm font-medium"
                    >
                      Modifier
                    </button>
                    <button
                      onClick={() => handleToggleStatus(equipment.id)}
                      className={`px-4 py-2 rounded-md text-sm font-medium transition-colors ${
                        equipment.status === 'inactive'
                          ? 'bg-green-600 text-white hover:bg-green-700'
                          : 'border border-gray-300 text-gray-700 hover:bg-gray-50'
                      }`}
                    >
                      {equipment.status === 'inactive' ? 'Activer' : 'Désactiver'}
                    </button>
                    <button
                      onClick={() => handleDelete(equipment.id)}
                      className="px-4 py-2 border border-red-300 rounded-md text-red-600 hover:bg-red-50 transition-colors text-sm font-medium ml-auto"
                    >
                      Supprimer
                    </button>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Empty State */}
        {myEquipments.length === 0 && (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center">
            <svg className="mx-auto h-12 w-12 text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
            </svg>
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Aucun équipement</h3>
            <p className="text-gray-600 mb-4">Commencez à partager votre matériel avec la communauté</p>
            <a
              href="/share-equipment"
              className="inline-block bg-orange-600 text-white px-6 py-2 rounded-md hover:bg-orange-700 transition-colors font-medium"
            >
              Ajouter mon premier équipement
            </a>
          </div>
        )}
      </div>

      {/* Edit Modal */}
      {isModalOpen && (
        <EquipmentModal
          equipment={selectedEquipment}
          onClose={() => setIsModalOpen(false)}
        />
      )}
    </div>
  );
};

export default MyEquipments;
