import React, { useState } from "react";
import EquipmentCard from "../components/EquipmentCard";

const Equipment = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("all");

  // Mock equipment data
  const equipmentList = [
    {
      id: 1,
      title: "Vélo de route Specialized Allez",
      owner: "Joseph Kennedy",
      location: "Paris 15ème",
      image:
        "https://images.unsplash.com/photo-1485965120184-e220f721d03e?w=400&h=300&fit=crop",
      category: "velo",
    },
    {
      id: 2,
      title: "Tente 4 places Quechua",
      owner: "John Cena",
      location: "Lyon 3ème",
      image:
        "https://images.unsplash.com/photo-1504280390367-361c6d9f38f4?w=400&h=300&fit=crop",
      category: "camping",
    },
    {
      id: 3,
      title: "Kayak gonflable 2 places",
      owner: "Mark Henry",
      location: "Marseille 8ème",
      image:
        "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=400&h=300&fit=crop",
      category: "nautique",
    },
    {
      id: 4,
      title: "Chaussures de randonnée Salomon",
      owner: "Undertaker",
      location: "Grenoble",
      image:
        "https://images.unsplash.com/photo-1551107696-a4b0c5a0d9a2?w=400&h=300&fit=crop",
      category: "randonnee",
    },
    {
      id: 5,
      title: "Planche de surf 7'6",
      owner: "The miz",
      location: "Biarritz",
      image:
        "https://images.unsplash.com/photo-1502680390469-be75c86b636f?w=400&h=300&fit=crop",
      category: "nautique",
    },
    {
      id: 6,
      title: "VTT électrique Decathlon",
      owner: "Randy Orton",
      location: "Toulouse",
      image:
        "https://images.unsplash.com/photo-1576435728678-68d0fbf94e91?w=400&h=300&fit=crop",
      category: "velo",
    },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-2xl font-bold text-gray-900">
            Matériel disponible
          </h1>
          <div className="flex items-center justify-between">
            <a
              href="/my-loans"
              className="bg-transparent text-orange-600 hover:text-white hover:border-orange-700 mx-1 border border-orange-600  px-6 py-2 rounded-md hover:bg-orange-700 transition-colors font-medium"
            >
              Mes prêts
            </a>
            <a
              href="/my-equipments"
              className="bg-transparent text-orange-600 hover:text-white hover:border-orange-700 mx-1 border border-orange-600  px-6 py-2 rounded-md hover:bg-orange-700 transition-colors font-medium"
            >
              Mon matériel
            </a>{" "}
            <a
              href="/share-equipment"
              className="bg-orange-600 text-white mx-1  px-6 py-2 rounded-md hover:bg-orange-700 transition-colors font-medium"
            >
              Partager mon matériel
            </a>
          </div>
        </div>

        {/* Search Bar */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-6">
          <div className="relative">
            <input
              type="text"
              placeholder="Rechercher du matériel..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-full pl-12 pr-4 py-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
            />
            <svg
              className="absolute left-4 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
              />
            </svg>
          </div>
        </div>

        {/* Filters */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-6">
          <div className="">
            {/* Category Filter */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Catégorie
              </label>
              <select
                value={selectedCategory}
                onChange={(e) => setSelectedCategory(e.target.value)}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent"
              >
                <option value="all">Toutes les catégories</option>
                <option value="velo">Vélo</option>
                <option value="camping">Camping</option>
                <option value="randonnee">Randonnée</option>
                <option value="nautique">Sports nautiques</option>
                <option value="ski">Ski & Snowboard</option>
              </select>
            </div>

        
          </div>
        </div>

        {/* Results Count */}
        <p className="text-sm text-gray-600 mb-4">
          {equipmentList.length} résultats
        </p>

        {/* Equipment Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
          {equipmentList.map((equipment) => (
            <EquipmentCard key={equipment.id} equipment={equipment} />
          ))}
        </div>
      </div>
    </div>
  );
};

export default Equipment;
