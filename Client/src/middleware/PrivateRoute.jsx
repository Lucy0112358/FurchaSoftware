import React, { useEffect } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useDispatch, useSelector } from 'react-redux';
import { getAuthUser } from "../redux/api/authApi";
import Loader from "../components/loader/Loader";


// const PrivateRoute = () => {

//   if (!localStorage.getItem("token")) {
//     return <Navigate to="/login" replace />;
//   }

//   return <Outlet />;
// };

const PrivateRoute = () => {
  const dispatch = useDispatch();
  const { authUser, loading } = useSelector((state) => state.auth);

  useEffect(() => {
    console.log(Object.keys(authUser).length === 0, 'authUser in private route');

    if (Object.keys(authUser).length === 0 && localStorage.getItem("token")) {
      dispatch(getAuthUser()).then((res) => {
        if (res.meta.requestStatus !== "fulfilled") {
          return <Navigate to="/login" replace />
        }
      });
    }
  }, [dispatch, authUser]);
  // console.log(authUser, 'authUser');
  //   if (Object.keys(authUser).length === 0 ) return <Navigate to="/login" replace />;

  return <Outlet />;
};

export default PrivateRoute;
