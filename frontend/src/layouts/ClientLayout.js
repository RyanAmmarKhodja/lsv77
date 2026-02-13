import { Outlet } from "react-router-dom";
import Navbar from "../components/Navbar";

export default function ClientLayout() {
  return (
    <>
      <Navbar loggedIn={false} />
      <div >
        <Outlet />
      </div>
    </>
  );
}