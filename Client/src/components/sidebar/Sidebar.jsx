import { Link, NavLink } from 'react-router-dom';
import './sidebar.css';
import { assets } from '../../assets/assets';
import { sitebarItems } from '../../data/SidebarItems';


const Sidebar = () => {
  return (
   <div className='sidebar h-screen '>
        <div className="sidebar__logo">
          <img src={assets.logo_icon} alt="logo"/>
        </div>
        <div className='sidebar__content'>
            {sitebarItems.map((item, index) => (
                <Link to={item.path} className='sidebar__content__link'>
                    <item.icon />
                    <p className='hidden lg:inline'>{item.name}</p>
                </Link>
            ))}
        </div>
   </div>
  );
};

export default Sidebar;
