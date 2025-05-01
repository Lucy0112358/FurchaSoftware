import React, { useEffect, useRef, useState } from "react";
import './addAdmin.css';
import '../modal.css';
import CustomCheckbox from "../../checkbox/CustomCheckbox";
import { useDispatch, useSelector } from 'react-redux';
import { getAddUserInfo, getFilteredUsers, setAddUserInfo } from "../../../redux/slice/userSlice";
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import 'react-tabs/style/react-tabs.css';
import CustomSelect from "../../select/CustomSelect";
import { getBranchesData, getUserGroupsData } from "../../../redux/slice/menuSlice";
import { filterUserWithOutPaginte, setUserInfo } from "../../../redux/api/userApi";
import { getLockerGroupsByBranchId } from "../../../redux/api/branchApi";
import { getFilteredLockerGroups } from "../../../redux/slice/lockerSlice";
import GroupName from "../../headers/GroupName";
import GenerateLocker from "../../lockers/GenerateLocker";
import NoData from "../../no-data/NoData";
import { toast } from "react-toastify";
import CloseButton from "../attributes/CloseButton";
import { getPermissions, getRoles } from "../../../redux/api/authApi";
import { getPermissionsData, getRolesData } from "../../../redux/slice/authSlice";
import CustomSelectRight from "../../select/CustomSelectRight";
import { setAdminInfo } from "../../../redux/api/adminApi";

const AddAdminModal = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();
  const [formErrors, setFormErrors] = useState({});
  const [userOptions, setUserOptions] = useState([]);
  const [roleOptions, setRoleOptions] = useState([]);
  const branches = useSelector(getBranchesData);
  const filteredBranchGroups = useSelector(getFilteredLockerGroups);
  const [selectedLockerId, setSelectedLockerId] = useState([]);
  const [selectedLockerAddId, setSelectedLockerAddId] = useState({});
  const [userRight, setUserRight] = useState({});
  const [sentGeneralInfo, setSentGeneralInfo] = useState({});
  const [selectedBranch, setSelectedBranch] = useState({});
  const filteredUsers = useSelector(getFilteredUsers);
  const [selectedUser, setSelectedUser] = useState(null);
  const roles = useSelector(getRolesData);
  const [selectRole, setSelectedRole] = useState();
  const permissions = useSelector(getPermissionsData);
  const [selectedPermissions, setSelectedPermissions] = useState({});
  const [userInfo, setUserInfo] = useState({});

  //END change for SelectBranch

  useEffect(() => {
    dispatch(filterUserWithOutPaginte({}));
    dispatch(getRoles());
  }, [dispatch]);

  useEffect(() => {
    const options = filteredUsers?.map((user) => ({
      value: user.id,
      label: user.name,
    }));
    setUserOptions(options);
  }, [filteredUsers]);

  useEffect(() => {
    const options = roles?.map((role) => ({
      value: role.id,
      label: role.name,
    }));
    setRoleOptions(options);
  }, [roles]);

  useEffect(() => {
    const initialPermissions = permissions.reduce((acc, permission) => {
      acc[permission.id] = true;
      return acc;
    }, {});
    if (Object.keys(initialPermissions).length) {
      sendGroupInfo('Permission', 'permissions', Object.keys(initialPermissions)
        .join(', '));
    }
    setSelectedPermissions(initialPermissions);
  }, [permissions]);

  const formatUserRightText = () => {
    const userRightsEntries = Object.entries(userRight);
    return userRightsEntries
      .map(([part, values]) => {
        const valuesEntries = Object.entries(values).map(
          ([key, value]) => `   ${key}: ${value}`
        );
        return `${part}:\n${valuesEntries.join('\n')}`;
      })
      .join('\n');
  };

  const validateForm = () => {
    const errors = {};
    // if (!sentGeneralInfo.name || !sentGeneralInfo.name.trim()) {
    //   errors.name = 'Name is required';
    // }
    // if (!sentGeneralInfo.surname || !sentGeneralInfo.surname.trim()) {
    //   errors.surname = 'Surname is required';
    // }
    // if (!sentGeneralInfo.email || !sentGeneralInfo.email.trim()) {
    //   errors.email = 'Email is required';
    // } else {
    //   const emailRegex = /\S+@\S+\.\S+/;
    //   if (!emailRegex.test(sentGeneralInfo.email)) {
    //     errors.email = 'Invalid email format';
    //   }
    // }
    // if (!sentGeneralInfo.phone || !sentGeneralInfo.phone.trim()) {
    //   errors.phone = 'Phone is required';
    // }
    // if (!sentGeneralInfo.userGroups || !sentGeneralInfo.userGroups.length) {
    //   errors.userGroups = 'At least one User Group must be selected';
    // }
    return errors;
  };

  const addAdminUser = () => {
    // const errors = validateForm();
    // setFormErrors(errors);

    // if (Object.keys(errors).length > 0) {
    //   toast.error('Please fill out the form correctly');
    //   return;
    // }

    const selectedPermissionIds = Object.entries(selectedPermissions)
      .filter(([id, isSelected]) => isSelected)
      .map(([id]) => Number(id));

      const data = {...sentGeneralInfo, permissions: selectedPermissionIds}

    dispatch(setAdminInfo(data))
      .then((response) => {
        if (response && response.payload?.isSuccess) {
          window.location.reload();
          // dispatch(setAddUserInfo({}));
          // setSentGeneralInfo({
          //   user_info: {},
          //   active_period: {},
          //   userGroups: [],
          //   cards: [],
          // });
          onClose();
        } else {
          toast.error(response.error?.message || 'Error occurred');
        }
      })
      .catch((error) => console.error('Error updating user info:', error));
  };

  const handleSelectBranch = (branch) => {
    setSelectedBranch({ [branch.id]: branch.name });
    const existAddedBranchLockers = selectedLockerAddId[branch.id];
    if (existAddedBranchLockers) {
      setSelectedLockerId(existAddedBranchLockers);
    } else {
      setSelectedLockerId([]);
    }
    dispatch(getLockerGroupsByBranchId(branch.id));
  };

  const handleBranchSelectAdd = (itemId) => {
    setSelectedLockerId((prevSelected) => {
      if (!prevSelected.includes(itemId)) {
        return [...prevSelected, itemId];
      }
      return prevSelected;
    });
  };

  const handleBranchSelectRemove = (itemId) => {
    setSelectedLockerId((prevSelected) => {
      if (prevSelected.includes(itemId)) {
        return prevSelected.filter((id) => id !== itemId);
      }
      return prevSelected;
    });
  };

  const handleClickBranchSelect = (itemId) => {
    setSelectedLockerId((prevSelected) => {
      if (prevSelected.includes(itemId)) {
        return prevSelected.filter((id) => id !== itemId);
      } else {
        return [...prevSelected, itemId];
      }
    });
  };

  const sendLockerIds = () => {

    setSelectedLockerAddId((prev) => ({
      ...prev,
      [Object.keys(selectedBranch)]: selectedLockerId,
    }));

    const selectedLockers = { ...selectedLockerAddId, [Object.keys(selectedBranch)]: selectedLockerId };

    const generalInfo = Object.entries(selectedLockers)
      .filter(([branchIdString, lockerIds]) => lockerIds.length > 0)
      .map(([branchIdString, lockerIds]) => {
        const branchId = Number(branchIdString);
        const branch = branches.find(b => b.id === branchId);
        const branchName = branch ? branch.name : "Unknown";
        const lockerIdsString = lockerIds.join(",");
        return `${branchName}: ${lockerIdsString}`;
      })
      .join("; ");

    dispatch(
      setAddUserInfo({
        'Locker': {
          ...userInfo['Locker'],
          ['lockers']: Object.values(selectedLockers).flat(),
        },
      })
    );

    setSentGeneralInfo((prev) => ({
      ...prev,
      'lockerIds': Object.values(selectedLockers).flat(),
    }));

    setUserRight((prev) => ({
      ...prev,
      'Locker': {
        ...(prev['Locker'] || {}),
        'lockers': generalInfo,
      },
    }));

    toast.success("Lockers added successfully");
  };

  const renderError = (fieldName) => {
    if (formErrors[fieldName]) {
      return (
        <div className="text-red-500 text-sm mt-1">
          {formErrors[fieldName]}
        </div>
      );
    }
    return null;
  };

  const sendGroupInfo = (part, key, value) => {
    console.log(part, key, value, 999998888888);
    
    dispatch(
      setAddUserInfo({
        [part]: {
          ...userInfo[part],
          [key]: value,
        },
      })
    );

    setSentGeneralInfo((prev) => ({
      ...prev,
      [key]: value,
    }));

    setUserRight((prev) => ({
      ...prev,
      [part]: {
        ...(prev[part] || {}),
        [key]: value,
      },
    }));
  };

  const handleRoleSelectChange = (selectedOption) => {
    const selectedRoleId = selectedOption.value;
    setSelectedRole(selectedRoleId);
    dispatch(getPermissions(selectedRoleId));
    sendGroupInfo('Role', 'roleId', selectedRoleId);
  }

  const handleCheckboxChange = (id) => {
    setSelectedPermissions((prev) => {
      const updated = {
        ...prev,
        [id]: !prev[id],
      };


      if (Object.keys(updated).length) {
        const trueIds = Object.entries(updated)
          .filter(([_, value]) => value)
          .map(([key]) => key)
          .join(', ');

        sendGroupInfo('Permission', 'permissions', trueIds);
      }

      return updated;
    });
  };

  const handleUserSelectChange = (selectedOption) => {
    const selectedUserId = selectedOption.value;
    setSelectedUser(selectedUserId);
    sendGroupInfo('User', 'userId', selectedUserId);
  };

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__addUser rounded-lg shadow-lg w-full max-w-4xl overflow-auto h-full">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Assign Administrators Rights To The User</h2>
          <CloseButton onClick={onClose}>
            &times;
          </CloseButton>
        </div>

        <Tabs>
          <TabList>
            <Tab>Info</Tab>
            <Tab>Lockers</Tab>
          </TabList>
          <TabPanel>
            <div>
              <div className="flex">
                {/* User Info */}
                <div className="add__modal__content__part w-[70%] mr-1">
                  <span>User</span>
                  <div className="add__modal__content__part__group mb-4 flex justify-between">
                    <div className="w-full mr-2">
                      <CustomSelectRight
                        options={userOptions}
                        value={userOptions.find((option) => option.value === selectedUser)}
                        onChange={(e) => handleUserSelectChange(e)}
                      />
                    </div>
                  </div>
                </div>

                <div className="add__modal__content__part w-[30%]">
                  <span>Choose Role</span>
                  <div className="add__modal__content__part__group mb-4 flex">
                    <div className="add__modal__group__select mr-0 w-full">
                      <CustomSelectRight
                        options={roleOptions}
                        value={roleOptions.find((option) => option.value === selectRole)}
                        onChange={handleRoleSelectChange}
                      />
                    </div>
                  </div>
                </div>
              </div>
              {
                selectRole && permissions?.length > 0 && (<div >
                  <div className="add__modal__content__part  mr-1">
                    <span>Right</span>
                    <div className="add__modal__content__part__group mb-4 flex justify-between">
                      {
                        <div>
                          {permissions.map((permission) => (
                            <div key={permission.id} className="permission-item flex items-center">
                              <CustomCheckbox
                                id={permission.id}
                                checked={selectedPermissions[permission.id]}
                                onChange={() => handleCheckboxChange(permission.id)}
                              />
                              <span className="ml-2 text-white">{permission.name}</span>
                            </div>
                          ))}
                        </div>
                      }
                    </div>
                  </div>
                </div>)
              }

              {/* User Rights */}
              <div className="add__modal__content__part">
                <span>User rights</span>
                <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
                  <label className="block text-gray-300">
                    User rights details
                  </label>
                  <textarea
                    className="w-full p-1 border rounded h-24"
                    readOnly
                    defaultValue={formatUserRightText()}
                  ></textarea>
                </div>
              </div>
            </div>

            <div className="flex justify-end space-x-4">
              <div className="modal__button">
                <button className="bg-gray-600 text-white rounded" onClick={onClose}>
                  Cancel
                </button>
              </div>
              <div className="modal__button">
                <button className="bg-gray-600 text-white rounded" onClick={addAdminUser}>
                  Save
                </button>
              </div>
            </div>
          </TabPanel>

          <TabPanel>
            <div className="flex justify-between flex-col" onContextMenu={(e) => e.preventDefault()}>
              <div className="add__modal__content__part__select w-full">
                <span>Branches</span>
                <div className="add__modal__content__part__group mb-4">
                  <div className="flex mr-2">
                    {
                      branches.length > 0 && (
                        branches.map((branch, index) => {
                          return (
                            branch.name !== 'All' &&
                            <div
                              key={index}
                              onClick={() => handleSelectBranch(branch)}
                              className={`
                                flex 
                                mr-2 
                                bg-[#475456] 
                                p-3 
                                rounded-xl 
                                text-white 
                                cursor-pointer 
                                ${selectedBranch.hasOwnProperty(branch.id)
                                  ? "border-2 border-cyan-500"
                                  : ""
                                }
                              `}>
                              {branch.name}
                            </div>
                          )
                        }
                        ))
                    }
                    {renderError('branch')}
                  </div>
                </div>
              </div>

              <div>
                {selectedBranch && filteredBranchGroups?.length ? (
                  <div className="pl-5 select-none">
                    {filteredBranchGroups.map((lockerGroup, groupIndex) => (
                      <React.Fragment key={groupIndex}>
                        <GroupName name={lockerGroup.groupName} />
                        <div className="flex flex-wrap mb-4">
                          {lockerGroup.groupLockers.map((item, itemIndex) => (
                            <div
                              key={itemIndex}
                              className={`mr-2 mb-2 ${selectedLockerId.includes(item.id)
                                ? 'selected__branch__id'
                                : ''
                                }`}
                              onMouseOver={(event) => {
                                if (event.buttons === 1 && event.ctrlKey) {
                                  handleBranchSelectRemove(item.id);
                                } else if (event.buttons === 1) {
                                  handleBranchSelectAdd(item.id);
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
                  className="text-white font-bold rounded p-2"
                >
                  Add locker
                </button>
              </div>
            </div>
          </TabPanel>
        </Tabs>
      </div>
    </div>
  );
};

export default AddAdminModal;
