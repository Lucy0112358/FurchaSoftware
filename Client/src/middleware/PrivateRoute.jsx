import React, { useEffect } from "react";
import { Navigate, Outlet } from "react-router-dom";

const PrivateRoute = () => {

  if (!localStorage.getItem("token")) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
};

export default PrivateRoute;
