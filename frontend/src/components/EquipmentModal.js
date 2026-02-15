import React, { useState, useEffect } from 'react';

const EquipmentModal = ({ equipment, onClose }) => {
  const [formData, setFormData] = useState({
    title: '',
    category: '',
    description: '',
    pricePerDay: '',
    location: '',
    condition: '',
    availability: ''
  });

  useEffect(() => {
    if (equipment) {
      setFormData({
        title: equipment.title || '',
        category: equipment.category || '',
        description: equipment.description || '',
        pricePerDay: equipment.price?.replace('€/jour', '') || '',
        location: equipment.location || '',
        condition: equipment.condition || '',
        availability: equipment.availability || ''
      });
    }
  }, [equipment]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log('Updated equipment:', formData);
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <h2 className="text-xl font-bold text-gray-900">Modifier l'équipement</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition-colors"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6">
          {/* Title */}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Titre de l'annonce *
            </label>
            <input
              type="text"
              name="title"
              value={formData.title}
              onChange={handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              required
            />
          </div>

          {/* Category */}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Catégorie *
            </label>
            <select
              name="category"
              value={formData.category}
              onChange={handleChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              required
            >
              <option value="">Sélectionner une catégorie</option>
              <option value="velo">Vélo</option>
              <option value="camping">Camping</option>
              <option value="randonnee">Randonnée</option>
              <option value="nautique">Sports nautiques</option>
              <option value="ski">Ski & Snowboard</option>
              <option value="escalade">Escalade</option>
              <option value="autre">Autre</option>
            </select>
          </div>

          {/* Description */}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Description *
            </label>
            <textarea
              name="description"
              value={formData.description}
              onChange={handleChange}
              rows="4"
              className="w-full px-4 py-2 border border-gray-300 rounded-md resize-none focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              required
            />
          </div>

          {/* Price and Location */}
          <div className="grid grid-cols-2 gap-4 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Prix par jour (€) *
              </label>
              <input
                type="number"
                name="pricePerDay"
                value={formData.pricePerDay}
                onChange={handleChange}
                min="0"
                step="0.5"
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Localisation *
              </label>
              <input
                type="text"
                name="location"
                value={formData.location}
                onChange={handleChange}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
                required
              />
            </div>
          </div>

          {/* Condition and Availability */}
          <div className="grid grid-cols-2 gap-4 mb-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                État *
              </label>
              <select
                name="condition"
                value={formData.condition}
                onChange={handleChange}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
                required
              >
                <option value="">Sélectionner un état</option>
                <option value="neuf">Neuf</option>
                <option value="excellent">Excellent état</option>
                <option value="bon">Bon état</option>
                <option value="satisfaisant">État satisfaisant</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Disponibilité *
              </label>
              <select
                name="availability"
                value={formData.availability}
                onChange={handleChange}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
                required
              >
                <option value="">Sélectionner</option>
                <option value="immediate">Immédiate</option>
                <option value="week">Dans la semaine</option>
                <option value="month">Dans le mois</option>
                <option value="date">Date spécifique</option>
              </select>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex gap-3">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 px-6 py-2 border border-gray-300 rounded-md text-gray-700 font-medium hover:bg-gray-50 transition-colors"
            >
              Annuler
            </button>
            <button
              type="submit"
              className="flex-1 px-6 py-2 bg-orange-600 text-white rounded-md font-medium hover:bg-orange-700 transition-colors"
            >
              Enregistrer
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EquipmentModal;
