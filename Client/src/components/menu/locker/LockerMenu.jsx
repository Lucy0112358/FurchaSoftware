import React, { useEffect, useState } from 'react'
import LockerModal from '../../modals/locker/LockerModal'
import { assets } from '../../../assets/assets';
import MediaQuery from 'react-responsive'
import { TbPlugConnected } from 'react-icons/tb';
import CustomSelect from '../../select/CustomSelect';
import { useDispatch, useSelector } from 'react-redux';
import { getBranchesData, getLockerGroups, getLockerStatusSelect, setLockerStatusSelect } from '../../../redux/slice/menuSlice';
import LockerStatus from './components/lockerStatus/LockerStatus';
import LockerTypes from './components/lockerTypes/LockerTypes';
import Connection from '../../connection/Connection';
import { getLockerFilter, setLockerFilter } from '../../../redux/slice/lockerSlice';
import { getLockers } from '../../../redux/api/lockerApi';
import { getLockerGroupsData } from '../../../redux/api/menuApi';
import { IoMdAdd } from 'react-icons/io';

function LockerMenu() {
    const dispatch = useDispatch();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [manageEnabled, setManageEnabled] = useState(true);
    const [selectedGroups, setSelectedGroups] = useState(null);
    const branches = useSelector(getBranchesData);
    const lockerViewEnabled = useSelector(getLockerStatusSelect);
    const lockerGroups = useSelector(getLockerGroups)
    const lockerFilters = useSelector(getLockerFilter)
    const [inputValue, setInputValue] = useState('');
    const [debounceTimeout, setDebounceTimeout] = useState(null);

    // useEffect(() => {
    //     dispatch(getLockerGroupsData());
    // }, []);

    const handleSelectChange = (selectedOption) => {
        setSelectedBranch(selectedOption);
        addFilters(selectedOption.value, 'branchId')
    };

    const [selectedBranch, setSelectedBranch] = useState(null);

    const handleLockerStatusSelect = () => {
        dispatch(setLockerStatusSelect(!lockerViewEnabled));
        if (lockerViewEnabled) {
            // dispatch(getAllUsers())
        } else {
            // dispatch(getAllGroups())
        }
    };

    const handleGroupsSelectChange = (selectedOption) => {
        setSelectedGroups(selectedOption);
        // addFilters(selectedOption, 'groupId')
        addFilters(selectedOption, 'groupId')

        // dispatch(userFilter({ 'filterByGroupId': selectedOption.value }));

    };

    const addFilters = (selectedOption, key) => {
        let filters = { ...lockerFilters, [key]: selectedOption }
        console.log(filters, 999);

        dispatch(getLockers(filters))
        dispatch(setLockerFilter(filters))
    }
    const handleFilterName = (e) => {
        let name = e.target.value;
        setInputValue(name);
        if (debounceTimeout) {
            clearTimeout(debounceTimeout);
        }

        setDebounceTimeout(
            setTimeout(() => {
                addFilters(name, 'name');
            }, 1000)
        );
    };

    return (
        <>
            <div className="menu flex justify-around">
                <div className='menu__add'>
                    <button
                        onClick={() => setIsModalOpen(true)}
                        className="menu__add__button text-white">
                        <img className='menu__add__icon' src={assets.add_icon} alt="logo" />
                        <span className="">
                            {"New Locker Group"}
                        </span>
                    </button>
                </div>
                <LockerModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
                <div className="menu__filter flex space-x-4">
                    <div className='flex flex-col'>
                        <div className='menu__filter__select'>
                            <CustomSelect options={branches} onChange={handleSelectChange} />
                            <label className="text-white block">Branch</label>
                        </div>
                        <div className='menu__filter__select'>
                            <CustomSelect options={lockerGroups} onChange={handleGroupsSelectChange} />
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
                                    onChange={() => handleLockerStatusSelect()}
                                />
                                <div className='menu__group__general mt-2'>
                                    <div className={`w-10 h-6 bg-gray-400 rounded-full relative transition duration-300 ease-in-out ${lockerViewEnabled ? 'bg-green-500' : ''}`}>
                                        <div className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transition duration-300 ease-in-out transform ${lockerViewEnabled ? 'translate-x-4' : ''}`}></div>
                                    </div>
                                    <span className="text-white">View</span>
                                </div>
                            </label>
                        </div>
                    </div>
                </div>

                <div className="flex items-center text-white flex-col">
                    <MediaQuery minWidth={550}>
                        <div className="flex flex-col items-end">
                            <span>Michael</span>
                            <span>(Administrator)</span>
                        </div>
                    </MediaQuery>

                    <div className="manage__page">
                        <label className="inline-flex items-center cursor-pointer">
                            <input
                                type="checkbox"
                                className="sr-only"
                                checked={manageEnabled}
                                onChange={() => setManageEnabled(!manageEnabled)}
                            />
                            <div className={`w-10 h-6 bg-gray-400 rounded-full relative transition duration-300 ease-in-out ${manageEnabled ? 'bg-green-500' : ''}`}>
                                <div className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transition duration-300 ease-in-out transform ${manageEnabled ? 'translate-x-4' : ''}`}></div>
                            </div>
                        </label>
                        <span>Manage</span>
                    </div>
                </div>
            </div>

            <div className="menu__filter__mobile hidden">
                <div className='flex justify-between'>
                    <div className='menu__filter__select'>
                        <CustomSelect options={branches} onChange={handleSelectChange} />
                        <label className="text-white block">Branch</label>
                    </div>
                    <div className='menu__filter__select'>
                        <CustomSelect options={lockerGroups} onChange={handleGroupsSelectChange} />
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
    )
}

export default LockerMenu