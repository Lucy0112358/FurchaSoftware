import React, { useEffect, useState } from "react";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";
import './addUserGroup.css';
import '../../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import { getAddUserInfo, setAddUserInfo } from "../../../../redux/slice/userSlice";
import 'react-tabs/style/react-tabs.css';
import { getBranchesData, getUserGroupsData } from "../../../../redux/slice/menuSlice";
import { getFilteredLockerGroups } from "../../../../redux/slice/lockerSlice";
import NoData from "../../../no-data/NoData";
import { getLockerGroupsByBranchId } from "../../../../redux/api/branchApi";
import GroupName from "../../../headers/GroupName";
import GenerateLocker from "../../../lockers/GenerateLocker";
import { toast } from "react-toastify";
import { getAllGroups, setUserGroup, updateUserGroupInfo } from "../../../../redux/api/groupApi";
import ShowFormikError from "../../../error/ShowFormikError";
import CloseButton from "../../attributes/CloseButton";
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import 'react-tabs/style/react-tabs.css';


const AddUserGroupModal = ({ isOpen, onClose, mode = "add", initialData = {} }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();
  const branches = useSelector(getBranchesData);
  const [selectedBranch, setSelectedBranch] = useState([]);
  const [sentGeneralInfo, setSentGeneralInfo] = useState({
    branches: [],
    name: '',
  });
  console.log(sentGeneralInfo, initialData, "sentGeneralInfo");

  const filteredBranchGroups = useSelector(getFilteredLockerGroups)
  const [selectedLockerId, setSelectedLockerId] = useState([]);
  const userInfo = useSelector(getAddUserInfo);
  const [groupRight, setGroupRight] = useState({});
  const [formErrors, setFormErrors] = useState({});
  const [selectedLockerAddId, setSelectedLockerAddId] = useState({});

  useEffect(() => {
    if (mode === "edit" && initialData) {
      setSentGeneralInfo({
        ...initialData,
        name: initialData.name || '',
      });

      const lockerMap = {};
      initialData.branches.forEach(branch => {
        lockerMap[branch.id] = branch.lockers || [];
      });
      setSelectedLockerAddId(lockerMap);
      setSentGeneralInfo(prev => ({
        ...prev,
        lockerIds: Object.values(lockerMap).flat()
      }));

      const generalInfo = initialData.branches
        .map(branch => `${branch.name}: ${branch.lockers.join(",")}`)
        .join("; ");
      console.log(generalInfo, "generalInfo");
      setAddUserInfo({
        'Locker': {
          ...userInfo['Locker'],
          ['lockers']: Object.values(lockerMap).flat(),
        },
      })

      setSentGeneralInfo((prev) => ({
        ...prev,
        'lockerIds': Object.values(lockerMap).flat(),
      }));

      setGroupRight((prev) => ({
        ...prev,
        'Locker': {
          ...(prev['Locker'] || {}),
          'lockers': generalInfo,
        },
      }));

    }
  }, [mode, initialData]);

  const validateForm = () => {
    const errors = {};
    if (!sentGeneralInfo.name || !sentGeneralInfo.name.trim()) errors.name = 'Name is required';
    return errors;
  };



  const addAllInfoForUserGroup = (part, key, value) => {
    const updatedUserInfo = {
      [part]: {
        ...userInfo[part],
        [key]: value,
      },
    };
    dispatch(setAddUserInfo(updatedUserInfo));

    // setGroupRight((prev) => ({
    //   ...prev,
    //   ...updatedUserInfo,
    // }));
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

    setGroupRight((prev) => ({
      ...prev,
      'Locker': {
        ...(prev['Locker'] || {}),
        'lockers': generalInfo,
      },
    }));
    toast.success("Lockers added successfully");
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

  // const addUserGroup = () => {
  //   const errors = validateForm();
  //   setFormErrors(errors);
  //   if (Object.keys(errors).length) {
  //     toast.error('Please fill out the form correctly');
  //     return;
  //   }

  //   dispatch(setUserGroup(sentGeneralInfo))
  //     .then((response) => {
  //       if (response && response.payload.isSuccess) {
  //         dispatch(getAllGroups());
  //         onClose()
  //       }
  //     })
  //     .catch((error) => console.error('Error updating user info:', error));
  // }

  const saveUserGroup = () => {
    const errors = validateForm();
    setFormErrors(errors);
    if (Object.keys(errors).length) {
      toast.error('Please fill out the form correctly');
      return;
    }

    const action = mode === "edit"
      ? updateUserGroupInfo({ ...sentGeneralInfo, id: initialData.id })
      : setUserGroup(sentGeneralInfo);

    dispatch(action).then((response) => {
      if (response?.payload?.isSuccess) {
        dispatch(getAllGroups());
        onClose();
      }
    });
  };

  const sendGroupInfo = (part, key, value) => {
    addAllInfoForUserGroup(part, key, value);
    setSentGeneralInfo((prev) => ({
      ...prev,
      [key]: value,
    }));
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

  const renderError = (fieldName) => {
    if (formErrors[fieldName]) {
      return (
        <ShowFormikError message={formErrors[fieldName]} />
      );
    }
    return null;
  };
  const validationSchema = Yup.object({
    name: Yup.string().required("Name is required"),
  });

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
        <Tabs>
          <TabList>
            <Tab>Info</Tab>
            <Tab>Lockers</Tab>
          </TabList>

          <TabPanel>
            {/* User Group Info */}
            <Formik
              initialValues={{ name: sentGeneralInfo.name || "" }}
              enableReinitialize
              validationSchema={validationSchema}
              onSubmit={(values) => {
                sendGroupInfo("Group", "name", values.name);
                saveUserGroup();
              }}
            >
              {({ errors, touched, handleSubmit, setFieldValue, values }) => (
                <Form id="groupForm" onSubmit={handleSubmit}>
                  <div className="add__modal__content__part">
                    <span>User Group Info</span>
                    <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
                      <div>
                        <Field
                          name="name"
                          placeholder="Group Name"
                          type="text"
                          className="w-full p-1 border rounded"
                          value={values.name}
                          onChange={(e) => {
                            setFieldValue("name", e.target.value);
                            sendGroupInfo("Group", "name", e.target.value);
                          }}
                        />

                        {errors.name && touched.name && (
                          <ShowFormikError message={errors.name} />
                        )}
                      </div>
                    </div>
                  </div>
                </Form>
              )}
            </Formik>

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
                  type="submit"
                  form="groupForm"
                >
                  Save
                </button>
              </div>
            </div>
          </TabPanel>

          <TabPanel>
            <div className="flex justify-between flex-col" onContextMenu={(e) => e.preventDefault()}>
              <div className="add__modal__content__part__select w-full">
                <span>Branches</span>
                <div className="add__modal__content__part__group mb-4 overflow-x-auto">
                  <div className="flex mr-2">
                    {
                      branches?.length > 0 && (
                        branches.map((branch, index) => {
                          return (
                            branch.name !== 'All' &&
                            <>
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
                            </>
                          )
                        }
                        ))
                    }
                    {renderError('branch')}
                  </div>
                </div>
              </div>

              <div>
                {Object.keys(selectedBranch).length && filteredBranchGroups?.length ? (
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
                                : 'locker__border'
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
              <div>
                {/* {selectedBranchesForShow.length > 0 && (
                  <div className="pl-5 select-none">
                    {selectedBranchesForShow.map((group) => (
                     
                    )))} */}
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

            {/* User Rights */}
            <div className="add__modal__content__part">
              <span>Group rights</span>
              <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
                <label className="block text-gray-300">Group rights details</label>
                <textarea
                  className="w-full p-1 border rounded h-24"
                  readOnly
                  value={formatUserRightText()}
                />
              </div>
            </div>
          </TabPanel>
        </Tabs>
      </div>
    </div>
  );
};

export default AddUserGroupModal;