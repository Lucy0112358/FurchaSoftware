import React, { useEffect, useRef, useState } from "react";
import './addAdmin.css';
import '../modal.css';
import CustomCheckbox from "../../checkbox/CustomCheckbox";
import { useDispatch, useSelector } from 'react-redux';
import { getAllUsersData, setAddUserInfo } from "../../../redux/slice/userSlice";
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import 'react-tabs/style/react-tabs.css';
import { getBranchesData, setLockerGroupWithFilters } from "../../../redux/slice/menuSlice";
import {  getUsers } from "../../../redux/api/userApi";
import { getFilteredLockerGroups } from "../../../redux/slice/menuSlice";
import NoData from "../../no-data/NoData";
import { toast } from "react-toastify";
import CloseButton from "../attributes/CloseButton";
import { getPermissions, getRoles } from "../../../redux/api/authApi";
import { getPermissionsData, getRolesData } from "../../../redux/slice/authSlice";
import CustomSelect from "../../select/CustomSelect";
import { getAllAdmins, setAdminInfo, updateAdminInfo } from "../../../redux/api/adminApi";
import ShowFormikError from "../../error/ShowFormikError";
import { fromCamelCasePretty } from "../../../Utils";
import { getLockerGroupsData } from "../../../redux/api/menuApi";

const AddAdminModal = ({ onClose, mode = "add", initialData = {} }) => {
  const dispatch = useDispatch();
  const [formErrors, setFormErrors] = useState({});
  const [userOptions, setUserOptions] = useState([]);
  const [roleOptions, setRoleOptions] = useState([]);
  const branches = useSelector(getBranchesData);
  const [selectedLockerGroupId, setSelectedLockerGroupId] = useState([]);
  const [selectedLockerGroupAddId, setSelectedLockerGroupAddId] = useState({});
  const [userRight, setUserRight] = useState({});
  const [sentGeneralInfo, setSentGeneralInfo] = useState({});
  const [selectedBranch, setSelectedBranch] = useState({});
  const usersData = useSelector(getAllUsersData);

  const [selectedUser, setSelectedUser] = useState(null);
  const roles = useSelector(getRolesData);
  const [selectRole, setSelectedRole] = useState(null);
  const permissions = useSelector(getPermissionsData);
  const [selectedPermissions, setSelectedPermissions] = useState({});
  const [userInfo, setUserInfo] = useState({});
  const filteredLockerGroups = useSelector(getFilteredLockerGroups);

  //END change for SelectBranch

  useEffect(() => {
    dispatch(getUsers({ isAdmin: false }));
    dispatch(getRoles());
    dispatch(getLockerGroupsData());
  }, [dispatch]);

  useEffect(() => {
    const options = usersData?.map((user) => ({
      value: user.id,
      label: user.name,
    }));
    setUserOptions(options);
  }, [usersData]);

  useEffect(() => {
    const options = roles?.map((role) => ({
      value: role.id,
      label: role.name,
    }));
    setRoleOptions(options);
  }, [roles]);

  useEffect(() => {
    if (initialData) {
      let roleId = initialData.roleId;
      if (roleId) {
        setSelectedRole(roleId);
        dispatch(getPermissions(roleId));
      }
      let existingGroupIds = initialData.branches?.flatMap(branch => branch.groupIds);
      setSelectedLockerGroupId(existingGroupIds);
      const lockerGroupMap = Object.fromEntries(
        initialData.branches?.map(branch => [branch.branchId, branch.groupIds]) || []
      );

      setSelectedLockerGroupAddId(lockerGroupMap);

      setSelectedPermissions(prev =>
        Object.fromEntries(
          Object.keys(prev).map(key => [
            key,
            initialData.permissionIds?.includes(Number(key))
          ])
        )
      );

      setSentGeneralInfo({
        roleId: initialData.roleId,
        id: initialData.id,
        groupIds: existingGroupIds || [],
      });

      const selectedLockerGroups = { ...lockerGroupMap, [Object.keys(selectedBranch)]: existingGroupIds };

      const generalInfo = Array.isArray(initialData?.branches)
        ? initialData.branches
          .filter((branch) => branch.groupIds?.length > 0)
          .map((branch) => {
            const branchName = branch?.branchName || "Unknown";
            const groupIdsString = branch.groupIds.join(",");
            return `${branchName}: ${groupIdsString}`;
          })
          .join("; ")
        : "";

      setUserRight((prev) => ({
        ...prev,
        'Groups': {
          ...(prev['Groups'] || {}),
          'groups': generalInfo,
        },
      }));

    }
  }, [initialData.roleId]);

  useEffect(() => {
    const allPermissions = permissions.flatMap(type => type.permissions);
    const initialPermissions = allPermissions.reduce((acc, permission) => {
      acc[permission.id] = true;
      return acc;
    }, {});

    // if (Object.keys(initialPermissions).length) {
    //   sendGroupInfo(
    //     'Permission',
    //     'permissions',
    //     Object.keys(initialPermissions).join(', ')
    //   );
    // }

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

  // const validateForm = () => {
  //   const errors = {};
  //   if (!selectedUser) {
  //     errors.user = 'User is required';
  //   }
  //   if (!selectRole) {
  //     errors.role = 'Role is required';
  //   }
  //   return errors;
  // };

  const validateForm = () => {
    const errors = {};

    if (mode !== "edit" && !selectedUser) {
      errors.user = 'User is required';
    }

    if (!selectRole) {
      errors.role = 'Role is required';
    }

    return errors;
  };

  const addAdminUser = () => {
    const errors = validateForm();
    setFormErrors(errors);

    if (Object.keys(errors).length > 0) {
      console.log("Form validation errors:", errors);
      toast.error('Please fill out the form correctly');
      return;
    }

    const selectedPermissionIds = Object.entries(selectedPermissions)
      .filter(([id, isSelected]) => isSelected)
      .map(([id]) => Number(id));

    const data = { ...sentGeneralInfo, permissions: selectedPermissionIds }
    const apiAction = mode === "edit" ? updateAdminInfo : setAdminInfo;

    dispatch(apiAction(data))
      .then((response) => {
        if (response && response.payload?.isSuccess) {
          dispatch(getAllAdmins());
          onClose();
        } else {
          toast.error(response.error?.message || 'Error occurred');
        }
      })
      .catch((error) => console.error('Error updating user info:', error));
  };
  const handleSelectBranch = (branch) => {
    setSelectedBranch({ [branch.id]: branch.name });
    const existAddedBranchLockerGroups = selectedLockerGroupAddId[branch.id];
    if (existAddedBranchLockerGroups) {
      setSelectedLockerGroupId(existAddedBranchLockerGroups);
    } else {
      setSelectedLockerGroupId([]);
    }
    dispatch(setLockerGroupWithFilters(branch.id));
  };

  const handleBranchSelectAdd = (itemId) => {
    setSelectedLockerGroupId((prevSelected) => {
      if (!prevSelected.includes(itemId)) {
        return [...prevSelected, itemId];
      }
      return prevSelected;
    });
  };

  const handleBranchSelectRemove = (itemId) => {
    setSelectedLockerGroupId((prevSelected) => {
      if (prevSelected.includes(itemId)) {
        return prevSelected.filter((id) => id !== itemId);
      }
      return prevSelected;
    });
  };

  const handleClickBranchSelect = (itemId) => {
    setSelectedLockerGroupId((prevSelected) => {
      if (prevSelected.includes(itemId)) {
        return prevSelected.filter((id) => id !== itemId);
      } else {
        return [...prevSelected, itemId];
      }
    });
  };

  const sendGroupIds = () => {
    setSelectedLockerGroupAddId((prev) => ({
      ...prev,
      [Object.keys(selectedBranch)]: selectedLockerGroupId,
    }));

    const selectedLockerGroups = { ...selectedLockerGroupAddId, [Object.keys(selectedBranch)]: selectedLockerGroupId };

    const generalInfo = Object.entries(selectedLockerGroups)
      .filter(([branchIdString, groupIds]) => groupIds.length > 0)
      .map(([branchIdString, groupIds]) => {
        const branchId = Number(branchIdString);
        const branch = branches.find(b => b.id === branchId);
        const branchName = branch ? branch.name : "Unknown";
        const groupIdsString = groupIds.join(",");
        return `${branchName}: ${groupIdsString}`;
      })
      .join("; ");

    dispatch(
      setAddUserInfo({
        'Groups': {
          ...userInfo['Groups'],
          ['groups']: Object.values(selectedLockerGroups).flat(),
        },
      })
    );

    setSentGeneralInfo((prev) => ({
      ...prev,
      'groupIds': Object.values(selectedLockerGroups).flat(),
    }));

    setUserRight((prev) => ({
      ...prev,
      'Groups': {
        ...(prev['Groups'] || {}),
        'groups': generalInfo,
      },
    }));

    toast.success("Groups added successfully");
  };

  const renderError = (fieldName) => {
    if (formErrors[fieldName]) {
      return (
        <ShowFormikError message={formErrors[fieldName]} />
      );
    }
    return null;
  };

  const sendGroupInfo = (part, key, value) => {
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

    // setUserRight((prev) => ({
    //   ...prev,
    //   [part]: {
    //     ...(prev[part] || {}),
    //     [key]: value,
    //   },
    // }));
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

      // const trueIds = Object.entries(updated)
      //   .filter(([_, value]) => value)
      //   .map(([key]) => key)
      //   .join(', ');

      // if (trueIds.length) {
      //   sendGroupInfo('Permission', 'permissions', trueIds);
      // }

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
      <div className="add__modal__content add__modal__content__addAdminUser rounded-lg shadow-lg w-full max-w-4xl overflow-auto h-full">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Assign Administrator To The User</h2>
          <CloseButton onClick={onClose}>
            &times;
          </CloseButton>
        </div>

        <Tabs>
          <TabList>
            <Tab>Info</Tab>
            <Tab>Locker groups</Tab>
          </TabList>
          <TabPanel>
            <div>
              <div className="flex">
                {/* User Info */}
                <div className="add__modal__content__part w-[70%] mr-1">
                  <span>User</span>
                  <div className="add__modal__content__part__group mb-4 flex justify-between">
                    {
                      mode === 'edit' ? (
                        <input
                          type="text"
                          value={initialData.name + ' ' + initialData.surname}
                          disabled
                          className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5"
                        />) : (
                        <div className="w-full mr-2">
                          <CustomSelect
                            options={userOptions}
                            value={userOptions.find((option) => option.value === selectedUser)}
                            onChange={(e) => handleUserSelectChange(e)}
                          />
                          {renderError('user')}
                        </div>)
                    }

                  </div>
                </div>

                <div className="add__modal__content__part w-[30%]">
                  <span>Choose Role</span>
                  <div className="add__modal__content__part__group mb-4 flex">
                    <div className="add__modal__group__select mr-0 w-full">
                      <CustomSelect
                        options={roleOptions}
                        value={roleOptions.find((option) => option.value === selectRole)}
                        onChange={handleRoleSelectChange}
                      />
                      {renderError('role')}
                    </div>
                  </div>
                </div>
              </div>
              {
                selectRole && permissions?.length > 0 && (<div >
                  <div className="add__modal__content__part  mr-1">
                    <span>Right</span>
                    <div className="add__modal__content__part__group mb-4 flex justify-between">
                      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-2 gap-6">
                        {permissions.map((type) => (
                          <div key={type.typeId} className="mb-4">
                            <h3 className="text-lg font-semibold text-white mb-2">{fromCamelCasePretty(type.typeName)}</h3>
                            {type.permissions.map((permission) => (
                              <div key={permission.id} className="permission-item flex items-center ml-4">
                                <CustomCheckbox
                                  id={permission.id}
                                  checked={selectedPermissions[permission.id]}
                                  onChange={() => handleCheckboxChange(permission.id)}
                                />
                                <span className="ml-2 text-white">{fromCamelCasePretty(permission.name)}</span>
                              </div>
                            ))}
                          </div>
                        ))}
                      </div>
                    </div>
                  </div>
                </div>)
              }
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
                    value={formatUserRightText()}
                  />
                </div>
              </div>
              <div className="add__modal__content__part__select w-full">
                <span>Branches</span>
                <div className="add__modal__content__part__group mb-4c overflow-x-auto">
                  <div className="flex mr-2">
                    {
                      branches?.length > 0 && (
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
                {selectedBranch && filteredLockerGroups?.length ? (
                  <div className="pl-5 select-none flex flex-wrap">
                    {filteredLockerGroups.map((lockerGroup, groupIndex) => (
                      <React.Fragment key={groupIndex}>
                        <div
                          className={`m-1 border-4 rounded-[14px] rounded-lg transition-colors duration-200 ${selectedLockerGroupId?.includes(lockerGroup.id)
                            ? 'border-[#62cb62]'
                            : 'border-transparent'
                            }`}
                          onMouseOver={(event) => {
                            if (event.buttons === 1 && event.ctrlKey) {
                              handleBranchSelectRemove(lockerGroup.id);
                            } else if (event.buttons === 1) {
                              handleBranchSelectAdd(lockerGroup.id);
                            }
                          }}
                          onClick={() => handleClickBranchSelect(lockerGroup.id)}
                        >
                          <div className={`
                                flex 
                                bg-[#475456] 
                                p-3 
                                rounded-xl 
                                text-white 
                                cursor-pointer 
                            }`}>
                            {lockerGroup.name}
                          </div>
                        </div>
                      </React.Fragment>
                    ))}
                  </div>
                ) : (
                  <NoData text="No Selected Groups" />
                )}
              </div>
              <div className="section__add">
                <button
                  onClick={sendGroupIds}
                  className="text-white font-bold rounded p-2"
                >
                  Add groups
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
