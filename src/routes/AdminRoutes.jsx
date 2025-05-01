import React from "react";
import { Navigate, Outlet } from "react-router";
import { useUserContext } from "./ProtectedRoute";
import { environment } from "../constants/environment";
const AdminRoutes = () => {
  const { roleName } = useUserContext();

  if (roleName === environment.adminRole) {
    return <Outlet />;
  }

  return <Navigate to="/forbidden" replace />;
};

export default AdminRoutes;
