import { Link, NavLink } from 'react-router-dom';
import './sidebar.css';
import { assets } from '../../assets/assets';
import { sitebarItems } from '../../data/SidebarItems';


const Sidebar = () => {
  return (
    <div className='sidebar h-screen'>
      <div className="sidebar__logo">
        <img src={assets.logo_icon} alt="logo" />
      </div>
      <div className='sidebar__content'>
        {sitebarItems.map((item, index) => {
          const isActive = location.pathname.includes('locker') && item.path.includes('locker');
          
          return (
            <NavLink 
              key={index} 
              to={item.path}
              className={'sidebar__content__link'}
            >
              <item.icon />
              <p className='hidden lg:inline'>{item.name}</p>
            </NavLink>
          );
        })}
      </div>
    </div>
  );
};

export default Sidebar;
