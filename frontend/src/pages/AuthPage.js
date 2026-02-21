import React, { useState } from "react";
import { Mail, Lock, User, Eye, EyeOff, ArrowRight } from "lucide-react";
import api from "../api";
import Loading from "../components/Loading";
import { useAuth } from "../AuthProvider";
import { useNavigate } from "react-router-dom";

const AuthPage = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [formData, setFormData] = useState({
    Email: "",
    Password: "",
    FirstName: "",
    LastName: "",
  });

  const auth = useAuth();
  const navigate = useNavigate();

  React.useEffect(() => {
    if (auth.token) {
      navigate("/");
    }
  }, [auth.token, navigate]); // Added dependency array to prevent infinite loops

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      if (isLogin) {
        // --- LOGIN LOGIC ---
        await auth.login(formData.Email, formData.Password);
        console.log("Logged in");
        // useEffect will handle the redirect to "/"
      } else {
        // --- REGISTER LOGIC ---
        const response = await api.post("/users/register", formData);

        setSuccess("Compte créé avec succès !");

        // Switch to login view and pre-fill the email
        setIsLogin(true);
        setFormData({
          Email: response.data.email || formData.Email,
          Password: "",
          FirstName: "",
          LastName: "",
        });
      }
    } catch (err) {
      console.error("Login Error:", err);
      // Handle both object {message: ""} and string errors
      const serverMessage = err.response?.data?.message || err.response?.data;
      setError(serverMessage || "Une erreur est survenue.");
    } finally {
      setLoading(false);
    }
  };

  // Style Constants (LeBonCoin Palette)
  const colors = {
    primary: "#F56B2A", // LBC Orange
    text: "#1A1A1A", // Deep Gray/Black
    bg: "#F4F6F7", // Soft background gray
    link: "#4183D7", // LBC Blue for links
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-[#F4F6F7] px-4 font-sans">
      {/* 1. Header/Logo Area */}
      <div className="mb-8 text-center">
        <h1 className="text-3xl font-black text-[#F56B2A] tracking-tighter mb-2">
          CampusInsider
        </h1>
        <p className="text-gray-600 text-sm">
          {isLogin
            ? "Bonjour ! Connectez-vous pour voir vos annonces."
            : "Rejoignez la communauté CampusInsider."}
        </p>
      </div>

      {/* 2. Auth Card */}
      <div className="w-full max-w-md bg-white rounded-2xl shadow-xl shadow-gray-200/50 p-8 border border-gray-100">
        <h2 className="text-xl font-bold text-[#1A1A1A] mb-6">
          {isLogin ? "Se connecter" : "Créer un compte"}
        </h2>
        

        {error && (
          <p className="text-white bg-red-500 p-4 rounded-2xl">
            {typeof error === "object" ? error.message : error}
          </p>
        )}

        {success && (
          <p className="text-white bg-green-500 p-4 rounded-2xl">{success}</p>
        )}

<div className="text-center mb-5">
     {loading && <Loading />}
</div>
      
        <form className="space-y-4" onSubmit={handleSubmit}>
          {/* Name Field (Register Only) */}
          {!isLogin && (
            <div>
              <label className="block text-xs font-bold uppercase tracking-wider text-gray-500 my-1 ml-1">
                Prénom
              </label>
              <div className="relative">
                <User
                  className="absolute left-3 top-3 text-gray-400"
                  size={18}
                />
                <input
                  name="FirstName"
                  value={formData.FirstName}
                  onChange={handleChange}
                  type="text"
                  placeholder="Jean"
                  className="w-full pl-10 pr-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-[#F56B2A] focus:border-transparent outline-none transition-all"
                />
              </div>
              <label className="block text-xs font-bold uppercase tracking-wider text-gray-500 my-1 ml-1">
                Nom
              </label>
              <div className="relative">
                <User
                  className="absolute left-3 top-3 text-gray-400"
                  size={18}
                />
                <input
                  value={formData.LastName}
                  onChange={handleChange}
                  type="text"
                  name="LastName"
                  placeholder="Dupont "
                  className="w-full pl-10 pr-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-[#F56B2A] focus:border-transparent outline-none transition-all"
                />
              </div>
            </div>
          )}

          {/* Email Field */}
          <div>
            <label className="block text-xs font-bold uppercase tracking-wider text-gray-500 mb-1 ml-1">
              Adresse Email (@lsv77.fr)
            </label>
            <div className="relative">
              <Mail className="absolute left-3 top-3 text-gray-400" size={18} />
              <input
                value={formData.Email}
                onChange={handleChange}
                type="Email"
                name="Email"
                placeholder="nom@lsv77.fr"
                className="w-full pl-10 pr-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-[#F56B2A] focus:border-transparent outline-none transition-all"
              />
            </div>
          </div>

          {/* Password Field */}
          <div>
            <div className="flex justify-between items-center mb-1 ml-1">
              <label className="block text-xs font-bold uppercase tracking-wider text-gray-500">
                Mot de passe
              </label>
              {isLogin && (
                <a
                  href="#"
                  className="text-xs font-semibold text-[#4183D7] hover:underline"
                >
                  Oublié ?
                </a>
              )}
            </div>
            <div className="relative">
              <Lock className="absolute left-3 top-3 text-gray-400" size={18} />
              <input
                type={showPassword ? "text" : "Password"}
                value={formData.Password}
                onChange={handleChange}
                name="Password"
                placeholder="••••••••"
                className="w-full pl-10 pr-12 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-[#F56B2A] focus:border-transparent outline-none transition-all"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-3 text-gray-400 hover:text-gray-600"
              >
                {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
              </button>
              
            </div>
          </div>
         
          {/* Submit Button */}
          <button className="w-full bg-[#F56B2A] hover:bg-[#E35B1D] text-white font-bold py-3 rounded-xl transition-all shadow-lg shadow-orange-200 flex items-center justify-center gap-2 group mt-6">
            {isLogin ? "Me connecter" : "Continuer"}
            
            <ArrowRight
              size={18}
              className="group-hover:translate-x-1 transition-transform"
            />
          </button>
        </form>

        {/* 3. Toggle Footer */}
        <div className="mt-8 pt-6 border-t border-gray-100 text-center">
          <p className="text-sm text-gray-600">
            {isLogin ? "Pas encore de compte ?" : "Vous avez déjà un compte ?"}
            <button
              onClick={() => setIsLogin(!isLogin)}
              className="ml-2 font-bold text-[#1A1A1A] hover:text-[#F56B2A] transition-colors"
            >
              {isLogin ? "Créer un compte" : "Se connecter"}
            </button>
          </p>
        </div>
      </div>

      {/* Footer Disclaimer */}
      <p className="mt-8 text-[10px] text-gray-400 text-center max-w-xs uppercase tracking-widest">
        En vous connectant, vous acceptez nos{" "}
        <span className="underline">Conditions Générales</span> et notre{" "}
        <span className="underline">Politique de Confidentialité</span>.
      </p>
    </div>
  );
};

export default AuthPage;
