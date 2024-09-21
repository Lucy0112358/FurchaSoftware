import { Outlet } from 'react-router-dom';
import Sidebar from '../sidebar/Sidebar';
import './layout.css';
import Menu from '../menu/Menu';

const Layout = () => {
  return (
    <div className="layout">
      <Sidebar />
      <main className="layout__content flex flex-col w-full">
        <div className='layout__menu'>
          <Menu />
        </div>
        <div className='layout__outlet'>
          <Outlet />
        </div>
      </main>
    </div>
  );
};

export default Layout;
