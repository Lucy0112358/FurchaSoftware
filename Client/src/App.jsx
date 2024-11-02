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



function App() {

  return (
    <>
    <ToastContainer />
    <Router>
      <Routes>
        <Route path="/" element={<PrivateRoute><Layout /></PrivateRoute>}>
          <Route index  path="/" element={<PrivateRoute><Users /></PrivateRoute>} />
          <Route index  path="/users" element={<PrivateRoute><Users /></PrivateRoute>} />
          <Route path="/lockers" element={<PrivateRoute><Lockers /></PrivateRoute>} />
          <Route path="modules" element={<PrivateRoute><Modules /></PrivateRoute>} />
        </Route>
       <Route path="login" element={<Signin />}></Route>
      </Routes>
    </Router>
    </>
    
  )
}

export default App
