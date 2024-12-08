import React, { useRef, useState } from "react";
import './addUser.css';
import '../modal.css';
import CustomSelectTest from "../../select/CustomSelectTest";
import CustomCheckbox from "../../checkbox/CustomCheckbox";
import { useDispatch, useSelector } from 'react-redux';
import { getAddUserInfo, setAddUserInfo } from "../../../redux/slice/userSlice";
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import 'react-tabs/style/react-tabs.css';
import CustomSelect from "../../select/CustomSelect";
import { getBranchesData, getUserGroupsData } from "../../../redux/slice/menuSlice";
import { userFilter } from "../../../redux/api/menuApi";
import { setUserInfo } from "../../../redux/api/userApi";
import { IoMdAdd } from "react-icons/io";
import { MdDelete } from "react-icons/md";
import AddUserGroupModal from "../addUserGroup/AddUserGroupModal";
import { getLockerGroupsByBranchId } from "../../../redux/api/branchApi";
import { getFilteredLockerGroups } from "../../../redux/slice/lockerSlice";
import OfficeName from "../../headers/OfficeName";
import GroupName from "../../headers/GroupName";
import GenerateLocker from "../../lockers/GenerateLocker";
import NoData from "../../no-data/NoData";


const AddUserModal = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();

  const [pinChecked, setPinChecked] = useState(false);
  const [qrChecked, setQrChecked] = useState(false);
  const userInfo = useSelector(getAddUserInfo);
  const [userRight, setUserRight] = useState({});
  const cardRef = useRef(null);
  const [cards, setCards] = useState([]);
  const [addGroupModalSwitch, setAddGroupModalSwitch] = useState(false);

  const branches = useSelector(getBranchesData);
  const filteredBranchGroups = useSelector(getFilteredLockerGroups)


  // TODO:  User Group
  const [sentGeneralInfo, setSentGeneralInfo] = useState({
    "isPinRequired": true,
  });

  const sendGroupInfo = (part, key, value) => {
    addAllInfoForUser(part, key, value);
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
    // addAllInfoForUserGroup('Group', 'name', selectedBranch) 
    let id = selectedOption.value
    console.log(selectedOption, 6666)
    let ids = selectedOption.map(item => item.value);
    console.log(ids, 7777)
    setSentGeneralInfo((prev) => ({
      ...prev,
      'userGroups': ids,
    }))

    dispatch(userFilter({ 'filterByGroupId': selectedOption.value }));
  };


  const addAllInfoForUser = (part, key, value) => {
    const updatedUserInfo = {
      [part]: {
        ...userInfo[part],
        [key]: value,
      },
    };
    dispatch(setAddUserInfo(updatedUserInfo));

    setUserRight((prev) => ({
      ...prev,
      ...updatedUserInfo,
    }));
  };

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

  // TODO: feat
  const addUser = () => {
    dispatch(setUserInfo(sentGeneralInfo))
      .then((response) => {
        console.log(response);
        if (response && response.payload.isSuccess) {
          onClose();
        }
      })
      .catch((error) => console.error('Error updating user info:', error));
  }
  console.log(sentGeneralInfo, 888)


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
    console.log(selectedOption.value, 999);
    dispatch(getLockerGroupsByBranchId(selectedOption.value));

  }

  //End Card part

  //PIN part
  const handePin = (value) => {
    setSentGeneralInfo((prev) => ({
      ...prev,
      'isPinRequired': value,
    }))
    sendGroupInfo('Pin', 'pin', value)
    console.log(value)
  }

  //End PIN part

  //Add Group Part
  const addGroup = () => {

  }
  //End Add Group Part

  //Branch parttt
  // TODO: feat
  // const [selectedBranch, setSelectedBranch] = useState(null);
  // const handleSelectChange = (selectedOption) => {
  //   setSelectedBranch(selectedOption);
  //   console.log("Выбранная опция:", selectedOption);
  // };
  // console.log(addGroupModalSwitch, 888)

  return (
    <div
      className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10"
    >
      {/* Test modal */}

      <div className="add__modal__content add__modal__content__addUser rounded-lg shadow-lg w-full max-w-4xl overflow-auto h-full">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Add User</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-200 text-xl"
          >
            &times;
          </button>
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
                    <input
                      placeholder="Name"
                      type="text"
                      // onChange={(e) => addAllInfoForUser('user_info', 'name', e.target.value)}
                      onChange={(e) => sendGroupInfo('user_info', 'name', e.target.value)}
                      className="w-full p-1 border rounded"
                    />
                  </div>
                  <div>
                    {/* <label className="block text-gray-300">Last name</label> */}
                    <input
                      placeholder="Last name"
                      type="text"
                      // onChange={(e) => addAllInfoForUser('user_info', 'surname', e.target.value)}
                      onChange={(e) => sendGroupInfo('user_info', 'surname', e.target.value)}

                      className="w-full p-1 border rounded"
                    />
                  </div>
                  <div>
                    {/* <label className="block text-gray-300">Email</label> */}
                    <input
                      placeholder="Email"
                      type="email"
                      // onChange={(e) => addAllInfoForUser('user_info', 'email', e.target.value)}
                      onChange={(e) => sendGroupInfo('user_info', 'email', e.target.value)}

                      className="w-full p-1 border rounded"
                    />
                  </div>
                  <div>
                    {/* <label className="block text-gray-300">Phone</label> */}
                    <input
                      placeholder="Phone"
                      type="text"
                      // onChange={(e) => addAllInfoForUser('user_info', 'phone', e.target.value)}
                      onChange={(e) => sendGroupInfo('user_info', 'phone', e.target.value)}

                      className="w-full p-1 border rounded"
                    />
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
                      // onChange={(e) => addAllInfoForUser('active_period', 'activeFrom', e.target.value)}
                      onChange={(e) => sendGroupInfo('active_period', 'activeFrom', e.target.value)}

                      className="w-full p-1 border rounded"
                    />
                  </div>
                  <div>
                    <label className="block text-gray-300">To</label>
                    <input
                      type="date"
                      // onChange={(e) => addAllInfoForUser('active_period', 'activeTo', e.target.value)}
                      onChange={(e) => sendGroupInfo('active_period', 'activeTo', e.target.value)}

                      className="w-full p-1 border rounded"
                    />
                  </div>
                </div>
              </div>

              {/* Lockers and User Group */}
              <div className="add__modal__content__part">
                <span>User group</span>
                <div className="add__modal__content__part__group mb-4 flex">
                  <div className="add__modal__group__select">
                    <CustomSelect options={userGroups} onChange={handleGroupsSelectChange} multiChoose={true} />
                  </div>
                  <div className="flex justify-center">
                    <button
                      onClick={() => setAddGroupModalSwitch(true)}
                      className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center"
                    >
                      <IoMdAdd className="fill-current mr-2" />
                      <span>Add Group</span>
                      <AddUserGroupModal isOpen={addGroupModalSwitch}
                        onClose={(e) => {
                          if (e?.stopPropagation) e.stopPropagation();
                          setAddGroupModalSwitch(false);
                        }}
                      />

                    </button>
                  </div>
                </div>
              </div>
              {/* <div className="add__modal__content__part__select">
                  <span>Lockers</span>
                  <div className="add__modal__content__part__group grid gap-4 mb-4 mr-2">
                    <div>
                      <CustomSelectTest />
                    </div>
                  </div>
                </div> */}
              {/* Credentials */}
              <div className="flex">
                <div className="add__modal__content__part w-full">
                  <span>Credentials</span>
                  <div className="add__modal__content__part__group crendentails mb-4">
                    <div className="col-span-2 flex items-center">
                      <div className="mr-2 w-full">
                        <label className="block text-gray-300">Card no.</label>
                        <input
                          type="text"
                          ref={cardRef}
                          // onChange={(e) => addAllInfoForUser('user_info', 'name', e.target.value)}
                          className="w-full p-1 border rounded"
                        />
                        {cards.length > 0 &&
                          <div>
                            <label className="block text-gray-300">Card manage</label>
                            <div className="card__manage">
                              <ul className="list-disc pl-5">
                                {cards.map((value, index) => (
                                  <li key={index} className="flex justify-between items-center mb-2">
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
                        }
                      </div>
                    </div>
                    <div className="col-span-1 mt-5 flex justify-between items-center">
                      <div className="flex">
                        <div className="generation w-full">
                          <div className="generation__checkbox flex items-center">
                            <CustomCheckbox
                              id={'pin'}
                              onChange={(checked) => handePin(checked)}
                            />
                            <span className="text-white">PIN</span>
                          </div>
                        </div>
                      </div>
                      <div className="section__add">
                        <button
                          onClick={(setCardNumbers)}
                          className="text-white font-bold rounded">
                          Add
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              {/* User Rights */}
              <div className="add__modal__content__part">
                <span>User rights</span>
                <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
                  <label className="block text-gray-300">User rights details</label>
                  <textarea
                    className="w-full p-1 border rounded h-24"
                    // value={userRight}
                    readOnly
                    defaultValue={formatUserRightText()}
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
                  className="bg-gray-600 text-white rounded "
                  onClick={addUser}
                >
                  Save
                </button>
              </div>
            </div>
          </TabPanel>
          <TabPanel>
            <div className="flex justify-between">
              <div className="add__modal__content__part__select">
                <span>Branch</span>
                <div className="add__modal__content__part__group grid gap-4 mb-4 mr-2">
                  <div>
                    <CustomSelect options={branches} onChange={handleSelectBranch} />
                  </div>
                </div>
              </div>
              <div>
                {filteredBranchGroups?.data?.length ? (
                  filteredBranchGroups.data.map((locker, index) => (
                    <React.Fragment key={index}>
                      <OfficeName name={locker.officeName} />
                      <div className='pl-5'>
                        {locker.lockers.map((lockerGroup, groupIndex) => (
                          <React.Fragment key={groupIndex}>
                            <GroupName name={lockerGroup.groupName} />
                            <div className='flex flex-wrap mb-4'>
                              {lockerGroup.groupLockers.map((item, itemIndex) => (
                                <dvi className="mr-2 mb-2">
                                  <GenerateLocker item={item} index={itemIndex} />
                                </dvi>
                              ))}
                            </div>
                          </React.Fragment>
                        ))}
                      </div>
                    </React.Fragment>
                  ))
                ) : (
                  <NoData text="No Lockers" />
                )}
              </div>
            </div>
          </TabPanel>
        </Tabs>
      </div>
    </div >
  );
};

export default AddUserModal;
