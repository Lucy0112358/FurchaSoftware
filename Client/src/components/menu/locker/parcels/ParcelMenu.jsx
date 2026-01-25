import React, { useEffect, useState } from 'react';
import LockerModal from '../../../modals/locker/LockerModal';
import { assets } from '../../../../assets/assets';
import MediaQuery from 'react-responsive';
import { TbPlugConnected } from 'react-icons/tb';
import { useDispatch, useSelector } from 'react-redux';
import {
  getBranchesData,
  getLockerGroups,
  getLockerStatusSelect,
  setParcelLockerStatusSelect
} from '../../../../redux/slice/menuSlice';
import LockerStatus from '.././components/lockerStatus/LockerStatus';
import Connection from '../../../connection/Connection';
import {
  getLockerFilter,
  getParcelLockerFilter,
  getParcelLockersData,
  setLockerFilter,
  setParcelLockerFilter,
  setSelectedLockerIds
} from '../../../../redux/slice/lockerSlice';
import { getLockers, getParcelLockers } from '../../../../redux/api/lockerApi';
import CustomSelect from '../../../select/CustomSelect';
import UserInfoModal from '../../../userInfo/UserInfoModal';
import { useHasPermission } from '../../../../hooks/useHasPermission';
import ModalActionButton from '../../../button/ModalActionButton';
import Manage from '../../../manage/Manage';
import { getAllBranchesData } from '../../../../redux/slice/branchSlice';
import ParcelModal from '../../../modals/locker/parcel/ParcelModal';
import { LuMessageSquare } from "react-icons/lu";
import ParcelMessageModal from '../../../modals/locker/parcelMessage/ParcelMessageModal';

function ParcelMenu() {
  const dispatch = useDispatch();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isMessageModalOpen, setIsMessageModalOpen] = useState(false);
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [inputValue, setInputValue] = useState('');
  const [debounceTimeout, setDebounceTimeout] = useState(null);

  const branches = useSelector(getAllBranchesData);
  const lockerGroups = useSelector(getLockerGroups);
  const lockerViewEnabled = useSelector(getLockerStatusSelect);
  const lockerFilters = useSelector(getParcelLockerFilter);
  const { hasPermission } = useHasPermission();
console.log(lockerFilters,111111111);

  const branchOptions = Array.isArray(branches)
    ? branches?.map(branch => ({ label: branch.name, value: branch.id }))
    : [];

  const handleSelectChange = (selectedOption) => {
    setSelectedBranch(selectedOption);
    addFilters(selectedOption?.value, 'branchId');
  };

  const addFilters = (value, key) => {
    const updatedFilters = { ...lockerFilters, [key]: value };
    dispatch(getParcelLockers(updatedFilters));
    dispatch(setParcelLockerFilter(updatedFilters));
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
    dispatch(setSelectedLockerIds([]));
    dispatch(setParcelLockerStatusSelect(!lockerViewEnabled));
  };

  return (
    <>
      <div className="menu flex justify-around">
        {/* Add Locker Group Button */}
        {
          hasPermission() && <ModalActionButton
            onClick={() => setIsModalOpen(true)}
            iconSrc={assets.add_icon}
            text="Store Parcel"
          />
        }
        {isModalOpen && <ParcelModal onClose={() => setIsModalOpen(false)} />}
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
          </div>
        </div>

        {/* Connection & View Toggle */}
        {/* <div className='flex flex-col items-center'>
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
                {
                  // hasPermission(['admin'], ['locker_group_create']) && (
                  <div className='menu__group__general mt-2'>
                    <div className={`w-10 h-6 rounded-full relative transition ${lockerViewEnabled ? 'bg-green-500' : 'bg-gray-400'}`}>
                      <div className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transition transform ${lockerViewEnabled ? 'translate-x-4' : ''}`}></div>
                    </div>
                    <span className="text-white">View</span>
                  </div>
                  // )
                }
              </label>
            </div>
          </div>
        </div> */}
        {
          <div className="flex flex-col items-center cursor-pointer">
            <LuMessageSquare
              className="menu__message__icon"
              onClick={() => setIsMessageModalOpen(true)}
            />
            <p className="text-white">Messages</p>
          </div>
        }
        {isMessageModalOpen && <ParcelMessageModal onClose={() => setIsMessageModalOpen(false)} />}

        {/* User Info & Manage Toggle */}
        <div className="flex items-center text-white flex-col">
          <MediaQuery minWidth={550}>
            <UserInfoModal />
          </MediaQuery>
          <Manage />
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
          <div className='menu__filter__search'>
            <input
              type="text"
              className="w-full p-2 rounded"
            />
            <label className="text-white block">Search User</label>
          </div>
        </div>
      </div>
    </>
  );
}

export default ParcelMenu;