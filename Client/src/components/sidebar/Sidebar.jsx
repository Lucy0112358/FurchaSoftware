import { Link, NavLink, useLocation } from 'react-router-dom';
import './sidebar.css';
import { assets } from '../../assets/assets';
import { sitebarItems } from '../../data/SidebarItems';
import { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { setSelectedLockerIds } from '../../redux/slice/lockerSlice';

const Sidebar = () => {
  const location = useLocation();
  const dispatch = useDispatch();
  useEffect(() => {
    handleSidebarChange(location.pathname);
  }, [location.pathname]);

  const handleSidebarChange = (path) => {
   dispatch(setSelectedLockerIds([]))
  };
  return (
    <div className='sidebar h-screen'>
      <div className="sidebar__logo">
        <img src={assets.logo_icon} alt="logo" />
      </div>
      <div className='sidebar__content'>
        {sitebarItems.map((item, index) => {
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
