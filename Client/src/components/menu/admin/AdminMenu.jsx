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

function AdminMenu() {
  const dispatch = useDispatch();

  const branches = useSelector(getBranchesData);
  const userGroups = useSelector(getUserGroupsData);

  const [selectedBranch, setSelectedBranch] = useState(null);
  const [selectedGroup, setSelectedGroup] = useState(null);
  const [filters, setFilters] = useState({});
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [inputValue, setInputValue] = useState('');
  const [debounceTimeout, setDebounceTimeout] = useState(null);
  const [manageEnabled, setManageEnabled] = useState(true);

  useEffect(() => {
    dispatch(getBranches());
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
        <div className='menu__add'>
          <button
            onClick={() => setIsModalOpen(true)}
            className="menu__add__button text-white"
          >
            <img className='menu__add__icon' src={assets.add_icon} alt="logo" />
            <span>Add Admin</span>
          </button>
        </div>
        {isModalOpen && <AddAdminModal onClose={handleCloseModal} />}

        {/* Filters */}
        <div className="menu__filter flex space-x-4">
          <div className='flex flex-col'>
            {renderSelect("Site", branchOptions, selectedBranch, (opt) => handleSelectChange(opt, 'branchId', setSelectedBranch))}
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

          <div className="manage__page">
            <label className="inline-flex items-center cursor-pointer">
              <input
                type="checkbox"
                className="sr-only"
                checked={manageEnabled}
                onChange={() => setManageEnabled(!manageEnabled)}
              />
              <div className={`w-10 h-6 rounded-full relative transition ${manageEnabled ? 'bg-green-500' : 'bg-gray-400'}`}>
                <div className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transform transition ${manageEnabled ? 'translate-x-4' : ''}`} />
              </div>
            </label>
            <span>Manage</span>
          </div>
        </div>
      </div>

      {/* Mobile Filter */}
      <div className="menu__filter__mobile hidden">
        <div className='flex justify-between'>
          {renderSelect("Site", branchOptions, selectedBranch, (opt) => handleSelectChange(opt, 'branchId', setSelectedBranch))}
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