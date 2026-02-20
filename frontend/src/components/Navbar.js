import React, { useState, useEffect, useRef } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../AuthProvider";
import {
  Search,
  MessageCircle,
  Car,
  Package,
  Home,
  PlusSquare,
  Bell,
  User,
  LogOut,
  X,
} from "lucide-react";

const Navbar = () => {
  const auth = useAuth();
  const navigate = useNavigate();
  const [showNotifications, setShowNotifications] = useState(false);
  const [showProfile, setShowProfile] = useState(false);
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [user, setUser] = useState(null);

  const notificationRef = useRef(null);
  const profileRef = useRef(null);

  // Close dropdowns when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (notificationRef.current && !notificationRef.current.contains(event.target)) {
        setShowNotifications(false);
      }
      if (profileRef.current && !profileRef.current.contains(event.target)) {
        setShowProfile(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  // Fetch user info
  useEffect(() => {
    const fetchUser = async () => {
      try {
        const token = localStorage.getItem("authToken");
        const response = await fetch("/api/users/me", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (response.ok) {
          const data = await response.json();
          setUser(data);
        }
      } catch (error) {
        console.error("Failed to fetch user:", error);
      }
    };

    fetchUser();
  }, []);

  // Fetch notifications
  useEffect(() => {
    const fetchNotifications = async () => {
      try {
        const token = localStorage.getItem("authToken");
        const response = await fetch("/api/notifications?pageSize=5&isRead=false", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (response.ok) {
          const data = await response.json();
          setNotifications(data.items || []);
        }
      } catch (error) {
        console.error("Failed to fetch notifications:", error);
      }
    };

    fetchNotifications();
    const interval = setInterval(fetchNotifications, 30000); // Poll every 30 seconds
    return () => clearInterval(interval);
  }, []);

  // Fetch unread count
  useEffect(() => {
    const fetchUnreadCount = async () => {
      try {
        const token = localStorage.getItem("authToken");
        const response = await fetch("/api/notifications/unread-count", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (response.ok) {
          const data = await response.json();
          setUnreadCount(data.count || 0);
        }
      } catch (error) {
        console.error("Failed to fetch unread count:", error);
      }
    };

    fetchUnreadCount();
    const interval = setInterval(fetchUnreadCount, 30000);
    return () => clearInterval(interval);
  }, []);

  const handleMarkAsRead = async (notificationId) => {
    try {
      const token = localStorage.getItem("authToken");
      await fetch(`/api/notifications/${notificationId}/read`, {
        method: "PATCH",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      // Update local state
      setNotifications((prev) =>
        prev.map((n) => (n.id === notificationId ? { ...n, isRead: true } : n))
      );
      setUnreadCount((prev) => Math.max(0, prev - 1));
    } catch (error) {
      console.error("Failed to mark as read:", error);
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      const token = localStorage.getItem("authToken");
      await fetch("/api/notifications/read-all", {
        method: "PATCH",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      setNotifications((prev) => prev.map((n) => ({ ...n, isRead: true })));
      setUnreadCount(0);
    } catch (error) {
      console.error("Failed to mark all as read:", error);
    }
  };

  const formatTimeAgo = (date) => {
    const seconds = Math.floor((new Date() - new Date(date)) / 1000);
    if (seconds < 60) return "À l'instant";
    if (seconds < 3600) return `Il y a ${Math.floor(seconds / 60)} min`;
    if (seconds < 86400) return `Il y a ${Math.floor(seconds / 3600)}h`;
    return `Il y a ${Math.floor(seconds / 86400)}j`;
  };

  return (
    <nav className="border-b border-gray-200 bg-white sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16 gap-8">
          {/* 1. Logo */}
          <div className="flex-shrink-0 flex items-center">
            <NavLink to="/" className="text-2xl font-black text-[#F56B2A] tracking-tighter">
              CampusInsider
            </NavLink>
          </div>

          {/* 2. Search Bar */}
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
            
            <NavLink to="/chat">
              <NavLinkItem icon={<MessageCircle size={20} />} label="Messages" />
            </NavLink>

            {/* Notifications Dropdown */}
            <div className="relative" ref={notificationRef}>
              <button
                onClick={() => setShowNotifications(!showNotifications)}
                className="flex flex-col items-center px-3 py-1 text-[#1A1A1A] hover:bg-gray-100 rounded-lg group transition-all relative"
              >
                <span className="text-gray-600 group-hover:text-[#F56B2A] transition-colors relative">
                  <Bell size={20} />
                  {unreadCount > 0 && (
                    <span className="absolute -top-1 -right-1 bg-red-500 text-white text-[10px] font-bold rounded-full h-4 w-4 flex items-center justify-center">
                      {unreadCount > 9 ? "9+" : unreadCount}
                    </span>
                  )}
                </span>
                <span className="text-[10px] font-medium mt-0.5">Notifications</span>
              </button>

              {/* Notifications Dropdown */}
              {showNotifications && (
                <div className="absolute right-0 mt-2 w-96 bg-white rounded-lg shadow-lg border border-gray-200 max-h-[500px] overflow-hidden flex flex-col">
                  <div className="p-4 border-b border-gray-200 flex justify-between items-center">
                    <h3 className="font-bold text-gray-900">Notifications</h3>
                    {unreadCount > 0 && (
                      <button
                        onClick={handleMarkAllAsRead}
                        className="text-xs text-[#F56B2A] hover:underline"
                      >
                        Tout marquer comme lu
                      </button>
                    )}
                  </div>

                  <div className="overflow-y-auto flex-1">
                    {notifications.length === 0 ? (
                      <div className="p-8 text-center text-gray-500">
                        <Bell size={48} className="mx-auto mb-2 text-gray-300" />
                        <p>Aucune notification</p>
                      </div>
                    ) : (
                      notifications.map((notif) => (
                        <div
                          key={notif.id}
                          className={`p-4 border-b border-gray-100 hover:bg-gray-50 cursor-pointer ${
                            !notif.isRead ? "bg-blue-50" : ""
                          }`}
                          onClick={() => {
                            handleMarkAsRead(notif.id);
                            if (notif.actionUrl) {
                              navigate(notif.actionUrl);
                              setShowNotifications(false);
                            }
                          }}
                        >
                          <div className="flex justify-between items-start gap-2">
                            <div className="flex-1">
                              <h4 className="font-semibold text-sm text-gray-900">
                                {notif.title}
                              </h4>
                              <p className="text-sm text-gray-600 mt-1">{notif.message}</p>
                              <p className="text-xs text-gray-400 mt-2">
                                {formatTimeAgo(notif.createdAt)}
                              </p>
                            </div>
                            {!notif.isRead && (
                              <div className="w-2 h-2 bg-[#F56B2A] rounded-full flex-shrink-0 mt-1"></div>
                            )}
                          </div>
                        </div>
                      ))
                    )}
                  </div>

                  <div className="p-3 border-t border-gray-200 text-center">
                    <button
                      onClick={() => {
                        navigate("/notifications");
                        setShowNotifications(false);
                      }}
                      className="text-sm text-[#F56B2A] hover:underline font-medium"
                    >
                      Voir toutes les notifications
                    </button>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* 4. Action Button & Profile */}
          <div className="flex items-center gap-3">
            <NavLink to="/create-post">
              <button className="flex items-center gap-2 bg-[#F56B2A] hover:bg-[#E35B1D] text-white px-4 py-2 rounded-lg font-bold text-sm transition-colors shadow-sm">
                <PlusSquare size={18} />
                <span className="hidden sm:inline">Déposer une annonce</span>
              </button>
            </NavLink>

            {/* Profile Dropdown */}
            <div className="relative" ref={profileRef}>
              <button
                onClick={() => setShowProfile(!showProfile)}
                className="flex items-center gap-2 px-3 py-2 rounded-lg hover:bg-gray-100 transition-colors"
              >
                <div className="w-8 h-8 bg-[#F56B2A] rounded-full flex items-center justify-center text-white font-bold">
                  {user?.firstName?.[0]?.toUpperCase() || "U"}
                </div>
                <span className="hidden md:block font-medium text-gray-900">
                  {user?.firstName || "User"}
                </span>
              </button>

              {/* Profile Dropdown Menu */}
              {showProfile && (
                <div className="absolute right-0 mt-2 w-56 bg-white rounded-lg shadow-lg border border-gray-200 py-2">
                  <div className="px-4 py-3 border-b border-gray-200">
                    <p className="font-semibold text-gray-900">
                      {user?.firstName} {user?.lastName}
                    </p>
                    <p className="text-sm text-gray-500">{user?.email}</p>
                  </div>

                  <button
                    onClick={() => {
                      navigate("/profile");
                      setShowProfile(false);
                    }}
                    className="w-full px-4 py-2 text-left hover:bg-gray-100 flex items-center gap-2 text-gray-700"
                  >
                    <User size={18} />
                    <span>Mon profil</span>
                  </button>

                  <button
                    onClick={() => {
                      auth.logout();
                      setShowProfile(false);
                    }}
                    className="w-full px-4 py-2 text-left hover:bg-gray-100 flex items-center gap-2 text-red-600"
                  >
                    <LogOut size={18} />
                    <span>Déconnexion</span>
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
};

// Helper Component for Links
const NavLinkItem = ({ icon, label }) => (
  <div className="flex flex-col items-center px-3 py-1 text-[#1A1A1A] hover:bg-gray-100 rounded-lg group transition-all">
    <span className="text-gray-600 group-hover:text-[#F56B2A] transition-colors">
      {icon}
    </span>
    <span className="text-[10px] font-medium mt-0.5">{label}</span>
  </div>
);

export default Navbar;