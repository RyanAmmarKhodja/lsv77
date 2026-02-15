import React from 'react';

const EquipmentCard = ({ equipment, compact = false }) => {
  if (compact) {
    return (
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow cursor-pointer">
        <div className="flex">
          <img 
            src={equipment.image} 
            alt={equipment.title}
            className="w-24 h-24 object-cover"
          />
          <div className="p-3 flex-1">
            <h3 className="font-semibold text-gray-900 text-sm mb-1 line-clamp-1">
              {equipment.title}
            </h3>
            <p className="text-orange-600 font-bold text-sm mb-1">{equipment.price}</p>
            <p className="text-xs text-gray-500">{equipment.location}</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow cursor-pointer">
      <img 
        src={equipment.image} 
        alt={equipment.title}
        className="w-full h-48 object-cover"
      />
      <div className="p-4">
        <h3 className="font-semibold text-gray-900 mb-2 line-clamp-2">
          {equipment.title}
        </h3>
        <p className="text-gray-500 mb-2">{equipment.owner}</p>
        <p className="text-sm text-gray-500 mb-3">{equipment.location}</p>
        <button className="w-full bg-orange-600 text-white py-2 rounded-md hover:bg-orange-700 transition-colors font-medium">
          Louer
        </button>
      </div>
    </div>
  );
};

export default EquipmentCard;
