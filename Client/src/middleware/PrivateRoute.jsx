import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Navigate } from 'react-router-dom';
// import { getCurrentUser } from '../store/slices/Auth/AuthApi';

const PrivateRoute = ({ children }) => {
    const dispatch = useDispatch();
    // const isAuth = useSelector(getIsAuth);
    // const [loading, setLoading] = useState(true);x
 
    // useEffect(() => {
    //   const fetchData = async () => {
    //     // if (!isAuth) {
    //     //   await dispatch(getCurrentUser());
    //     // }
    //     // setLoading(false); 
    //   };
    //   fetchData();
    // }, [dispatch, isAuth]);
  
    // if (loading) {
    //   return <div class="lds-ring">Loader</div>
    // }
  
    // return isAuth ? children : <Navigate to={`/login`} />;


    return children
  };
  

export default PrivateRoute;