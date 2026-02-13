import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../AuthProvider";
import { NavLink } from "react-router-dom";

function Navbar(props) {
  const auth = useAuth();
  const navigate = useNavigate();
  const [error, setError] = React.useState(null);
  const jungleActive = { color: "#1C7435", fontWeight: "bold" };

  const [search, setSearch] = useState("");

  const handleSearch = (e) => {
    e.preventDefault();
    navigate(`/catalogue?search=${encodeURIComponent(search)}`);
  };

  const submit = async () => {
    try {
      const res = await auth.logout();
      console.log("Logged out", res);
      navigate("/");
    } catch (err) {
      setError(err.response?.data?.message || "Login failed");
    }
  };
  return (
    <>
      <nav
        className="navbar navbar-expand-lg"
        style={{
          position: "fixed",
          width: "100%",
          zIndex: 10,
          backgroundColor: "rgba(255, 255, 255, 0.95)",
          backdropFilter: "blur(8px)",
          boxShadow: "0 2px 15px rgba(0,0,0,0.05)",
          borderBottom: "1px solid #e9ecef",
          transition: "all 0.3s ease",
        }}
        data-bs-theme="light"
      >
        <div
          className="container-fluid"
          style={props.loggedIn ? { paddingLeft: "90px" } : {}}
        >
          <NavLink
            className="navbar-brand"
            to="/"
            style={{
              textDecoration: "none",
              color: "#1C7435",
              fontWeight: "800",
              fontSize: "1.6rem",
              letterSpacing: "-0.5px",
              display: "flex",
              alignItems: "center",
              transition: "transform 0.2s ease",
            }}
            onMouseEnter={(e) =>
              (e.currentTarget.style.transform = "scale(1.02)")
            }
            onMouseLeave={(e) => (e.currentTarget.style.transform = "scale(1)")}
          >
            <span style={{ marginRight: "8px" }}></span> Azbobinette
          </NavLink>

          <button
            className="navbar-toggler border-0"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#navbarColor03"
            aria-controls="navbarColor03"
            aria-expanded="false"
            aria-label="Toggle navigation"
          >
            <span className="navbar-toggler-icon"></span>
          </button>

          <div className="collapse navbar-collapse" id="navbarColor03">
            <ul className="navbar-nav me-auto mb-2 mb-lg-0">
              <li className="nav-item">
                <NavLink
                  className="nav-link mx-2"
                  to="/"
                  style={({ isActive }) => ({
                    color: isActive ? "#1C7435" : "#6c757d",
                    textDecoration: "none",
                    fontWeight: isActive ? "600" : "500",
                    borderBottom: isActive
                      ? "2px solid #1C7435"
                      : "2px solid transparent",
                    transition: "all 0.2s ease",
                    paddingBottom: "2px",
                  })}
                >
                  Accueil
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink
                  className="nav-link mx-2"
                  to="/catalogue"
                  style={({ isActive }) => ({
                    color: isActive ? "#1C7435" : "#6c757d",
                    textDecoration: "none",
                    fontWeight: isActive ? "600" : "500",
                    borderBottom: isActive
                      ? "2px solid #1C7435"
                      : "2px solid transparent",
                    transition: "all 0.2s ease",
                    paddingBottom: "2px",
                  })}
                >
                  Animaux
                </NavLink>
              </li>
               <li className="nav-item">
                <NavLink
                  className="nav-link mx-2"
                  to="/about"
                  style={({ isActive }) => ({
                    color: isActive ? "#1C7435" : "#6c757d",
                    textDecoration: "none",
                    fontWeight: isActive ? "600" : "500",
                    borderBottom: isActive
                      ? "2px solid #1C7435"
                      : "2px solid transparent",
                    transition: "all 0.2s ease",
                    paddingBottom: "2px",
                  })}
                >
                  Préparer sa visite
                </NavLink>
              </li>
            </ul>

            <form
              className="d-flex align-items-center"
              onSubmit={handleSearch}
              style={{ position: "relative" }}
            >
              <input
                className="form-control me-2"
                type="search"
                placeholder="Rechercher un animal..."
                onChange={(e) => setSearch(e.target.value)}
                value={search}
                style={{
                  borderRadius: "20px",
                  backgroundColor: "#f8f9fa",
                  border: "1px solid #dee2e6",
                  paddingLeft: "15px",
                  fontSize: "0.9rem",
                  width: "250px",
                  transition: "width 0.3s ease, box-shadow 0.3s ease",
                }}
                onFocus={(e) => {
                  e.target.style.width = "300px";
                  e.target.style.boxShadow =
                    "0 0 0 0.25rem rgba(28, 116, 53, 0.1)";
                }}
                onBlur={(e) => (e.target.style.width = "250px")}
              />
              <button
                className="btn"
                type="submit"
                style={{
                  backgroundColor: "#1C7435",
                  color: "white",
                  borderRadius: "20px",
                  padding: "6px 20px",
                  fontWeight: "500",
                  transition: "all 0.2s ease",
                }}
                onMouseEnter={(e) =>
                  (e.currentTarget.style.backgroundColor = "#155d2a")
                }
                onMouseLeave={(e) =>
                  (e.currentTarget.style.backgroundColor = "#1C7435")
                }
              >
                Chercher
              </button>
            </form>

            {props.loggedIn && (
              <a
                className="btn btn-link"
                href="#"
                onClick={submit}
                style={{
                  marginLeft: "15px",
                  color: "#dc3545",
                  textDecoration: "none",
                  fontSize: "0.9rem",
                  fontWeight: "500",
                }}
              >
                Déconnexion
              </a>
            )}
          </div>
        </div>
      </nav>
    </>
  );
}

export default Navbar;