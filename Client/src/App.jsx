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
              <Route path="modules" element={<Modules />} />
            </Route>
          </Route>
          <Route path="login" element={<Signin />}></Route>
        </Routes>
      </Router>
    </>

  )
}

export default App
