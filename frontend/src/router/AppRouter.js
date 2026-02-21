import { Routes, Route, Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../AuthProvider";
import ClientLayout from "../layouts/ClientLayout";
import PageNotFound from "../pages/PageNotFound";
import AuthPage from "../pages/AuthPage";
import Feed from "../pages/Feed";
import Post from "../pages/Post";
import CreatePost from "../pages/CreatePost";
import Chat from "../pages/Chat";
import Statistics from "../pages/Statistics";
import { useEffect, useState } from "react";

export default function AppRouter() {
  const auth = useAuth();
  const [user, setUser] = useState();

  // If authenticated, show the Layout + Children. If not, redirect to login.
  const PrivateRoute = () => {
    return auth.token ? (
      <ClientLayout>
        <Outlet />
      </ClientLayout>
    ) : (
      <Navigate to="/login" replace />
    );
  };

  // If authenticated, redirect home (don't show login page). If not, show AuthPage.
  const PublicRoute = () => {
    return !auth.token ? <Outlet /> : <Navigate to="/" replace />;
  };

  return (
    <Routes>
      {/* 1. Public Routes (Accessible only when logged out) */}
      <Route element={<PublicRoute />}>
        <Route path="/login" element={<AuthPage />} />
      </Route>


      {/* 2. Protected Routes (Accessible only when logged in) */}
      <Route element={<PrivateRoute />}>
        <Route path="/" element={<Feed />} />
        <Route path="/feed" element={<Feed />} />
        <Route path="/post/:id" element={<Post />} />
        <Route path="/create-post" element={<CreatePost />} />
        <Route path="/chat" element={<Chat />} />
        <Route path="/statistics" element={<Statistics />} />
      </Route>

      {/* 3. Catch-all (Outside the wrappers or inside a specific one) */}
      <Route path="*" element={<PageNotFound />} />
    </Routes>
  );
}
