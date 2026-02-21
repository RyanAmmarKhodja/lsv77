import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import PageNotFound from "../pages/PageNotFound";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../AuthProvider";
import ClientLayout from "../layouts/ClientLayout";
import AuthPage from "../pages/AuthPage";
import Feed from "../pages/Feed";
import Post from "../pages/Post";
import CreatePost from "../pages/CreatePost";
import Chat from "../pages/Chat";
import Statistics from "../pages/Statistics";

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
          {/* Catch-all route for 404 */}
          <Route path="*" element={<PageNotFound />} />{" "}
          <Route path="/login" element={<AuthPage />} />
        </Route>

        {/* Protected Routes */}
        <Route element={<PrivateRoute />}>
          <Route path="/" element={<Feed />} />
          <Route path="/feed" element={<Feed />} />
          <Route path="/post/:id" element={<Post />} />
          <Route path="/create-post" element={<CreatePost />} />
          <Route path="/chat" element={<Chat />} />
          <Route path="/statistics" element={<Statistics />} />
        </Route>
      </Routes>
    </>
  );
}
