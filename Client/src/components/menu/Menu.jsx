import { useState, useEffect } from 'react';
import './menu.css';
import { useLocation } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { getBranches, getUserGroups } from '../../redux/api/menuApi';
import UserMenu from './user/UserMenu';
import LockerMenu from './locker/LockerMenu';
import ModulesMenu from './modules/ModulesMenu';
import AdminMenu from './admin/AdminMenu';
import BranchMenu from './branch/BranchMenu';
import { getLockerTypes } from '../../redux/api/lockerApi';
import ParcelMenu from './locker/parcels/ParcelMenu';
import ProfileMenu from './profile/ProfileMenu';
import UserMenuContainer from './user/UserMenuContainer';


function Menu() {
  const location = useLocation();
  const dispatch = useDispatch();
  const [path, setPath] = useState('/');

  useEffect(() => {
    dispatch(getBranches());
    dispatch(getUserGroups());
    dispatch(getLockerTypes());
  }, []);

  useEffect(() => {
    setPath(location.pathname);
  }, [location]);

  return (
    <div >
      {
        path == '/' || path == '/users' ?
          <UserMenuContainer />
          : path == '/lockers' ?
            <LockerMenu />
            : path == '/admins' ?
              <AdminMenu />
              : path == '/modules' ?
                <ModulesMenu />
                : path == '/branches' ?
                  <BranchMenu />
                  : path == '/parcels' ?
                    <ParcelMenu />
                    : path == '/profile' ?
                      <ProfileMenu />
                      : null
      }
    </div >
  );
}

export default Menu