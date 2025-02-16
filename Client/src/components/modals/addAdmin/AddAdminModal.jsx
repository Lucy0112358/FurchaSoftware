import React, { useRef, useState } from "react";
import './addAdmin.css';
import '../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import { getAddUserInfo, setAddUserInfo } from "../../../redux/slice/userSlice";
import 'react-tabs/style/react-tabs.css';
import CustomSelect from "../../select/CustomSelect";
import { getBranchesData, getUserGroupsData } from "../../../redux/slice/menuSlice";
import { userFilter } from "../../../redux/api/menuApi";
import { setUserInfo } from "../../../redux/api/userApi";
import { IoMdAdd } from "react-icons/io";
import { getLockerGroupsByBranchId } from "../../../redux/api/branchApi";
import { getFilteredLockerGroups } from "../../../redux/slice/lockerSlice";
import instance from "../../../config/axios/axiosConfig";
import AsyncSelect from 'react-select/async';
import { setAddAdminInfo } from "../../../redux/slice/adminSlice";
import CloseButton from "../attributes/CloseButton";



const AddAdminModal = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();
  const [adminRight, setAdminRight] = useState({});
  const cardRef = useRef(null);
  const [cards, setCards] = useState([]);
  const [debounceTimeout, setDebounceTimeout] = useState(null);
  const [inputValue, setInputValue] = useState('');

  // TODO:  User Group
  const [sentGeneralInfo, setSentGeneralInfo] = useState({
    "isPinRequired": true,
  });

  const sendGroupInfo = (part, key, value) => {
    addAllInfoForAdmin(part, key, value);
    setSentGeneralInfo((prev) => ({
      ...prev,
      [key]: value,
    }));
  };

  // TODO:  User Group
  const [selectedGroups, setSelectedGroups] = useState(null);
  const userGroups = useSelector(getUserGroupsData);

  const handleGroupsSelectChange = (selectedOption) => {
    setSelectedGroups(selectedOption);

    // TODO:
    // addAllInfoForAdminGroup('Group', 'name', selectedBranch) 
    let id = selectedOption.value
    let ids = selectedOption.map(item => item.value);
    setSentGeneralInfo((prev) => ({
      ...prev,
      'adminType': ids,
    }))
    dispatch(userFilter({ 'filterByGroupId': selectedOption.value }));
  };


  const addAllInfoForAdmin = (part, key, value) => {
    const updatedAdminInfo = {
      [part]: {
        ...adminInfo[part],
        [key]: value,
      },
    };
    dispatch(setAddAdminInfo(updatedAdminInfo));

    setAdminRight((prev) => ({
      ...prev,
      ...updatedAdminInfo,
    }));
  };

  const handleFilterName = (e) => {
    let name = e.target.value;
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

  const formatUserRightText = () => {
    const userRightsEntries = Object.entries(adminRight);
    return userRightsEntries
      .map(([part, values]) => {
        const valuesEntries = Object.entries(values).map(
          ([key, value]) => `   ${key}: ${value}`
        );
        return `${part}:\n${valuesEntries.join('\n')}`;
      })
      .join('\n');
  };

  // TODO: feat
  const addUser = () => {
    dispatch(setUserInfo(sentGeneralInfo))
      .then((response) => {
        if (response && response.payload.isSuccess) {
          onClose();
        }
      })
      .catch((error) => console.error('Error updating user info:', error));
  }


  // Card part
  const setCardNumbers = () => {
    const newCard = cardRef.current.value.trim();

    if (newCard) {
      setCards((prevCards) => [...prevCards, newCard]);
      cardRef.current.value = null;
      const addNewCardToCards = [...cards, newCard];
      setSentGeneralInfo((prev) => ({
        ...prev,
        'cards': prev.cards ? addNewCardToCards : [newCard],
      }))
      sendGroupInfo('Card No', 'cards', addNewCardToCards)

    }
  }

  const handleDeleteCards = (value) => {
    const withoutDeletedCards = cards.filter((card) => card !== value);
    setCards(withoutDeletedCards);
    setSentGeneralInfo((prev) => ({
      ...prev,
      'cards': withoutDeletedCards,
    }))
    sendGroupInfo('Card No', 'cards', withoutDeletedCards)
  }

  const handleSelectBranch = (selectedOption) => {
    dispatch(getLockerGroupsByBranchId(selectedOption.value));

  }

  //End Card part

  //Add Group Part
  const addGroup = () => {

  }

  const [selectedUser, setSelectedUser] = useState(null);
  const [userOptions, setUserOptions] = useState([]);

  const handleUserInputChange = (newValue, { action }) => {
    if (action === 'input-change') {
      setInputValue(newValue);
    }
    return newValue;
  };

  const handleUserChange = (selected) => {
    setSelectedUser(selected);
    setInputValue('');
  };

  const loadUserOptions = (inputValue) => {

    return instance
      .get(`/User/search-user/?adminId=8&name=${inputValue}`)
      .then((response) => {
        return response.data.data.map((item) => ({
          value: item.id,
          label: item.name,
        }));
      });
  };

  return (
    <div
      className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10"
    >
      <div className="add__modal__content add__modal__content__addUser rounded-lg shadow-lg w-full max-w-4xl overflow-auto h-full">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Assign Administrators Rights To The User</h2>
          <CloseButton onClick={onClose}>
            &times;
          </CloseButton>
        </div>
        <div className="flex">
          {/* User Info */}
          <div className="add__modal__content__part w-[70%] mr-1">
            <span>User</span>
            <div className="add__modal__content__part__group mb-4 flex">
              <AsyncSelect
                className="w-full mr-2"
                cacheOptions
                loadOptions={loadUserOptions}
                isSearchable={true}
                inputValue={inputValue}
                onInputChange={handleUserInputChange}
                onChange={handleUserChange}
                closeMenuOnSelect={false}
              />

              <div className="flex w-1/6">
                <button
                  // onClick={() => setAddBranchModalSwitch(!addBranchModalSwitch)}
                  className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center h-[43px]"
                >
                  <IoMdAdd className="fill-current" style={{ fontSize: 'xx-large' }} />
                </button>
              </div>
            </div>
          </div>

          <div className="add__modal__content__part w-[30%]">
            <span>Admin Type</span>
            <div className="add__modal__content__part__group mb-4 flex">
              <div className="add__modal__group__select mr-0 w-full">
                <CustomSelect options={userGroups} onChange={handleGroupsSelectChange} multiChoose={true} />
              </div>
            </div>
          </div>
        </div>
        <div className="add__modal__content__part">
          <span>Admin rights</span>
          <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
            <label className="block text-gray-300">Admin rights details</label>
            <textarea
              className="w-full p-1 border rounded h-24"
              // value={adminRight}
              readOnly
              defaultValue={formatUserRightText()}
            ></textarea>
          </div>
        </div>
        {/* Buttons */}
        <div className="flex justify-end space-x-4">
          <div className="modal__button">
            <button className="bg-gray-600 text-white rounded "
              onClick={onClose}>
              Cancel
            </button>
          </div>
          <div className="modal__button">
            <button
              className="bg-gray-600 text-white rounded "
              onClick={addUser}
            >
              Save
            </button>
          </div>
        </div>
      </div>
    </div >
  );
};

export default AddAdminModal;
