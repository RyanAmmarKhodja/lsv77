import React from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../AuthProvider";
import {
  Search,
  MessageCircle,
  Car,
  Package,
  Home,
  PlusSquare,
} from "lucide-react";



const Navbar = () => {
  const auth = useAuth();
  return (
    <nav className="border-b border-gray-200 bg-white sticky top-0 z-50">
      {/* Upper Container */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16 gap-8">
          {/* 1. Logo */}
          <div className="flex-shrink-0 flex items-center">
            <span className="text-2xl font-black text-[#F56B2A] tracking-tighter cursor-pointer">
              <NavLink to="/">CampusInsider</NavLink>
            </span>
          </div>

          {/* 2. Search Bar (Minimalist Style) */}
          <div className="hidden md:flex flex-1 max-w-md relative">
            <input
              type="text"
              placeholder="Rechercher sur CampusInsider..."
              className="w-full bg-gray-100 border-none rounded-lg py-2 pl-4 pr-10 focus:ring-2 focus:ring-[#F56B2A] focus:bg-white transition-all outline-none text-sm"
            />
            <div className="absolute right-3 top-2.5 text-gray-500">
              <Search size={18} />
            </div>
          </div>

          {/* 3. Navigation Links */}
          <div className="hidden lg:flex items-center space-x-1">
            <NavLink to="/">
              <NavLinkItem icon={<Home size={20} />} label="Accueil" />
            </NavLink>

            <NavLink to="/equipment">
              <NavLinkItem icon={<Package size={20} />} label="Matériel" />
            </NavLink>
            <NavLink to="/corides">
              <NavLinkItem icon={<Car size={20} />} label="Covoiturage" />
            </NavLink>
            <NavLink to="/">
              <NavLinkItem
                icon={<MessageCircle size={20} />}
                label="Messagerie"
              />
            </NavLink>

            <button className="bg-red-500 text-white " onClick={auth.logout}>
          LOGOUT
            </button>
          </div>

          {/* 4. Action Button (The "LeBonCoin" Orange Button) */}
          <div className="flex items-center">
            <button className="flex items-center gap-2 bg-[#F56B2A] hover:bg-[#E35B1D] text-white px-4 py-2 rounded-lg font-bold text-sm transition-colors shadow-sm">
              <PlusSquare size={18} />
              <span className="hidden sm:inline">Déposer une annonce</span>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};

// Helper Component for Links
const NavLinkItem = ({ icon, label }) => (
  <a
    href="#"
    className="flex flex-col items-center px-3 py-1 text-[#1A1A1A] hover:bg-gray-100 rounded-lg group transition-all"
  >
    <span className="text-gray-600 group-hover:text-[#F56B2A] transition-colors">
      {icon}
    </span>
    <span className="text-[10px] font-medium mt-0.5">{label}</span>
  </a>
);

export default Navbar;
