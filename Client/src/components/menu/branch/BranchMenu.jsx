import { useState, useRef, useEffect } from 'react';
import '../menu.css';
import { RiAddBoxLine } from "react-icons/ri";
import { assets } from '../../../assets/assets';
import CustomSelect from '../../select/CustomSelect';
import { useDispatch, useSelector } from 'react-redux';
import { filterUserByName, getBranches, getUserGroups, userFilter } from '../../../redux/api/menuApi';
import { getSelectGroupSelect, getBranchesData, getUserGroupsData, setMenuFilter, setUserGroupSelect } from '../../../redux/slice/menuSlice';
import AddUserModal from '../../modals/addUser/AddUserModal';
import { getAllUsers } from '../../../redux/api/userApi';
import { getAllGroups } from '../../../redux/api/groupApi';
import GeneralAddModal from '../../modals/GeneralAddModal';
import { TbPlugConnected } from "react-icons/tb";
import MediaQuery from 'react-responsive'
import LockerTypes from '../locker/components/lockerTypes/LockerTypes';
import Connection from '../../connection/Connection';
import ModulesModal from '../../modals/modules/ModulesModal';
import BranchModal from '../../modals/branch/BranchModal';
import { getAllBranches } from '../../../redux/api/branchApi';
import UserInfoModal from '../../userInfo/UserInfoModal';


function BranchMenu() {
    const dispatch = useDispatch();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [inputValue, setInputValue] = useState('');
    const [debounceTimeout, setDebounceTimeout] = useState(null);
    const [manageEnabled, setManageEnabled] = useState(true);

    const handleFilterName = (e) => {
        let name = e.target.value;
        setInputValue(name);

        if (debounceTimeout) {
            clearTimeout(debounceTimeout);
        }

        setDebounceTimeout(
            setTimeout(() => {
                dispatch(getAllBranches({ 'name': name }));
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
                            {"Add Branch"}
                        </span>
                    </button>
                </div>
                {isModalOpen && <BranchModal onClose={() => setIsModalOpen(false)} />}
                <div className="menu__filter flex space-x-4">
                    <div className='menu__filter__search'>
                        <input
                            type="text"
                            value={inputValue}
                            onChange={handleFilterName}
                            className="w-full rounded"
                        />
                        <label className="text-white block">Search</label>
                    </div>
                </div>

                <div className="menu__connection flex items-start text-white">
                    <MediaQuery minWidth={769}>
                        <Connection />
                    </MediaQuery>
                    <MediaQuery maxWidth={768}>
                        <TbPlugConnected className='menu__connection__status__icon' />
                    </MediaQuery>
                </div>

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

export default BranchMenu