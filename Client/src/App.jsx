import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css';
import Modules from './pages/modules/Modules';
import Layout from './components/layout/Layout';
import Signin from './pages/auth/Signin';

function App() {

  return (
    <Router>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Modules />} />
        </Route>
       <Route path="login" element={<Signin />}></Route>
      </Routes>
    </Router>
  )
}

export default App
