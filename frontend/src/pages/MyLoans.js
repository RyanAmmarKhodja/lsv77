import React, { useState } from 'react';

const MyLoans = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [typeFilter, setTypeFilter] = useState('all');

  // Mock loans data
  const loans = [
    {
      id: 1,
      type: 'borrowed',
      equipment: 'Vélo de route Specialized',
      owner: 'Jean Dupont',
      startDate: '10 Fév',
      endDate: '15 Fév',
      daysRemaining: 1,
      status: 'active',
      price: '75€',
      image: 'https://images.unsplash.com/photo-1485965120184-e220f721d03e?w=400&h=300&fit=crop'
    },
    {
      id: 2,
      type: 'lent',
      equipment: 'Tente 4 places Quechua',
      borrower: 'Marie Martin',
      startDate: '12 Fév',
      endDate: '20 Fév',
      daysRemaining: 6,
      status: 'active',
      price: '80€',
      image: 'https://images.unsplash.com/photo-1504280390367-361c6d9f38f4?w=400&h=300&fit=crop'
    },
    {
      id: 3,
      type: 'borrowed',
      equipment: 'Kayak gonflable',
      owner: 'Paul Bernard',
      startDate: '5 Fév',
      endDate: '10 Fév',
      status: 'completed',
      price: '100€',
      image: 'https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=400&h=300&fit=crop'
    },
    {
      id: 4,
      type: 'lent',
      equipment: 'Chaussures de randonnée',
      borrower: 'Sophie Leroux',
      startDate: '8 Fév',
      endDate: '12 Fév',
      status: 'overdue',
      daysOverdue: 2,
      price: '32€',
      image: 'https://images.unsplash.com/photo-1551107696-a4b0c5a0d9a2?w=400&h=300&fit=crop'
    },
    {
      id: 5,
      type: 'borrowed',
      equipment: 'Planche de surf',
      owner: 'Lucas Petit',
      startDate: '15 Fév',
      endDate: '18 Fév',
      status: 'pending',
      price: '36€',
      image: 'https://images.unsplash.com/photo-1502680390469-be75c86b636f?w=400&h=300&fit=crop'
    }
  ];

  const getStatusBadge = (status) => {
    const statusConfig = {
      active: { label: 'En cours', color: 'bg-blue-100 text-blue-800' },
      completed: { label: 'Terminé', color: 'bg-green-100 text-green-800' },
      overdue: { label: 'En retard', color: 'bg-red-100 text-red-800' },
      pending: { label: 'À venir', color: 'bg-gray-100 text-gray-800' }
    };
    const config = statusConfig[status];
    return (
      <span className={`px-2 py-1 rounded-full text-xs font-medium ${config.color}`}>
        {config.label}
      </span>
    );
  };

  const handleMarkComplete = (loanId) => {
    if (window.confirm('Marquer cette location comme terminée ?')) {
      console.log('Mark complete:', loanId);
    }
  };

  const handleCancelLoan = (loanId) => {
    if (window.confirm('Êtes-vous sûr de vouloir annuler cette location ?')) {
      console.log('Cancel loan:', loanId);
    }
  };

  const handleContactUser = (userName) => {
    console.log('Contact user:', userName);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-6">
        {/* Header */}
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Mes prêts</h1>
          <p className="text-gray-600 mt-1">Gérez vos locations en cours et historique</p>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
            <p className="text-sm text-gray-600 mb-1">Total locations</p>
            <p className="text-2xl font-bold text-gray-900">{loans.length}</p>
          </div>
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
            <p className="text-sm text-gray-600 mb-1">En cours</p>
            <p className="text-2xl font-bold text-blue-600">
              {loans.filter(l => l.status === 'active').length}
            </p>
          </div>
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
            <p className="text-sm text-gray-600 mb-1">En retard</p>
            <p className="text-2xl font-bold text-red-600">
              {loans.filter(l => l.status === 'overdue').length}
            </p>
          </div>
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
            <p className="text-sm text-gray-600 mb-1">Terminées</p>
            <p className="text-2xl font-bold text-green-600">
              {loans.filter(l => l.status === 'completed').length}
            </p>
          </div>
        </div>

        {/* Search and Filters */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {/* Search */}
            <div className="relative">
              <input
                type="text"
                placeholder="Rechercher..."
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

            {/* Type Filter */}
            <div>
              <select
                value={typeFilter}
                onChange={(e) => setTypeFilter(e.target.value)}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              >
                <option value="all">Tous les types</option>
                <option value="borrowed">J'emprunte</option>
                <option value="lent">Je prête</option>
              </select>
            </div>

            {/* Status Filter */}
            <div>
              <select
                value={statusFilter}
                onChange={(e) => setStatusFilter(e.target.value)}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              >
                <option value="all">Tous les statuts</option>
                <option value="active">En cours</option>
                <option value="pending">À venir</option>
                <option value="overdue">En retard</option>
                <option value="completed">Terminé</option>
              </select>
            </div>
          </div>
        </div>

        {/* Loans List */}
        <div className="space-y-4">
          {loans.map(loan => (
            <div key={loan.id} className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 hover:shadow-md transition-shadow">
              <div className="flex gap-4">
                {/* Image */}
                <img 
                  src={loan.image}
                  alt={loan.equipment}
                  className="w-32 h-32 object-cover rounded-md flex-shrink-0"
                />

                {/* Content */}
                <div className="flex-1 min-w-0">
                  <div className="flex items-start justify-between mb-2">
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-1">
                        <h3 className="font-semibold text-gray-900 text-lg">{loan.equipment}</h3>
                        {getStatusBadge(loan.status)}
                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                          loan.type === 'borrowed' 
                            ? 'bg-purple-100 text-purple-800' 
                            : 'bg-orange-100 text-orange-800'
                        }`}>
                          {loan.type === 'borrowed' ? 'J\'emprunte' : 'Je prête'}
                        </span>
                      </div>
                      <p className="text-sm text-gray-600">
                        {loan.type === 'borrowed' 
                          ? `Propriétaire: ${loan.owner}`
                          : `Emprunteur: ${loan.borrower}`
                        }
                      </p>
                    </div>
                    <p className="text-orange-600 font-bold text-lg">{loan.price}</p>
                  </div>

                  {/* Date Info */}
                  <div className="flex items-center gap-4 text-sm text-gray-600 mb-3">
                    <span className="flex items-center gap-1">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                      </svg>
                      Du {loan.startDate} au {loan.endDate}
                    </span>
                    {loan.status === 'active' && loan.daysRemaining && (
                      <span className={`font-medium ${loan.daysRemaining <= 2 ? 'text-orange-600' : 'text-gray-600'}`}>
                        {loan.daysRemaining} jour{loan.daysRemaining > 1 ? 's' : ''} restant{loan.daysRemaining > 1 ? 's' : ''}
                      </span>
                    )}
                    {loan.status === 'overdue' && (
                      <span className="font-medium text-red-600">
                        Retard de {loan.daysOverdue} jour{loan.daysOverdue > 1 ? 's' : ''}
                      </span>
                    )}
                  </div>

                  {/* Status Alert */}
                  {loan.status === 'overdue' && (
                    <div className="bg-red-50 border border-red-200 rounded-md p-2 mb-3">
                      <p className="text-sm text-red-800">
                        ⚠️ Cette location est en retard. Veuillez contacter {loan.type === 'borrowed' ? 'le propriétaire' : 'l\'emprunteur'}.
                      </p>
                    </div>
                  )}

                  {/* Actions */}
                  <div className="flex items-center gap-2">
                    {loan.type === 'borrowed' && loan.status === 'active' && (
                      <button
                        onClick={() => handleMarkComplete(loan.id)}
                        className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 transition-colors text-sm font-medium"
                      >
                        Marquer comme rendu
                      </button>
                    )}
                    {loan.type === 'lent' && loan.status === 'active' && (
                      <button
                        onClick={() => handleMarkComplete(loan.id)}
                        className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 transition-colors text-sm font-medium"
                      >
                        Marquer comme récupéré
                      </button>
                    )}
                    {loan.status === 'pending' && (
                      <button
                        onClick={() => handleCancelLoan(loan.id)}
                        className="px-4 py-2 border border-red-300 rounded-md text-red-600 hover:bg-red-50 transition-colors text-sm font-medium"
                      >
                        Annuler
                      </button>
                    )}
                    <button
                      onClick={() => handleContactUser(loan.type === 'borrowed' ? loan.owner : loan.borrower)}
                      className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 transition-colors text-sm font-medium"
                    >
                      Contacter
                    </button>
                    {loan.status === 'completed' && (
                      <span className="text-sm text-gray-500 ml-auto">Location terminée</span>
                    )}
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Empty State */}
        {loans.length === 0 && (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-12 text-center">
            <svg className="mx-auto h-12 w-12 text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
            </svg>
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Aucune location</h3>
            <p className="text-gray-600 mb-4">Vous n'avez pas encore de locations en cours</p>
            <a
              href="/equipment"
              className="inline-block bg-orange-600 text-white px-6 py-2 rounded-md hover:bg-orange-700 transition-colors font-medium"
            >
              Découvrir le matériel disponible
            </a>
          </div>
        )}
      </div>
    </div>
  );
};

export default MyLoans;
