import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from "../pages/Login";
import PageNotFound from "../pages/PageNotFound";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../AuthProvider";
import ClientLayout from "../layouts/ClientLayout";
import Home from "../pages/Home";
import Equipment from "../pages/Equipment";
import ShareEquipment from "../pages/ShareEquipment";
import MyLoans from "../pages/MyLoans";
import MyEquipments from "../pages/MyEquipments";
import CoRide from "../pages/CoRide";
import CurrentRide from "../pages/CurrentRide";
import AuthPage from "../pages/AuthPage";

export default function AppRouter() {
  const PrivateRoute = () => {
    const auth = useAuth();
    return auth.token ? <ClientLayout /> : <Navigate to="/login" />;
  };

  function PublicRoute() {
    const auth = useAuth();
    return auth.token ? <ClientLayout /> : <ClientLayout />;
  }

  return (
    <>
      <Routes>
        <Route element={<PublicRoute />}>
          <Route path="/login" element={<AuthPage />} />

          {/* Catch-all route for 404 */}
          <Route path="*" element={<PageNotFound />} />
        </Route>

        {/* Protected Routes */}
        <Route element={<PrivateRoute />}>
          <Route path="/" element={<Home />} />
          <Route path="/equipment" element={<Equipment />} />
          <Route path="/share-equipment" element={<ShareEquipment />} />
          <Route path="/login" element={<Login />} />
          <Route path="/my-loans" element={<MyLoans />} />
          <Route path="/my-equipments" element={<MyEquipments />} />
          <Route path="/corides" element={<CoRide />} />
          <Route path="/current-ride" element={<CurrentRide />} />
        </Route>
      </Routes>
    </>
  );
}
