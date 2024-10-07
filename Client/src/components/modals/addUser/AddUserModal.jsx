import React, { useState } from "react";
import './addUser.css';
import '../modal.css';
import CustomSelectTest from "../../select/CustomSelectTest";
import CustomCheckbox from "../../checkbox/CustomCheckbox";
import { useDispatch, useSelector } from 'react-redux';
import { getAddUserInfo, setAddUserInfo } from "../../../redux/slice/userSlice";
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import 'react-tabs/style/react-tabs.css';
import CustomSelect from "../../select/CustomSelect";
import { getUserGroupsData } from "../../../redux/slice/menuSlice";
import { userFilter } from "../../../redux/api/menuApi";
import { setUserInfo } from "../../../redux/api/userApi";


const AddUserModal = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();

  const [pinChecked, setPinChecked] = useState(false);
  const [qrChecked, setQrChecked] = useState(false);
  const userInfo = useSelector(getAddUserInfo);
  const [userRight, setUserRight] = useState({});

  // TODO:  User Group
  const [sentGeneralInfo, setSentGeneralInfo] = useState({
   "cards": [
    "8938559966",
    "15641561",
  ], 
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
      'userGroups' : ids,
    }))
    
    dispatch(userFilter({'filterByGroupId': selectedOption.value}));

    console.log("Выбранная groups:", selectedOption);
  };


  console.log(userInfo, "userInfo!!!!!!!!!")

  const addAllInfoForUser = (part, key, value) => {
    const updatedUserInfo = {
      [part]: {
        ...userInfo[part],
        [key]: value,
      },
    };
    console.log(updatedUserInfo, 9999)
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
          ([key, value]) => `${key}: ${value}`
        );
        return `${part}:\n${valuesEntries.join('\n')}`;
      })
      .join('\n\n');
  };

  // TODO: feat
  const addUser = () =>{
    dispatch(setUserInfo(sentGeneralInfo))
    console.log(sentGeneralInfo,888)
  }

  //Branch parttt
  // TODO: feat
  // const [selectedBranch, setSelectedBranch] = useState(null);
  // const handleSelectChange = (selectedOption) => {
  //   setSelectedBranch(selectedOption);
  //   console.log("Выбранная опция:", selectedOption);
  // };


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
              <div className="flex justify-around">
                {/* <div className="add__modal__content__part__select">
                  <span>Lockers</span>
                  <div className="add__modal__content__part__group grid gap-4 mb-4 mr-2">
                    <div>
                      <CustomSelectTest />
                    </div>
                  </div>
                </div> */}
                <div className="add__modal__content__part__select">
                  <span>User group</span>
                  <div className="add__modal__content__part__group grid gap-4 mb-4 ml-2">
                    <div>
                      {/* <label className="block text-gray-300">User group</label> */}
                      <CustomSelect options={userGroups}  onChange={handleGroupsSelectChange} multiChoose={true}/>
                    </div>
                  </div>
                </div>
              </div>
              {/* Credentials */}
              <div className="flex">
                <div className="add__modal__content__part">
                  <span>Credentials</span>
                  <div className="add__modal__content__part__group crendentails grid grid-cols-3 gap-4 mb-4">
                    <div className="col-span-2 flex items-center">
                      <div className="mr-2 w-full">
                        <label className="block text-gray-300">Card no.</label>
                        <textarea
                          className="w-full p-1 border rounded h-24"
                          placeholder="Existing card"
                        ></textarea>
                      </div>
                    </div>
                    <div className="col-span-1 mt-5">
                      <div className="flex">
                        <div className="generation w-full">
                          <div className="generation__checkbox flex items-center">
                            <CustomCheckbox id={'pin'} />
                            <span className="text-white">Pin</span>
                          </div>
                        </div>
                      </div>
                      <div className="section__add">
                        <button className="text-white font-bold rounded">
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
                <span>Lockers</span>
                <div className="add__modal__content__part__group grid gap-4 mb-4 mr-2">
                  <div>
                    <CustomSelectTest />
                  </div>
                </div>
              </div>
            </div>
          </TabPanel>
        </Tabs>
      </div>
    </div>
  );
};

export default AddUserModal;
