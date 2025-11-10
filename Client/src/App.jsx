import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css';
import Users from './pages/users/Users';
import Layout from './components/layout/Layout';
import Signin from './pages/auth/Signin';
import PrivateRoute from './middleware/PrivateRoute';
import 'react-toastify/dist/ReactToastify.css';
import { ToastContainer } from 'react-toastify';
import Modules from './pages/modules/Modules';
import Lockers from './pages/lockers/Lockers';
import Admins from './pages/admins/Admins';
import Branch from './pages/branch/Branch';
import { useDispatch } from 'react-redux';
import { useEffect } from 'react';
import { getAuthUser } from './redux/api/authApi';
import NotFound from './pages/not-found/NotFound';
import Parcels from './pages/lockers/parcels/Parcels';
import Profile from './pages/profile/Profile';

function App() {

  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(getAuthUser());
  }, [dispatch]);

  return (
    <>
      <ToastContainer />
      <Router>
        <Routes>
          <Route element={<PrivateRoute />}>
            <Route path="/" element={<Layout />}>
              <Route index path="/" element={<Users />} />
              <Route index path="/users" element={<Users />} />
              <Route path="/admins" element={<Admins />} />
              <Route path="/branches" element={<Branch />} />
              <Route path="/lockers" element={<Lockers />} />
              <Route path="/parcels" element={<Parcels />} />
              <Route path="/modules" element={<Modules />} />
              <Route path="/profile" element={<Profile />} />
            </Route>
          </Route>
          <Route path="login" element={<Signin />}></Route>
          <Route path="*" element={<NotFound />} />
        </Routes>
      </Router>
    </>

  )
}

export default App
