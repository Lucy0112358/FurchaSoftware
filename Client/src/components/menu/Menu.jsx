import { useState, useRef, useEffect } from 'react';
import './menu.css';
import { RiAddBoxLine } from "react-icons/ri";
import { assets } from '../../assets/assets';
import CustomSelect from '../select/CustomSelect';
import { useLocation } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { filterUserByName, getUserBranches, getUserGroups, userFilter } from '../../redux/api/menuApi';
import { getSelectGroupSelect, getUserBranchesData, getUserGroupsData, setUserGroupSelect } from '../../redux/slice/menuSlice';
import AddUserModal from '../modals/addUser/AddUserModal';
import { getAllUsers } from '../../redux/api/userApi';
import { getAllGroups } from '../../redux/api/groupApi';
import GeneralAddModal from '../modals/GeneralAddModal';


function Menu() {
  const location = useLocation();
  const dispatch = useDispatch();
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [selectedGroups, setSelectedGroups] = useState(null);
  const userGroupEnabled = useSelector(getSelectGroupSelect)

  //input search by nane
  const [inputValue, setInputValue] = useState('');
  const [debounceTimeout, setDebounceTimeout] = useState(null);

  const handleSelectChange = (selectedOption) => {
    setSelectedBranch(selectedOption);

    dispatch(userFilter({'filterByBranchId': selectedOption.value}));
    console.log("Выбранная опция:", selectedOption);
  };

  const handleGroupsSelectChange = (selectedOption) => {
    setSelectedGroups(selectedOption);
    dispatch(userFilter({'filterByGroupId': selectedOption.value}));

    console.log("Выбранная groups:", selectedOption);
  };

  //Add USER modal part 
  // const [isModalOpen, setIsModalOpen] = useState(false);

  const userBranches = useSelector(getUserBranchesData);
  const userGroups = useSelector(getUserGroupsData);
  const [manageEnabled, setManageEnabled] = useState(true);

  useEffect(() => {
    console.log(location.pathname);
    let path = location.pathname;
    console.log(path);
    if (path == '/' || path == '/users') {
       dispatch(getUserGroups());
    } else if (path == '/modules') {
      
    }
  }, [location]);

  useEffect(() => {
    dispatch(getUserBranches());
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

  const handleInputChange = (e) => {
    let name = e.target.value;
    setInputValue(name);

    if (debounceTimeout) {
      clearTimeout(debounceTimeout);
    }

    setDebounceTimeout(
      setTimeout(() => {
        dispatch(filterUserByName({'name': name}));
      }, 1000)
    );
  };

  const handleUserGroupSelect = () => {
    dispatch(setUserGroupSelect(!userGroupEnabled));
    if(userGroupEnabled){
      dispatch(getAllUsers())
    } else {
      dispatch(getAllGroups())
    }
  };

  return (
    <div >
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
              <CustomSelect options={userBranches}  onChange={handleSelectChange} />
              <label className="text-white block">Site</label>
            </div>
            <div className='menu__filter__select'>
              <CustomSelect options={userGroups}  onChange={handleGroupsSelectChange} />
              <label className="text-white block">User Group</label>
            </div>
          </div>
          <div className='menu__filter__search'>
            <input
              type="text"
              value={inputValue}
              onChange={handleInputChange}
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

        <div className="menu__connection flex items-start text-white">
          <span className="menu__connection__status"></span>
          <span>Connection</span>
        </div>

        <div className="flex items-center text-white flex-col">
          <div className="flex flex-col items-end">
            <span>Michael</span>
            <span>(Administrator)</span>
          </div>
          <div className="flex items-center ml-4 flex-col mt-3">
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
    </div>
  );
}

export default Menu