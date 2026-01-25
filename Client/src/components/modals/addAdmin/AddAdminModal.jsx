import React, { useEffect, useRef, useState, useMemo, useCallback } from "react";
import { useDispatch, useSelector } from 'react-redux';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import { toast } from "react-toastify";

// Styles
import './addAdmin.css';
import '../modal.css';
import 'react-tabs/style/react-tabs.css';

// Components
import CustomCheckbox from "../../checkbox/CustomCheckbox";
import CustomSelect from "../../select/CustomSelect";
import NoData from "../../no-data/NoData";
import CloseButton from "../attributes/CloseButton";
import ShowFormikError from "../../error/ShowFormikError";

// Redux & API
import { getAllUsersData, setAddUserInfo } from "../../../redux/slice/userSlice";
import { getBranchesData, getLockerGroups, setLockerGroupWithFilters, getFilteredLockerGroups } from "../../../redux/slice/menuSlice";
import { getUsers } from "../../../redux/api/userApi";
import { getLockerGroupsData } from "../../../redux/api/menuApi";
import { getPermissions, getRoles } from "../../../redux/api/authApi";
import { getPermissionsData, getRolesData } from "../../../redux/slice/authSlice";
import { getAllAdmins, setAdminInfo, updateAdminInfo } from "../../../redux/api/adminApi";

// Utils
import { fromCamelCasePretty } from "../../../Utils";

const AddAdminModal = ({ onClose, mode = "add", initialData = {} }) => {
  const dispatch = useDispatch();

  // --- Selectors ---
  const branches = useSelector(getBranchesData);
  const usersData = useSelector(getAllUsersData);
  const roles = useSelector(getRolesData);
  const permissions = useSelector(getPermissionsData);
  const filteredLockerGroups = useSelector(getFilteredLockerGroups);
  const allLockerGroups = useSelector(getLockerGroups);

  // --- State ---
  const [formErrors, setFormErrors] = useState({});
  const [selectedLockerGroupId, setSelectedLockerGroupId] = useState([]);
  const [selectedLockerGroupAddId, setSelectedLockerGroupAddId] = useState({});
  const [userRight, setUserRight] = useState({});
  const [sentGeneralInfo, setSentGeneralInfo] = useState({});
  const [selectedBranch, setSelectedBranch] = useState({});
  const [selectedUser, setSelectedUser] = useState(null);
  const [selectRole, setSelectedRole] = useState(null);
  const [selectedPermissions, setSelectedPermissions] = useState({});

  // --- Memos ---
  const userOptions = useMemo(() => 
    usersData?.map((user) => ({ value: user.id, label: user.name })) || [], 
  [usersData]);

  const roleOptions = useMemo(() => 
    roles?.map((role) => ({ value: role.id, label: role.name })) || [], 
  [roles]);

  // --- Helpers ---
  const getGroupNames = useCallback((groupIds) => {
    return groupIds.map((id) => {
      const foundGroup = allLockerGroups?.find((g) => g.id === id);
      return foundGroup ? foundGroup.name : id;
    });
  }, [allLockerGroups]);

  const formatUserRightText = () => {
    return Object.entries(userRight)
      .map(([part, values]) => {
        const valuesEntries = Object.entries(values).map(([key, value]) => `   ${key}: ${value}`);
        return `${part}:\n${valuesEntries.join('\n')}`;
      })
      .join('\n');
  };

  // --- Effects ---
  useEffect(() => {
    dispatch(getUsers({ isAdmin: false }));
    dispatch(getRoles());
    dispatch(getLockerGroupsData());
  }, [dispatch]);

  // Initial Data Setup
  useEffect(() => {
    if (initialData && Object.keys(initialData).length > 0) {
      const { roleId, id, branches: initialBranches, permissionIds } = initialData;

      if (roleId) {
        setSelectedRole(roleId);
        dispatch(getPermissions(roleId));
      }

      const existingGroupIds = initialBranches?.flatMap(branch => branch.groupIds) || [];
      setSelectedLockerGroupId(existingGroupIds);

      const lockerGroupMap = Object.fromEntries(
        initialBranches?.map(branch => [branch.branchId, branch.groupIds]) || []
      );
      setSelectedLockerGroupAddId(lockerGroupMap);

      setSentGeneralInfo({
        roleId,
        id,
        groupIds: existingGroupIds,
      });

      const generalInfoText = Array.isArray(initialBranches)
        ? initialBranches
          .filter((branch) => branch.groupIds?.length > 0)
          .map((branch) => `${branch?.branchName || "Unknown"}: ${getGroupNames(branch.groupIds).join(", ")}`)
          .join("; ")
        : "";

      setUserRight(prev => ({
        ...prev,
        'Groups': { ...prev['Groups'], 'groups': generalInfoText },
      }));

      if (permissionIds) {
        setSelectedPermissions(prev => 
          Object.fromEntries(Object.keys(prev).map(key => [key, permissionIds.includes(Number(key))]))
        );
      }
    }
  }, [initialData, dispatch, getGroupNames]);

  // Permissions Sync
  useEffect(() => {
    const allPermissions = permissions.flatMap(type => type.permissions);
    const initialPermissions = allPermissions.reduce((acc, permission) => {
      acc[permission.id] = true;
      return acc;
    }, {});
    setSelectedPermissions(initialPermissions);
  }, [permissions]);

  // --- Handlers ---
  const validateForm = () => {
    const errors = {};
    if (mode !== "edit" && !selectedUser) errors.user = 'User is required';
    if (!selectRole) errors.role = 'Role is required';
    return errors;
  };

  const sendGroupInfo = (part, key, value) => {
    dispatch(setAddUserInfo({ [part]: { [key]: value } }));
    setSentGeneralInfo(prev => ({ ...prev, [key]: value }));
  };

  const addAdminUser = () => {
    const errors = validateForm();
    setFormErrors(errors);

    if (Object.keys(errors).length > 0) {
      toast.error('Please fill out the form correctly');
      return;
    }

    const selectedPermissionIds = Object.entries(selectedPermissions)
      .filter(([_, isSelected]) => isSelected)
      .map(([id]) => Number(id));

    const data = { ...sentGeneralInfo, permissions: selectedPermissionIds };
    const apiAction = mode === "edit" ? updateAdminInfo : setAdminInfo;

    dispatch(apiAction(data))
      .then((response) => {
        if (response?.payload?.isSuccess) {
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
    setSelectedLockerGroupId(selectedLockerGroupAddId[branch.id] || []);
    dispatch(setLockerGroupWithFilters(branch.id));
  };

  const handleBranchSelectAdd = (itemId) => {
    setSelectedLockerGroupId(prev => prev.includes(itemId) ? prev : [...prev, itemId]);
  };

  const handleBranchSelectRemove = (itemId) => {
    setSelectedLockerGroupId(prev => prev.filter(id => id !== itemId));
  };

  const handleClickBranchSelect = (itemId) => {
    setSelectedLockerGroupId(prev => 
      prev.includes(itemId) ? prev.filter(id => id !== itemId) : [...prev, itemId]
    );
  };

  const sendGroupIds = () => {
    const branchIdKey = Object.keys(selectedBranch)[0];
    if (!branchIdKey) return;

    const updatedLockerGroupAddId = {
      ...selectedLockerGroupAddId,
      [branchIdKey]: selectedLockerGroupId,
    };
    setSelectedLockerGroupAddId(updatedLockerGroupAddId);

    const displayInfo = Object.entries(updatedLockerGroupAddId)
      .filter(([_, groupIds]) => groupIds.length > 0)
      .map(([bId, groupIds]) => {
        const branch = branches.find((b) => b.id === Number(bId));
        return `${branch ? branch.name : "Unknown"}: ${getGroupNames(groupIds).join(", ")}`;
      })
      .join("; ");

    const flatGroupIds = Object.values(updatedLockerGroupAddId).flat();

    dispatch(setAddUserInfo({ Groups: { groups: flatGroupIds } }));
    setSentGeneralInfo(prev => ({ ...prev, groupIds: flatGroupIds }));
    setUserRight(prev => ({
      ...prev,
      Groups: { ...prev["Groups"], groups: displayInfo },
    }));

    toast.success("Groups added successfully");
  };

  const handleRoleSelectChange = (selectedOption) => {
    const selectedRoleId = selectedOption.value;
    setSelectedRole(selectedRoleId);
    dispatch(getPermissions(selectedRoleId));
    sendGroupInfo('Role', 'roleId', selectedRoleId);
  };

  const handleCheckboxChange = (id) => {
    setSelectedPermissions(prev => ({ ...prev, [id]: !prev[id] }));
  };

  const handleUserSelectChange = (selectedOption) => {
    const selectedUserId = selectedOption.value;
    setSelectedUser(selectedUserId);
    sendGroupInfo('User', 'userId', selectedUserId);
  };

  const renderError = (fieldName) => formErrors[fieldName] ? <ShowFormikError message={formErrors[fieldName]} /> : null;

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__addAdminUser rounded-lg shadow-lg w-full max-w-4xl overflow-auto h-full">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Assign Administrator To The User</h2>
          <CloseButton onClick={onClose}>&times;</CloseButton>
        </div>

        <Tabs>
          <TabList>
            <Tab>Info</Tab>
            <Tab>Locker groups</Tab>
          </TabList>

          <TabPanel>
            <div>
              <div className="flex">
                <div className="add__modal__content__part w-[70%] mr-1">
                  <span>User</span>
                  <div className="add__modal__content__part__group mb-4 flex justify-between">
                    {mode === 'edit' ? (
                      <input
                        type="text"
                        value={`${initialData.name || ''} ${initialData.surname || ''}`}
                        disabled
                        className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg block w-full p-2.5"
                      />
                    ) : (
                      <div className="w-full mr-2">
                        <CustomSelect
                          options={userOptions}
                          value={userOptions.find((opt) => opt.value === selectedUser)}
                          onChange={handleUserSelectChange}
                        />
                        {renderError('user')}
                      </div>
                    )}
                  </div>
                </div>

                <div className="add__modal__content__part w-[30%]">
                  <span>Choose Role</span>
                  <div className="add__modal__content__part__group mb-4 flex">
                    <div className="add__modal__group__select mr-0 w-full">
                      <CustomSelect
                        options={roleOptions}
                        value={roleOptions.find((opt) => opt.value === selectRole)}
                        onChange={handleRoleSelectChange}
                      />
                      {renderError('role')}
                    </div>
                  </div>
                </div>
              </div>

              {selectRole && permissions?.length > 0 && (
                <div className="add__modal__content__part mr-1">
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
                                checked={!!selectedPermissions[permission.id]}
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
              )}
            </div>

            <div className="flex justify-end space-x-4">
              <div className="modal__button">
                <button className="bg-gray-600 text-white rounded" onClick={onClose}>Cancel</button>
              </div>
              <div className="modal__button">
                <button className="bg-gray-600 text-white rounded" onClick={addAdminUser}>Save</button>
              </div>
            </div>
          </TabPanel>

          <TabPanel>
            <div className="flex justify-between flex-col" onContextMenu={(e) => e.preventDefault()}>
              <div className="add__modal__content__part">
                <span>User rights</span>
                <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
                  <label className="block text-gray-300">User rights details</label>
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
                    {branches?.filter(b => b.name !== 'All').map((branch) => (
                      <div
                        key={branch.id}
                        onClick={() => handleSelectBranch(branch)}
                        className={`flex mr-2 bg-[#475456] p-3 rounded-xl text-white cursor-pointer ${
                          selectedBranch.hasOwnProperty(branch.id) ? "border-2 border-cyan-500" : ""
                        }`}
                      >
                        {branch.name}
                      </div>
                    ))}
                    {renderError('branch')}
                  </div>
                </div>
              </div>

              <div>
                {Object.keys(selectedBranch).length > 0 && filteredLockerGroups?.length ? (
                  <div className="pl-5 select-none flex flex-wrap">
                    {filteredLockerGroups.map((group) => (
                      <div
                        key={group.id}
                        className={`m-1 border-4 rounded-[14px] transition-colors duration-200 ${
                          selectedLockerGroupId?.includes(group.id) ? 'border-[#62cb62]' : 'border-transparent'
                        }`}
                        onMouseOver={(e) => {
                          if (e.buttons === 1 && e.ctrlKey) handleBranchSelectRemove(group.id);
                          else if (e.buttons === 1) handleBranchSelectAdd(group.id);
                        }}
                        onClick={() => handleClickBranchSelect(group.id)}
                      >
                        <div className="flex bg-[#475456] p-3 rounded-xl text-white cursor-pointer">
                          {group.name}
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <NoData text="No Selected Groups" />
                )}
              </div>

              <div className="section__add">
                <button onClick={sendGroupIds} className="text-white font-bold rounded p-2">
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