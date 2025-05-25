import { useState, useRef, useEffect } from 'react';
import '../menu.css';
import { assets } from '../../../assets/assets';
import { useDispatch, useSelector } from 'react-redux';
import { filterUserByName, getBranches, getLockerGroupsData, getUserGroups, userFilter } from '../../../redux/api/menuApi';
import { getSelectGroupSelect, getBranchesData, getUserGroupsData, setMenuFilter, setUserGroupSelect } from '../../../redux/slice/menuSlice';
import { getAllUsers } from '../../../redux/api/userApi';
import { getAllGroups } from '../../../redux/api/groupApi';
import GeneralAddModal from '../../modals/GeneralAddModal';
import { TbPlugConnected } from "react-icons/tb";
import MediaQuery from 'react-responsive'
import Connection from '../../connection/Connection';
import CustomSelect from '../../select/CustomSelect';
import UserInfoModal from '../../userInfo/UserInfoModal';


function UserMenu() {
    const dispatch = useDispatch();
    const [selectedBranch, setSelectedBranch] = useState(null);
    const [selectedGroups, setSelectedGroups] = useState(null);
    const userGroupEnabled = useSelector(getSelectGroupSelect);
    const [filters, setFilters] = useState({});
    const branches = useSelector(getBranchesData);
    const userGroups = useSelector(getUserGroupsData);
    const [manageEnabled, setManageEnabled] = useState(true);
    const [inputValue, setInputValue] = useState('');
    const [debounceTimeout, setDebounceTimeout] = useState(null);
    const fileInputRef = useRef(null);

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

    useEffect(() => {
        dispatch(getBranches());
        dispatch(getLockerGroupsData());
    }, []);

    const handleFileChange = (event) => {
        const file = event.target.files[0];
        if (file) {
            console.log('Выбран файл:', file.name);
        }
    };

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
                <GeneralAddModal />
                <div className="menu__group flex items-center">
                    <label className="cursor-pointer">
                        <input
                            type="checkbox"
                            className="sr-only"
                            checked={userGroupEnabled}
                            onChange={() => handleUserGroupSelect()}
                        />
                        <div className='menu__group__general'>
                            <div className={`w-10 h-6 bg-gray-400 rounded-full relative transition duration-300 ease-in-out ${userGroupEnabled ? 'bg-green-500' : ''}`}>
                                <div className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transition duration-300 ease-in-out transform ${userGroupEnabled ? 'translate-x-4' : ''}`}></div>
                            </div>
                            <span className="text-white">Group</span>
                        </div>
                    </label>
                </div>

                <div className="menu__filter flex space-x-4">
                    <div className='flex flex-col'>
                        <div className='menu__filter__select'>
                            <CustomSelect
                                options={(Array.isArray(branches) ? branches : []).map(branch => ({
                                    label: branch.name,
                                    value: branch.id,
                                }))}
                                value={selectedBranch}
                                onChange={handleSelectChange}
                            />
                            <label className="text-white block">Site</label>
                        </div>
                        <div className='menu__filter__select'>
                            <CustomSelect
                                options={(Array.isArray(userGroups) ? userGroups : []).map(group => ({
                                    label: group.name,
                                    value: group.id,
                                }))}
                                value={selectedGroups}
                                onChange={handleGroupsSelectChange}
                            />
                            <label className="text-white block">User Group</label>
                        </div>
                    </div>
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

                <div className="flex flex-col items-center space-y-2">
                    <div className="flex space-x-2">
                        <div className="menu__file__uploader">
                            <img src={assets.import_icon} className="cursor-pointer" onClick={handleFileClick} alt="import" />
                            <input
                                type="file"
                                ref={fileInputRef}
                                style={{ display: 'none' }}
                                onChange={handleFileChange}
                            />
                        </div>
                        <div className="menu__file__uploader">
                            <img src={assets.export_icon} className="cursor-pointer" alt="export" />
                        </div>
                    </div>
                    <div>
                        <span className='text-white'>Export/Import</span>
                    </div>
                </div>

                {/* <div className='menu__connection__and__manage'> */}
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
                {/* </div> */}
                {/* <LockerTypes /> */}

            </div>
            <div className="menu__filter__mobile hidden">
                <div className='flex justify-between'>
                    <div className='menu__filter__select'>
                        <CustomSelect
                            options={(Array.isArray(branches) ? branches : []).map(branch => ({
                                label: branch.name,
                                value: branch.id,
                            }))}
                            value={selectedBranch}
                            onChange={handleSelectChange}
                        />
                        <label className="text-white block">Site</label>
                    </div>
                    <div className='menu__filter__select'>
                        <CustomSelect
                            options={(Array.isArray(userGroups) ? userGroups : []).map(group => ({
                                label: group.name,
                                value: group.id,
                            }))}
                            value={selectedGroups}
                            onChange={handleGroupsSelectChange}
                        />
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

export default UserMenu