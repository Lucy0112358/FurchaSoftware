import React, { useEffect, useRef, useState } from "react";
import './userModal.css';
import '../modal.css';
import CustomCheckbox from "../../checkbox/CustomCheckbox";
import { useDispatch, useSelector } from 'react-redux';
import { getAddUserInfo, setAddUserInfo } from "../../../redux/slice/userSlice";
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import 'react-tabs/style/react-tabs.css';
import { getBranchesData } from "../../../redux/slice/menuSlice";
import { getUsers, setUserInfo, updateUserInfo } from "../../../redux/api/userApi";
import { IoMdAdd } from "react-icons/io";
import { MdDelete } from "react-icons/md";
import { getLockerGroupsByBranchId } from "../../../redux/api/branchApi";
import { clearFilteredLockerGroups, getFilteredLockerGroups } from "../../../redux/slice/lockerSlice";
import GroupName from "../../headers/GroupName";
import GenerateLocker from "../../lockers/GenerateLocker";
import NoData from "../../no-data/NoData";
import { toast } from "react-toastify";
import CloseButton from "../attributes/CloseButton";
import CustomSelect from "../../select/CustomSelect";
import ShowFormikError from "../../error/ShowFormikError";
import { getAllGroupsData } from "../../../redux/slice/groupSlice";
import { getAllGroups } from "../../../redux/api/groupApi";
import AddUserGroupModal from "./addUserGroup/AddUserGroupModal";

const AddUserModal = ({ isOpen, onClose, mode = "add", initialData = {} }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();
  const userInfo = useSelector(getAddUserInfo);
  const [formErrors, setFormErrors] = useState({});
  const cardRef = useRef(null);
  const [cards, setCards] = useState([]);
  const [addGroupModalSwitch, setAddGroupModalSwitch] = useState(false);
  const branches = useSelector(getBranchesData);
  const filteredBranchGroups = useSelector(getFilteredLockerGroups);
  const userGroups = useSelector(getAllGroupsData);
  const [selectedGroups, setSelectedGroups] = useState(null);
  const [selectedLockerId, setSelectedLockerId] = useState([]);
  const [selectedLockerAddId, setSelectedLockerAddId] = useState({});
  const [userRight, setUserRight] = useState({});
  const [sentGeneralInfo, setSentGeneralInfo] = useState({ isPinRequired: false });
  const [selectedBranch, setSelectedBranch] = useState({});

  useEffect(() => {
    dispatch(getAllGroups());
  }, [dispatch]);

  useEffect(() => {
    if (mode === "edit" && initialData) {
      setSentGeneralInfo({
        ...initialData,
        name: initialData.name || '',
        surname: initialData.surname || '',
        email: initialData.email || '',
        isPinRequired: initialData.isPinRequired || false,
        userGroups: initialData.userGroups || [],
        cards: initialData.cards || [],
        lockerIds: initialData.lockerIds || [],
        activeFrom: initialData.activeFrom
          ? initialData.activeFrom.split("T")[0]
          : '',
        activeTo: initialData.activeTo
          ? initialData.activeTo.split("T")[0]
          : '',
      });
      setCards(initialData.cards || []);
      setSelectedGroups(
        initialData.userGroups?.map(id => ({
          value: id,
          label: userGroups.find(group => group.id === id)?.name || "Unknown"
        })) || []
      );
    }
  }, [mode, initialData, userGroups]);

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

  const sendGroupInfo = (part, key, value) => {
    dispatch(
      setAddUserInfo({
        [part]: {
          ...userInfo[part],
          [key]: value,
        },
      })
    );
    setSentGeneralInfo((prev) => ({ ...prev, [key]: value }));
    // setUserRight((prev) => ({
    //   ...prev,
    //   [part]: { ...(prev[part] || {}), [key]: value },
    // }));
  };

  const validateForm = () => {
    const errors = {};
    if (!sentGeneralInfo.name || !sentGeneralInfo.name.trim()) errors.name = 'Name is required';
    if (!sentGeneralInfo.surname || !sentGeneralInfo.surname.trim()) errors.surname = 'Surname is required';
    if (!sentGeneralInfo.email || !sentGeneralInfo.email.trim()) {
      errors.email = 'Email is required';
    } else {
      const emailRegex = /\S+@\S+\.\S+/;
      if (!emailRegex.test(sentGeneralInfo.email)) {
        errors.email = 'Invalid email format';
      }
    }
    if (!sentGeneralInfo.phone || !sentGeneralInfo.phone.trim()) errors.phone = 'Phone is required';
    // if (!sentGeneralInfo.userGroups || !sentGeneralInfo.userGroups.length) errors.userGroups = 'At least one User Group must be selected';
    return errors;
  };

  const addUser = () => {
    const errors = validateForm();
    setFormErrors(errors);
    if (Object.keys(errors).length) {
      toast.error('Please fill out the form correctly');
      return;
    }
    dispatch(setUserInfo(sentGeneralInfo))
      .then(res => {
        if (res?.payload?.isSuccess) {
          toast.success("User added successfully");
          onClose();
        } else {
          toast.error(res.payload || 'Error occurred');
        }
      });
  };

//   const sendLockerIds = () => {

//     setSelectedLockerAddId((prev) => ({
//       ...prev,
//       [Object.keys(selectedBranch)]: selectedLockerId,
//     }));

//     const selectedLockers = { ...selectedLockerAddId, [Object.keys(selectedBranch)]: selectedLockerId };

//     const generalInfo = Object.entries(selectedLockers)
//       .filter(([branchIdString, lockerIds]) => lockerIds.length > 0)
//       .map(([branchIdString, lockerIds]) => {
//         const branchId = Number(branchIdString);
//         const branch = branches.find(b => b.id === branchId);
//         const branchName = branch ? branch.name : "Unknown";
//         const lockerIdsString = lockerIds.join(",");
//         return `${branchName}: ${lockerIdsString}`;
//       })
//       .join("; ");

//     dispatch(
//       setAddUserInfo({
//         'Locker': {
//           ...userInfo['Locker'],
//           ['lockers']: Object.values(selectedLockers).flat(),
//         },
//       })
//     );
// console.log(selectedLockers, "selectedLockers");

//     setSentGeneralInfo((prev) => ({
//       ...prev,
//       'lockerIds': Object.values(selectedLockers).flat(),
//     }));

//     setUserRight((prev) => ({
//       ...prev,
//       'Locker': {
//         ...(prev['Locker'] || {}),
//         'lockers': generalInfo,
//       },
//     }));

//     // sendGroupInfo('Locker', 'lockers', generalInfo)

//     // const branchIds = selectedBranches.map((branch) => branch.value);
//     // setSentGeneralInfo((prev) => ({
//     //   ...prev,
//     //   branchIds: Object.values(selectedLockerAddId).flat(),
//     // }));
//     // console.log(selectedLockerId, "selectedLockerId");

//     // setSelectedBranchesForShow({
//     //   ...selectedBranchesForShow,
//     //   [selectedBranch]: selectedLockerId.toString(),
//     // });
//     toast.success("Lockers added successfully");
//   };

const sendLockerIds = () => {
    const branchIdKey = Object.keys(selectedBranch)[0];
    if (!branchIdKey) return;

    // Обновляем локальный стейт выбранных ID для конкретной ветки
    const updatedLockerAddId = {
      ...selectedLockerAddId,
      [branchIdKey]: selectedLockerId,
    };
    setSelectedLockerAddId(updatedLockerAddId);

    // 1. Формируем текст для отображения (номера вместо ID)
    const displayInfo = Object.entries(updatedLockerAddId)
      .filter(([_, ids]) => ids.length > 0)
      .map(([bId, ids]) => {
        const branch = branches.find((b) => b.id === Number(bId));
        const branchName = branch ? branch.name : "Unknown";

        // Ищем номера шкафчиков по их ID в загруженных группах
        const lockerNumbers = ids.map((id) => {
          let foundNumber = id; // fallback
          filteredBranchGroups.forEach((group) => {
            const locker = group.groupLockers.find((l) => l.id === id);
            if (locker) foundNumber = locker.number || locker.name || id;
          });
          return foundNumber;
        });

        return `${branchName}: ${lockerNumbers.join(",")}`;
      })
      .join("; ");

    // 2. Данные для Redux (UI часть)
    dispatch(
      setAddUserInfo({
        Locker: {
          ...userInfo["Locker"],
          lockers: Object.values(updatedLockerAddId).flat(),
        },
      })
    );

    // 3. Данные для отправки на БЭКЕНД (только ID)
    setSentGeneralInfo((prev) => ({
      ...prev,
      lockerIds: Object.values(updatedLockerAddId).flat(),
    }));

    // 4. Текст для визуального поля User Rights (номера)
    setUserRight((prev) => ({
      ...prev,
      Locker: {
        ...(prev["Locker"] || {}),
        lockers: displayInfo,
      },
    }));

    toast.success("Lockers added successfully");
  };


  useEffect(() => {
    if (mode === "edit" && initialData?.branches?.length) {
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
      setUserRight(prev => ({
        ...prev,
        Locker: { lockers: generalInfo }
      }));
    }
  }, [mode, initialData]);

  const updateUser = () => {
    const errors = validateForm();
    setFormErrors(errors);
    if (Object.keys(errors).length) {
      toast.error("Please fill out the form correctly");
      return;
    }
    dispatch(updateUserInfo({ ...sentGeneralInfo, id: initialData.id }))
      .then(res => {
        if (res?.payload?.isSuccess) {
          toast.success("User updated successfully");
          dispatch(getUsers());
          onClose();
        } else {
          toast.error(res.error?.message || "Error occurred");
        }
      });
  };

  const setCardNumbers = () => {
    if (!cardRef.current) return;
    const newCard = cardRef.current.value.trim();
    if (!newCard) return;
    const newCards = [...cards, newCard];
    setCards(newCards);
    setSentGeneralInfo(prev => ({ ...prev, cards: newCards }));
    sendGroupInfo('Card No', 'cards', newCards);
    cardRef.current.value = '';
  };

  const handleDeleteCards = (value) => {
    const newCards = cards.filter(card => card !== value);
    setCards(newCards);
    setSentGeneralInfo(prev => ({ ...prev, cards: newCards }));
    sendGroupInfo('Card No', 'cards', newCards);
  };

  const handleGroupsSelectChange = (selectedOption) => {
    setSelectedGroups(selectedOption);
    const ids = selectedOption.map(o => o.value);
    setSentGeneralInfo(prev => ({ ...prev, userGroups: ids }));
    sendGroupInfo('User Groups', 'userGroups', ids);
    // TODO: Check filter application
    // if (selectedOption.length) dispatch(userFilter({ filterByGroupId: selectedOption[0].value }));
  };

  const handleSelectBranch = (branch) => {
    setSelectedBranch({ [branch.id]: branch.name });
    const alreadySelected = selectedLockerAddId[branch.id] || [];
    console.log(alreadySelected, 'alreadySelected');

    setSelectedLockerId(alreadySelected);
    dispatch(getLockerGroupsByBranchId(branch.id));
  };

  const handlePin = (value) => {
    setSentGeneralInfo(prev => ({ ...prev, isPinRequired: value }));
    sendGroupInfo('Pin', 'isPinRequired', value);
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

  const handleBranchSelectRemove = (itemId) => {
    setSelectedLockerId((prevSelected) => {
      if (prevSelected.includes(itemId)) {
        return prevSelected.filter((id) => id !== itemId);
      }
      return prevSelected;
    });
  };

  const handleBranchSelectAdd = (itemId) => {
    setSelectedLockerId((prevSelected) => {
      if (!prevSelected.includes(itemId)) {
        return [...prevSelected, itemId];
      }
      return prevSelected;
    });
  };

  const handleClose = () => {
    setSelectedLockerId([]);
    setSelectedLockerAddId({});
    setUserRight({});
    setSentGeneralInfo({ isPinRequired: false });
    setSelectedBranch({});
    setCards([]);
    setSelectedGroups(null);

    dispatch(clearFilteredLockerGroups([]));

    onClose();
  };

  const renderError = (field) => formErrors[field] ? <ShowFormikError message={formErrors[field]} /> : null;

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__addUser rounded-lg shadow-lg w-full max-w-4xl overflow-auto h-full">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">{mode === 'add' ? 'Add User' : 'Edit User'}</h2>
          <CloseButton onClick={handleClose}>&times;</CloseButton>
        </div>
        <Tabs>
          <TabList>
            <Tab>Info</Tab>
            <Tab>Lockers</Tab>
          </TabList>

          <TabPanel>
            <div>
              {/* User Info */}
              <div className="add__modal__content__part">
                <span>User Info</span>
                <div className="add__modal__content__part__group grid grid-cols-2 gap-4 mb-4">
                  <div>
                    {/* <input
                      placeholder="Name"
                      type="text"
                      value={userInfo.user_info?.name}
                      onChange={(e) =>
                        sendGroupInfo('user_info', 'name', e.target.value)
                      }
                      className="w-full p-1 border rounded"
                    /> */}
                    <input
                      placeholder="Name"
                      type="text"
                      value={sentGeneralInfo.name || ''}
                      onChange={(e) =>
                        sendGroupInfo('user_info', 'name', e.target.value)
                      }
                      className="w-full p-1 border rounded"
                    />
                    {renderError('name')}
                  </div>
                  <div>
                    <input
                      placeholder="Last name"
                      type="text"
                      value={sentGeneralInfo.surname || ''}
                      onChange={(e) =>
                        sendGroupInfo('user_info', 'surname', e.target.value)
                      }
                      className="w-full p-1 border rounded"
                    />
                    {renderError('surname')}
                  </div>
                  <div>
                    <input
                      placeholder="Email"
                      type="email"
                      value={sentGeneralInfo.email || ''}
                      onChange={(e) =>
                        sendGroupInfo('user_info', 'email', e.target.value)
                      }
                      className="w-full p-1 border rounded"
                    />
                    {renderError('email')}
                  </div>
                  <div>
                    <input
                      placeholder="Phone"
                      type="text"
                      value={sentGeneralInfo.phone || ''}
                      onChange={(e) =>
                        sendGroupInfo('user_info', 'phone', e.target.value)
                      }
                      className="w-full p-1 border rounded"
                    />
                    {renderError('phone')}
                  </div>
                </div>
              </div>
              {/* Active Period */}
              <div className="add__modal__content__part">
                <span>Active Period</span>
                <div className="add__modal__content__part__group grid grid-cols-2 gap-4 mb-4">
                  <div>
                    <label className="block text-gray-300">From</label>
                    <input
                      type="date"
                      value={sentGeneralInfo.activeFrom || ''}
                      onChange={(e) =>
                        sendGroupInfo('active_period', 'activeFrom', e.target.value)
                      }
                      className="w-full p-1 border rounded"
                    />
                    {renderError('activeFrom')}
                  </div>
                  <div>
                    <label className="block text-gray-300">To</label>
                    <input
                      type="date"
                      value={sentGeneralInfo.activeTo || ''}
                      onChange={(e) =>
                        sendGroupInfo('active_period', 'activeTo', e.target.value)
                      }
                      className="w-full p-1 border rounded"
                    />
                    {renderError('activeTo')}
                  </div>
                </div>
              </div>

              {/* User group */}
              <div className="add__modal__content__part">
                <span>User group</span>
                <div className="add__modal__content__part__group mb-4 flex items-top">
                  <div className="add__modal__group__select mr-4 w-full">
                    <CustomSelect
                      options={userGroups.map((group) => ({
                        label: group.name,
                        value: group.id,
                      }))}
                      value={selectedGroups}
                      onChange={handleGroupsSelectChange}
                      multiChoose={true}
                    />
                    {renderError('userGroups')}
                  </div>
                  <div className="flex justify-center">
                    <button
                      onClick={() => setAddGroupModalSwitch(true)}
                      className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center h-[43px]"
                    >
                      <IoMdAdd className="fill-current text-2xl" />
                      <AddUserGroupModal
                        isOpen={addGroupModalSwitch}
                        onClose={(e) => {
                          if (e?.stopPropagation) e.stopPropagation();
                          setAddGroupModalSwitch(false);
                        }}
                      />
                    </button>
                  </div>
                </div>
              </div>

              {/* Credentials */}
              <div className="flex">
                <div className="add__modal__content__part w-full">
                  <span>Credentials</span>
                  <div className="add__modal__content__part__group crendentails mb-4">
                    <div className="col-span-2 flex items-center">
                      <div className="mr-2 w-full">
                        <div className=" mb-4 flex items-end">
                          <div className="add__modal__group__select mr-4 w-full">
                            <label className="block text-gray-300">Card no.</label>
                            <input
                              type="text"
                              ref={cardRef}
                              className="w-full p-1 border rounded"
                            />
                          </div>
                          <div className="section__add">
                            <button
                              style={{ height: "40px" }}
                              onClick={setCardNumbers}
                              className="text-white font-bold rounded"
                            >
                              Add
                            </button>
                          </div>
                        </div>
                        {cards.length > 0 && (
                          <div>
                            <label className="block text-gray-300">
                              Card manage
                            </label>
                            <div className="card__manage">
                              <ul className="list-disc pl-5">
                                {cards.map((value, index) => (
                                  <li
                                    key={index}
                                    className="flex justify-between items-center mb-2"
                                  >
                                    <span>{value}</span>
                                    <button
                                      onClick={() => handleDeleteCards(value)}
                                      className="bg-red-500 text-white px-2 py-1 rounded-md hover:bg-red-700"
                                    >
                                      <MdDelete />
                                    </button>
                                  </li>
                                ))}
                              </ul>
                            </div>
                          </div>
                        )}
                      </div>
                    </div>
                    <div className="col-span-1 mt-5 flex justify-between items-center">
                      <div className="flex">
                        <div className="generation w-full">
                          <div className="generation__checkbox flex items-center">
                            <CustomCheckbox
                              id={'pin'}
                              checked={sentGeneralInfo.isPinRequired}
                              onChange={(checked) => handlePin(checked)}
                            />
                            <span className="text-white">PIN</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>


            </div>

            {/* Кнопки */}
            {/* <div className="flex justify-end space-x-4">
              <div className="modal__button">
                <button className="bg-gray-600 text-white rounded" onClick={onClose}>
                  Cancel
                </button>
              </div>
              <div className="modal__button">
                <button className="bg-gray-600 text-white rounded" onClick={addUser}>
                  Save
                </button>
              </div>
            </div> */}
            <div className="flex justify-end space-x-4 mt-4">
              <button className="bg-gray-600 text-white rounded px-4 py-2" onClick={handleClose}>Cancel</button>
              <button className="bg-gray-600 text-white rounded px-4 py-2" onClick={mode === 'add' ? addUser : updateUser}>
                {mode === 'add' ? 'Save' : 'Update'}
              </button>
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
          </TabPanel>
        </Tabs>
      </div>
    </div>
  );
};

export default AddUserModal;
