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


function ModulesMenu() {
    const dispatch = useDispatch();
    const [selectedBranch, setSelectedBranch] = useState(null);
    const [selectedGroups, setSelectedGroups] = useState(null);
    const userGroupEnabled = useSelector(getSelectGroupSelect);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [filters, setFilters] = useState({});

    console.log(filters, "filtersfilters")

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

        // console.log("Выбранная groups:", selectedOption);
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

    const userBranches = useSelector(getBranchesData);
    const userGroups = useSelector(getUserGroupsData);
    const [manageEnabled, setManageEnabled] = useState(true);

    useEffect(() => {
        dispatch(getBranches());
    }, []);

    const handleFileChange = (event) => {
        const file = event.target.files[0];
        if (file) {
            console.log('Выбран файл:', file.name);
        }
    };

    const fileInputRef = useRef(null);

    const handleFileClick = () => {
        fileInputRef.current.click();
    };

    const handleFilterName = (e) => {
        let name = e.target.value;
        setFilters({})
        setInputValue(name);

        if (debounceTimeout) {
            clearTimeout(debounceTimeout);
        }

        setDebounceTimeout(
            setTimeout(() => {
                dispatch(filterUserByName({ 'name': name }));
            }, 1000)
        );
    };

    const handleUserGroupSelect = () => {
        dispatch(setUserGroupSelect(!userGroupEnabled));
        if (userGroupEnabled) {
            dispatch(getAllUsers())
        } else {
            dispatch(getAllGroups())
        }
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
                                {"Add Modules"}
                            </span>
                        </button>
                    </div>
                    <ModulesModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
                <div className="menu__filter flex space-x-4">
                    <div className='flex flex-col'>
                        <div className='menu__filter__select'>
                            <CustomSelect options={userBranches} onChange={handleSelectChange} />
                            <label className="text-white block">Branch</label>
                        </div>
                        <div className='menu__filter__select'>
                            <CustomSelect options={userGroups} onChange={handleGroupsSelectChange} />
                            <label className="text-white block">Locker Group</label>
                        </div>
                    </div>
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
                {/* </div> */}
                {/* <LockerTypes /> */}

            </div>
            <div className="menu__filter__mobile hidden">
                <div className='flex justify-between'>
                    <div className='menu__filter__select'>
                        <CustomSelect />
                        <label className="text-white block">Site</label>
                    </div>
                    <div className='menu__filter__select'>
                        <CustomSelect />
                        <label className="text-white block">User Group</label>
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

export default ModulesMenu