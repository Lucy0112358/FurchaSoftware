import { useState, useRef, useEffect } from 'react';
import './menu.css';
import { RiAddBoxLine } from "react-icons/ri";
import { assets } from '../../assets/assets';
import CustomSelect from '../select/CustomSelect';
import { useLocation } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { filterUserByName, getBranches, getUserGroups, userFilter } from '../../redux/api/menuApi';
import { getSelectGroupSelect, getBranchesData, getUserGroupsData, setMenuFilter, setUserGroupSelect } from '../../redux/slice/menuSlice';
import AddUserModal from '../modals/addUser/AddUserModal';
import { getAllUsers } from '../../redux/api/userApi';
import { getAllGroups } from '../../redux/api/groupApi';
import GeneralAddModal from '../modals/GeneralAddModal';
import { TbPlugConnected } from "react-icons/tb";
import MediaQuery from 'react-responsive'
import LockerTypes from './locker/components/lockerTypes/LockerTypes';
import UserMenu from './user/UserMenu';
import LockerMenu from './locker/LockerMenu';
import ModulesMenu from './modules/ModulesMenu';
import AdminMenu from './admin/AdminMenu';


function Menu() {
  const location = useLocation();
  const dispatch = useDispatch();
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [selectedGroups, setSelectedGroups] = useState(null);
  const userGroupEnabled = useSelector(getSelectGroupSelect);
  const [filters, setFilters] = useState({});
  const [path, setPath] = useState('/');

  //input search by nane
  const [inputValue, setInputValue] = useState('');
  const [debounceTimeout, setDebounceTimeout] = useState(null);

  const handleSelectChange = (selectedOption) => {
    setSelectedBranch(selectedOption);
    addFilters(selectedOption, 'branchId')
  }
  const handleGroupsSelectChange = (selectedOption) => {
    setSelectedGroups(selectedOption);
    addFilters(selectedOption, 'groupId')

    // dispatch(userFilter({ 'filterByGroupId': selectedOption.value }));
  };

  const addFilters = (selectedOption, key) => {
    setFilters((prevFilters) => {
      const updatedFilters = {
        ...prevFilters,
        [key]: selectedOption.value,
      };

      dispatch(userFilter(updatedFilters));
      return updatedFilters;
    });
  }

  //Add USER modal part 
  // const [isModalOpen, setIsModalOpen] = useState(false);

  // const userBranches = useSelector(getBranchesData);
  // const userGroups = useSelector(getUserGroupsData);
  // const [manageEnabled, setManageEnabled] = useState(true);

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
                : null
      }
    </div >
  );
}

export default Menu