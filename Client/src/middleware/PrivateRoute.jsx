import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useSelector } from "react-redux";
import { getIsAuth, getLoading } from "../redux/slice/authSlice";

const PrivateRoute = () => {
  const isAuth = useSelector(getIsAuth)
  console.log(isAuth);
  
  const loading = useSelector(getLoading)
  if (loading) {
    return <div>Загрузка...</div>;
  }


  if (!isAuth) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
};

export default PrivateRoute;
