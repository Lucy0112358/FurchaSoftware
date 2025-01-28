import React, { useState } from "react";
import './addUserGroup.css';
import '../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import { getAddUserInfo, setAddUserInfo } from "../../../redux/slice/userSlice";
import 'react-tabs/style/react-tabs.css';
import CustomSelect from "../../select/CustomSelect";
import { getBranchesData, getUserGroupsData } from "../../../redux/slice/menuSlice";
import { setUserGroup } from "../../../redux/api/menuApi";
import { getFilteredLockerGroups } from "../../../redux/slice/lockerSlice";
import NoData from "../../no-data/NoData";
import { getLockerGroupsByBranchId } from "../../../redux/api/branchApi";
import GroupName from "../../headers/GroupName";
import GenerateLocker from "../../lockers/GenerateLocker";
import { toast } from "react-toastify";
import CloseButton from "../attributes/CloseButton";


const AddUserGroupModal = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();
  const branches = useSelector(getBranchesData);
  const [selectedBranch, setSelectedBranch] = useState([]);
  const [sentGeneralInfo, setSentGeneralInfo] = useState({
    branches: [],
    name: '',
  });
  const [selectedBranches, setSelectedBranches] = useState([]);
  const filteredBranchGroups = useSelector(getFilteredLockerGroups)
  const [selectedLockerId, setSelectedLockerId] = useState([]);



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
          if (Array.isArray(value)) {
            return `${key}: ${value.join(', ')}`;
          } else if (typeof value === 'object') {
            const { value: val, label } = value;
            return `${key}: ${label}`;
          }
          return `${key}: ${value}`;
        });
        return `${part}:\n${valuesEntries.join('\n')}`;
      })
      .join('\n\n');
  };

  // const handleSelectBranch = (selectedOption) => {
  //   setSelectedBranch(selectedOption);
  // };
  const handleSelectBranch = (selectedOption) => {
    setSelectedBranches((prevSelected) => {
      const added = selectedOption.filter((item) => !prevSelected.includes(item));
      if (added.length > 0) {
        dispatch(getLockerGroupsByBranchId(added[0].value));
      }
      return selectedOption;
    });
  }

  const addBranchHandle = () => {
    addAllInfoForUserGroup('Branch', 'name', selectedBranch)
    let id = selectedBranch.value
    setSentGeneralInfo((prev) => ({
      ...prev,
      'branches': [...prev.branches, id],
    }))
  };

  const addUserGroup = () => {
    dispatch(setUserGroup(sentGeneralInfo))
      .then((response) => {
        if (response && response.payload.isSuccess) {
          onClose()
        }
      })
      .catch((error) => console.error('Error updating user info:', error));
  }

  const sendGroupInfo = (part, key, value) => {
    addAllInfoForUserGroup(part, key, value);
    setSentGeneralInfo((prev) => ({
      ...prev,
      [key]: value,
    }));
  };

  const sendLockerIds = () => {
    sendGroupInfo('Locker', 'lockerIds', selectedLockerId)
    const branchIds = selectedBranches.map((branch) => branch.value);
    setSentGeneralInfo((prev) => ({
      ...prev,
      branchIds: branchIds,
    }));
    toast.success("Lockers added successfully");
  }

  const handleBranchSelect = (itemId) => {
    setSelectedLockerId((prevSelected) => {
      if (!prevSelected.includes(itemId)) {
        return [...prevSelected, itemId];
      }
      return prevSelected;
    });
  }

  const handleClickBranchSelect = (itemId) => {
    setSelectedLockerId((prevSelected) => {
      if (prevSelected.includes(itemId)) {
        return prevSelected.filter((id) => id !== itemId);
      } else {
        return [...prevSelected, itemId];
      }
    });
  };

  return (
    <div
      className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10"
    >
      {/* Test modal */}

      <div className="add__modal__content add__modal__content__addUser rounded-lg shadow-lg w-full max-w-4xl overflow-auto h-full">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Add User Group</h2>
          <CloseButton onClick={onClose}>
            &times;
          </CloseButton>
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
            {/* <span>Choose branches</span> */}
            {/* <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
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
            </div> */}

            {/* ////////////////////Start */}
            <div className="flex justify-between flex-col">
              <div>
                <span>Choose Branch</span>
                <div className="add__modal__content__part__group grid gap-4 mb-4 mr-2">
                  <div>
                    <CustomSelect options={branches} onChange={handleSelectBranch} multiChoose={true} />
                  </div>
                </div>
              </div>
              <div>
                {filteredBranchGroups?.length ? (
                  <div className='pl-5'>
                    {filteredBranchGroups.map((lockerGroup, groupIndex) => (
                      <React.Fragment key={groupIndex}>
                        <GroupName name={lockerGroup.groupName} />
                        <div className='flex flex-wrap mb-4'
                        >
                          {lockerGroup.groupLockers.map((item, itemIndex) => (
                            <div className={`mr-2 mb-2 select-none ${selectedLockerId.includes(item.id) ? 'selected__branch__id' : ''
                              }`} isDisabled={true} key={itemIndex}
                              onMouseOver={(event) => {
                                if (event.buttons === 1) {
                                  handleBranchSelect(item.id)
                                }
                              }}
                              onClick={() => handleClickBranchSelect(item.id)}
                            >
                              <GenerateLocker item={item} index={itemIndex} />
                            </div>
                          ))}
                        </div>
                      </React.Fragment>
                    ))}
                  </div>
                ) : (
                  <NoData text="No Selected Lockers" />
                )}
              </div>
              <div className="section__add">
                <button
                  onClick={sendLockerIds}
                  className="text-white font-bold rounded p-2">
                  Add locker
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
                value={formatUserRightText()}
                readOnly
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
