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


function Menu() {
  const location = useLocation();
  const dispatch = useDispatch();
  const [path, setPath] = useState('/');

  useEffect(() => {
    dispatch(getBranches());
    dispatch(getUserGroups());
  }, []);

  useEffect(() => {
    setPath(location.pathname);
  }, [location]);

  return (
    <div >
      {
        path == '/' || path == '/users' ?
          <UserMenu />
          : path == '/lockers' ?
            <LockerMenu />
            : path == '/admins' ?
              <AdminMenu />
              : path == '/modules' ?
                <ModulesMenu />
                : path == '/branches' ?
                  <BranchMenu />
                  : null
      }
    </div >
  );
}

export default Menu