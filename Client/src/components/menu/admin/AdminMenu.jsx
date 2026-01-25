import { useState, useEffect } from 'react';
import '../menu.css';
import { assets } from '../../../assets/assets';
import CustomSelect from '../../select/CustomSelect';
import { useDispatch, useSelector } from 'react-redux';
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
import { getBranches } from '../../../redux/api/branchApi';
import { getAllAdmins } from '../../../redux/api/adminApi';


function AdminMenu() {
  const dispatch = useDispatch();
  const branches = useSelector(getAllBranchesData);
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [filters, setFilters] = useState({ branchId: null, name: '' });
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [debounceTimeout, setDebounceTimeout] = useState(null);
  const { hasPermission } = useHasPermission();

  useEffect(() => {
    dispatch(getBranches());
  }, [dispatch]);

  useEffect(() => {
    if (debounceTimeout) clearTimeout(debounceTimeout);

    const timeout = setTimeout(() => {
      dispatch(getAllAdmins(filters));
    }, 500);

    setDebounceTimeout(timeout);

    return () => clearTimeout(timeout);
  }, [filters, dispatch]);

 const handleSelectChange = (option) => {
    setSelectedBranch(option);
    setFilters((prev) => ({
      ...prev,
      branchId: option?.value ?? null,
    }));
  };
  const handleSearchChange = (e) => {
    const name = e.target.value;
    setFilters((prev) => ({
      ...prev,
      name,
    }));
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
    ? [{ label: 'All branches', value: null }, ...branches.map(branch => ({ label: branch.name, value: branch.id }))]
    : [{ label: '', value: null }];

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
            {renderSelect("Branch", branchOptions, selectedBranch, handleSelectChange)}
          </div>

          {/* Search */}
          <div className='menu__filter__search'>
            <input
              type="text"
              value={filters.name}
              onChange={handleSearchChange}
              className="w-full rounded"
            />
            <label className="text-white block">Search Admin</label>
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
          {renderSelect("Branch", branchOptions, selectedBranch, handleSelectChange)}
          <div className='menu__filter__search'>
            <input
              type="text"
              value={filters.name}
              onChange={handleSearchChange}
              className="w-full p-2 rounded"
            />
            <label className="text-white block">Search Admin</label>
          </div>
        </div>
      </div>
    </>
  );
}

export default AdminMenu;