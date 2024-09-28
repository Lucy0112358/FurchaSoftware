import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css';
import Users from './pages/users/Users';
import Layout from './components/layout/Layout';
import Signin from './pages/auth/Signin';
import PrivateRoute from './middleware/PrivateRoute';
import 'react-toastify/dist/ReactToastify.css';
import { ToastContainer } from 'react-toastify';



function App() {

  return (
    <>
    <ToastContainer />
    <Router>
      <Routes>
        <Route path="/" element={<PrivateRoute><Layout /></PrivateRoute>}>
          <Route index element={<PrivateRoute><Users /></PrivateRoute>} />
        </Route>
       <Route path="login" element={<Signin />}></Route>
      </Routes>
    </Router>
    </>
    
  )
}

export default App
