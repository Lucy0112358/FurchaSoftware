import { useState, useEffect } from 'react';
import '../menu.css';
import { assets } from '../../../assets/assets';
import CustomSelect from '../../select/CustomSelect';
import { useDispatch, useSelector } from 'react-redux';
import {
  filterUserByName,
  getBranches,
  getLockerGroupsData,
  userFilter
} from '../../../redux/api/menuApi';
import {
  getBranchesData,
  getUserGroupsData
} from '../../../redux/slice/menuSlice';
import { setPermissions } from '../../../redux/slice/authSlice';
import AddAdminModal from '../../modals/addAdmin/AddAdminModal';
import Connection from '../../connection/Connection';
import MediaQuery from 'react-responsive';
import { TbPlugConnected } from "react-icons/tb";
import UserInfoModal from '../../userInfo/UserInfoModal';
import { useHasPermission } from '../../../hooks/useHasPermission';
import ModalActionButton from '../../button/ModalActionButton';
import Manage from '../../manage/Manage';
import { getAllBranchesData } from '../../../redux/slice/branchSlice';
import { getAllBranches } from '../../../redux/api/branchApi';


function AdminMenu() {
  const dispatch = useDispatch();

  const branches = useSelector(getAllBranchesData);
  const userGroups = useSelector(getUserGroupsData);

  const [selectedBranch, setSelectedBranch] = useState(null);
  const [selectedGroup, setSelectedGroup] = useState(null);
  const [filters, setFilters] = useState({});
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [inputValue, setInputValue] = useState('');
  const [debounceTimeout, setDebounceTimeout] = useState(null);
  const { hasPermission } = useHasPermission();

  useEffect(() => {
    dispatch(getAllBranches());
    dispatch(getLockerGroupsData());
  }, [dispatch]);

  const handleSelectChange = (option, key, setSelected) => {
    setSelected(option);
    const updatedFilters = { ...filters, [key]: option.value };
    setFilters(updatedFilters);
    dispatch(userFilter(updatedFilters));
  };

  const handleSearchChange = (e) => {
    const name = e.target.value;
    setInputValue(name);
    setFilters({});

    if (debounceTimeout) clearTimeout(debounceTimeout);

    const timeout = setTimeout(() => {
      dispatch(filterUserByName({ name }));
    }, 1000);

    setDebounceTimeout(timeout);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    dispatch(setPermissions([]));
  };

  const renderSelect = (label, options, value, onChange) => (
    <div className='menu__filter__select'>
      <CustomSelect
        options={options}
        value={options.find(opt => opt.value === value?.value)}
        onChange={onChange}
      />
      <label className="text-white block">{label}</label>
    </div>
  );

  // Safe mapping
  const branchOptions = Array.isArray(branches)
    ? branches.map(branch => ({ label: branch.name, value: branch.id }))
    : [];

  const groupOptions = Array.isArray(userGroups)
    ? userGroups.map(group => ({ label: group.name, value: group.id }))
    : [];

  return (
    <>
      <div className="menu flex justify-around">
        {/* Add Admin Button */}
        {
          hasPermission() && <ModalActionButton
            onClick={() => setIsModalOpen(true)}
            iconSrc={assets.add_icon}
            text="Add Admin"
          />
        }
        {isModalOpen && <AddAdminModal onClose={handleCloseModal} />}

        {/* Filters */}
        <div className="menu__filter flex space-x-4">
          <div className='flex flex-col'>
            {renderSelect("Branch", branchOptions, selectedBranch, (opt) => handleSelectChange(opt, 'branchId', setSelectedBranch))}
            {renderSelect("User Group", groupOptions, selectedGroup, (opt) => handleSelectChange(opt, 'groupId', setSelectedGroup))}
          </div>

          {/* Search */}
          <div className='menu__filter__search'>
            <input
              type="text"
              value={inputValue}
              onChange={handleSearchChange}
              className="w-full rounded"
            />
            <label className="text-white block">Search User</label>
          </div>
        </div>

        {/* Connection Status */}
        <div className="menu__connection flex items-start text-white">
          <MediaQuery minWidth={769}><Connection /></MediaQuery>
          <MediaQuery maxWidth={768}><TbPlugConnected className='menu__connection__status__icon' /></MediaQuery>
        </div>

        {/* User Info & Manage Toggle */}
        <div className="flex items-center text-white flex-col">
          <MediaQuery minWidth={550}>
            <UserInfoModal />
          </MediaQuery>
          <Manage />
        </div>
      </div>

      {/* Mobile Filter */}
      <div className="menu__filter__mobile hidden">
        <div className='flex justify-between'>
          {renderSelect("Branch", branchOptions, selectedBranch, (opt) => handleSelectChange(opt, 'branchId', setSelectedBranch))}
          {renderSelect("User Group", groupOptions, selectedGroup, (opt) => handleSelectChange(opt, 'groupId', setSelectedGroup))}
          <div className='menu__filter__search'>
            <input type="text" className="w-full p-2 rounded" />
            <label className="text-white block">Search User</label>
          </div>
        </div>
      </div>
    </>
  );
}

export default AdminMenu;