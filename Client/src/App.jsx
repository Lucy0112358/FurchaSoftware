import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css';
import Modules from './pages/modules/Modules';
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
          <Route index element={<PrivateRoute><Modules /></PrivateRoute>} />
        </Route>
       <Route path="login" element={<Signin />}></Route>
      </Routes>
    </Router>
    </>
    
  )
}

export default App
