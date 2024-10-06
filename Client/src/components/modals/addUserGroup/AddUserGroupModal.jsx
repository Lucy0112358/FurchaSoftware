import React, { useState } from "react";
import './addUserGroup.css';
import '../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import { getAddUserInfo, setAddUserInfo } from "../../../redux/slice/userSlice";
import 'react-tabs/style/react-tabs.css';
import CustomSelect from "../../select/CustomSelect";
import { getUserBranchesData, getUserGroupsData } from "../../../redux/slice/menuSlice";
import { setUserGroup } from "../../../redux/api/menuApi";


const AddUserGroupModal = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();
  const userBranches = useSelector(getUserBranchesData);
  const [selectedBranch, setSelectedBranch] = useState([]);
  const [sentGeneralInfo, setSentGeneralInfo] = useState({
    branches: [],
    name: '',
  });

  console.log(sentGeneralInfo, 111111111111)

  const userInfo = useSelector(getAddUserInfo);
  const [groupRight, setGroupRight] = useState({});

  const addAllInfoForUserGroup = (part, key, value) => {
    const updatedUserInfo = {
      [part]: {
        ...userInfo[part],
        [key]: value,
      },
    };
    dispatch(setAddUserInfo(updatedUserInfo));

    setGroupRight((prev) => ({
      ...prev,
      ...updatedUserInfo,
    }));
  };

  const formatUserRightText = () => {
    const userRightsEntries = Object.entries(groupRight);
    return userRightsEntries
      .map(([part, values]) => {
        const valuesEntries = Object.entries(values).map(([key, value]) => {
          console.log(values , key, "valuevaluevalue")
          if (Array.isArray(value)) {
            return `${key}: ${value.join(', ')}`;
          } else if (typeof value === 'object') {
            const { value: val, label } = value;
            return `${key}: ${label}`;
          }
          return `${key}: ${value}`;
        });
        console.log(valuesEntries, part, values, "valuesEntries")
        return `${part}:\n${valuesEntries.join('\n')}`;
      })
      .join('\n\n');
  };

  const handleSelectChange = (selectedOption) => {
    setSelectedBranch(selectedOption);
    
    console.log("Выбранная опция:", selectedOption);
  };

  const addBranchHandle = () => {
    addAllInfoForUserGroup('Branch', 'name', selectedBranch) 
    let id = selectedBranch.value
    setSentGeneralInfo((prev) => ({
      ...prev,
      'branches' : [...prev.branches, id],
    }))
  };

  const addUserGroup = () => {
    dispatch(setUserGroup(sentGeneralInfo))
    console.log(sentGeneralInfo,888)
  };  

  const sendGroupInfo = (part, key, value) => {
    addAllInfoForUserGroup(part, key, value);
    setSentGeneralInfo((prev) => ({
      ...prev,
      [key]: value,
    }));
  };

  return (
    <div
      className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10"
    >
      {/* Test modal */}

      <div className="add__modal__content add__modal__content__addUser rounded-lg shadow-lg w-full max-w-4xl overflow-auto h-full">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Add User Group</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-200 text-xl"
          >
            &times;
          </button>
        </div>
        <div>
          {/* User Group Info */}
          <div className="add__modal__content__part">
            <span>User Group Info</span>
            <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
              <div>
                <input
                  placeholder="Group Name"
                  type="text"
                  onChange={(e) => sendGroupInfo('Group', 'name', e.target.value)}
                  className="w-full p-1 border rounded"
                />
              </div>
            </div>
          </div>

          <div className="add__modal__content__part">
            <span>Choose branches</span>
            <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
              <div>
                <CustomSelect options={userBranches} onChange={handleSelectChange} />
              </div>
              <div className="section__add">
                <button 
                  className="text-white font-bold rounded"
                  onClick={() => addBranchHandle()}
                >
                  Add
                </button>
              </div>
            </div>
          </div>

          {/* User Rights */}
          <div className="add__modal__content__part">
            <span>Group rights</span>
            <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
              <label className="block text-gray-300">Group rights details</label>
              <textarea
                className="w-full p-1 border rounded h-24"
                // value={groupRight}
                value={formatUserRightText()}
              ></textarea>
            </div>
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
              className="bg-gray-600 text-white rounded"
              onClick={addUserGroup}
            >
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AddUserGroupModal;
