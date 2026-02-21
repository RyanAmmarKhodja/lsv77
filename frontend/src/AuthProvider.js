import { useContext, createContext, useState, useEffect } from "react";

import { useNavigate } from "react-router-dom";
import api from "./api";
import Loading from "./components/Loading";

const AuthContext = createContext();

// AuthProvider will pass down Auth data (user, token)
// and Auth functions (login, logout) to children.
// (user, token) saved in localStorage, and then in states
// so that browser won't forget the credentials data after refresh.

const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem("token") || "");
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  // useEffect will run every refresh to "remember" token.
  useEffect(() => {
    const initAuth = async () => {
      if (token) {
        api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
        try {
          const res = await api.get("/users/me"); // Get logged-in user
          setUser(res.data);
        } catch (err) {
          console.error("Invalid or expired token:", err);
          logout(); // logout if token is bad
        }
       
      } setLoading(false);
    };

    initAuth();
  }, [token]);

  // LOGIN
  const login = async (Email, Password) => {
    try {
      const response = await api.post("/auth/login", { Email, Password });
      const token = response.data.token;

      setToken(token);
      setUser(response.data.user);

      localStorage.setItem("token", token);
      api.defaults.headers.common["Authorization"] = `Bearer ${token}`;

      navigate("/");
      return response.data;
    } catch (error) {
      throw error;
    }
  };

  // LOGOUT
  const logout = async () => {
    try {
      localStorage.removeItem("token");

      delete api.defaults.headers.common["Authorization"]; // remove token from headers
      setUser(null);
      setToken("");

      navigate("/login");
    } catch (error) {
      throw error;
    }
  };


  return (
    // Provide user, token, login and logout to children
    <AuthContext.Provider value={{ token, user, login, logout }}>
      {!loading ? children : <div className="h-screen flex items-center justify-center"><Loading/></div>}
    </AuthContext.Provider>
  );
};

export default AuthProvider;

export const useAuth = () => {
  return useContext(AuthContext);
};