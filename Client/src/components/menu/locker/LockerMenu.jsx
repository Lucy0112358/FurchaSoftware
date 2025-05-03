import React, { useEffect, useState } from 'react';
import LockerModal from '../../modals/locker/LockerModal';
import { assets } from '../../../assets/assets';
import MediaQuery from 'react-responsive';
import { TbPlugConnected } from 'react-icons/tb';
import { useDispatch, useSelector } from 'react-redux';
import {
  getBranchesData,
  getLockerGroups,
  getLockerStatusSelect,
  setLockerStatusSelect
} from '../../../redux/slice/menuSlice';
import LockerStatus from './components/lockerStatus/LockerStatus';
import LockerTypes from './components/lockerTypes/LockerTypes';
import Connection from '../../connection/Connection';
import {
  getLockerFilter,
  setLockerFilter
} from '../../../redux/slice/lockerSlice';
import { getLockers } from '../../../redux/api/lockerApi';
import CustomSelect from '../../select/CustomSelect';
import UserInfoModal from '../../userInfo/UserInfoModal';

function LockerMenu() {
  const dispatch = useDispatch();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [manageEnabled, setManageEnabled] = useState(true);
  const [selectedGroups, setSelectedGroups] = useState(null);
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [inputValue, setInputValue] = useState('');
  const [debounceTimeout, setDebounceTimeout] = useState(null);

  const branches = useSelector(getBranchesData);
  const lockerGroups = useSelector(getLockerGroups);
  const lockerViewEnabled = useSelector(getLockerStatusSelect);
  const lockerFilters = useSelector(getLockerFilter);

  const branchOptions = Array.isArray(branches)
    ? branches.map(branch => ({ label: branch.name, value: branch.id }))
    : [];

  const groupOptions = Array.isArray(lockerGroups)
    ? lockerGroups.map(group => ({ label: group.name, value: group.id }))
    : [];

  const handleSelectChange = (selectedOption) => {
    setSelectedBranch(selectedOption);
    addFilters(selectedOption?.value, 'branchId');
  };

  const handleGroupsSelectChange = (selectedOption) => {
    setSelectedGroups(selectedOption);
    addFilters(selectedOption?.value, 'groupId');
  };

  const addFilters = (value, key) => {
    const updatedFilters = { ...lockerFilters, [key]: value };
    dispatch(getLockers(updatedFilters));
    dispatch(setLockerFilter(updatedFilters));
  };

  const handleFilterName = (e) => {
    const name = e.target.value;
    setInputValue(name);

    if (debounceTimeout) clearTimeout(debounceTimeout);

    const timeout = setTimeout(() => {
      addFilters(name, 'name');
    }, 1000);

    setDebounceTimeout(timeout);
  };

  const handleLockerStatusSelect = () => {
    dispatch(setLockerStatusSelect(!lockerViewEnabled));
  };

  return (
    <>
      <div className="menu flex justify-around">
        {/* Add Locker Group Button */}
        <div className='menu__add'>
          <button
            onClick={() => setIsModalOpen(true)}
            className="menu__add__button text-white"
          >
            <img className='menu__add__icon' src={assets.add_icon} alt="logo" />
            <span>New Locker Group</span>
          </button>
        </div>
        {isModalOpen && <LockerModal onClose={() => setIsModalOpen(false)} />}
        {/* Filters */}
        <div className="menu__filter flex space-x-4">
          <div className='flex flex-col'>
            <div className='menu__filter__select'>
              <CustomSelect
                options={branchOptions}
                value={selectedBranch}
                onChange={handleSelectChange}
              />
              <label className="text-white block">Branch</label>
            </div>
            <div className='menu__filter__select'>
              <CustomSelect
                options={groupOptions}
                value={selectedGroups}
                onChange={handleGroupsSelectChange}
              />
              <label className="text-white block">Locker Group</label>
            </div>
          </div>
          <div className='flex flex-col'>
            <div className='menu__filter__search'>
              <input
                type="text"
                value={inputValue}
                onChange={handleFilterName}
                className="w-full rounded"
              />
              <label className="text-white block">Search User</label>
            </div>
            <div>
              <LockerStatus addFilters={addFilters} />
            </div>
          </div>
        </div>

        {/* Responsive Locker Status */}
        <MediaQuery maxWidth={1280}>
          <div>
            <LockerStatus addFilters={addFilters} />
          </div>
        </MediaQuery>

        {/* Connection & View Toggle */}
        <div className='flex flex-col items-center'>
          <div className="menu__connection flex items-start text-white">
            <MediaQuery minWidth={769}>
              <Connection />
            </MediaQuery>
            <MediaQuery maxWidth={768}>
              <TbPlugConnected className='menu__connection__status__icon' />
            </MediaQuery>
          </div>
          <div>
            <div className="menu__group flex items-center">
              <label className="cursor-pointer">
                <input
                  type="checkbox"
                  className="sr-only"
                  checked={lockerViewEnabled}
                  onChange={handleLockerStatusSelect}
                />
                <div className='menu__group__general mt-2'>
                  <div className={`w-10 h-6 rounded-full relative transition ${lockerViewEnabled ? 'bg-green-500' : 'bg-gray-400'}`}>
                    <div className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transition transform ${lockerViewEnabled ? 'translate-x-4' : ''}`}></div>
                  </div>
                  <span className="text-white">View</span>
                </div>
              </label>
            </div>
          </div>
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
                <div className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transition transform ${manageEnabled ? 'translate-x-4' : ''}`}></div>
              </div>
            </label>
            <span>Manage</span>
          </div>
        </div>
      </div>

      {/* Mobile Filters */}
      <div className="menu__filter__mobile hidden">
        <div className='flex justify-between'>
          <div className='menu__filter__select'>
            <CustomSelect
              options={branchOptions}
              value={selectedBranch}
              onChange={handleSelectChange}
            />
            <label className="text-white block">Branch</label>
          </div>
          <div className='menu__filter__select'>
            <CustomSelect
              options={groupOptions}
              value={selectedGroups}
              onChange={handleGroupsSelectChange}
            />
            <label className="text-white block">Locker Group</label>
          </div>
          <div className='menu__filter__search'>
            <input
              type="text"
              className="w-full p-2 rounded"
            />
            <label className="text-white block">Search User</label>
          </div>
        </div>
      </div>

      <div className='flex justify-end w-full'>
        <LockerTypes addFilters={addFilters} />
      </div>
    </>
  );
}

export default LockerMenu;