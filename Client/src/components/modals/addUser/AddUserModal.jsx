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
import { toast } from "react-toastify";
import CloseButton from "../attributes/CloseButton";

const AddUserModal = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();

  const [pinChecked, setPinChecked] = useState(false);
  const [qrChecked, setQrChecked] = useState(false);
  const userInfo = useSelector(getAddUserInfo);

  // Здесь храним локальные ошибки по каждому из полей
  const [formErrors, setFormErrors] = useState({});

  const cardRef = useRef(null);
  const [cards, setCards] = useState([]);
  const [addGroupModalSwitch, setAddGroupModalSwitch] = useState(false);

  const branches = useSelector(getBranchesData);
  const filteredBranchGroups = useSelector(getFilteredLockerGroups);
  const userGroups = useSelector(getUserGroupsData);

  const [selectedBranches, setSelectedBranches] = useState([]);
  const [selectedGroups, setSelectedGroups] = useState(null);
  const [selectedLockerId, setSelectedLockerId] = useState([]);

  const [userRight, setUserRight] = useState({});

  // Все данные, которые будем отправлять на сервер
  const [sentGeneralInfo, setSentGeneralInfo] = useState({
    isPinRequired: true,
    // При желании можно сразу проинициализировать поля:
    // name: '',
    // surname: '',
    // email: '',
    // phone: '',
    // activeFrom: '',
    // activeTo: '',
    // userGroups: [],
    // cards: []
  });

  // Универсальная функция для записи значений в наше состояние и userInfo
  const sendGroupInfo = (part, key, value) => {
    // Обновляем Redux (не обязательно, если не планируется использование userInfo)
    dispatch(
      setAddUserInfo({
        [part]: {
          ...userInfo[part],
          [key]: value,
        },
      })
    );

    // Обновляем локальный state, который будем потом отправлять
    setSentGeneralInfo((prev) => ({
      ...prev,
      [key]: value,
    }));

    // Дополнительно формируем текстовое описание полей (userRight) – если нужно
    setUserRight((prev) => ({
      ...prev,
      [part]: {
        ...(prev[part] || {}),
        [key]: value,
      },
    }));
  };

  // Формируем строковое представление текущих прав пользователя
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

  // ======================================
  // Валидация формы
  // ======================================
  const validateForm = () => {
    const errors = {};

    // Проверяем имя
    if (!sentGeneralInfo.name || !sentGeneralInfo.name.trim()) {
      errors.name = 'Name is required';
    }

    // Проверяем фамилию
    if (!sentGeneralInfo.surname || !sentGeneralInfo.surname.trim()) {
      errors.surname = 'Surname is required';
    }

    // Проверяем email
    if (!sentGeneralInfo.email || !sentGeneralInfo.email.trim()) {
      errors.email = 'Email is required';
    } else {
      // Простейшая проверка формата email
      const emailRegex = /\S+@\S+\.\S+/;
      if (!emailRegex.test(sentGeneralInfo.email)) {
        errors.email = 'Invalid email format';
      }
    }

    // Проверяем телефон
    if (!sentGeneralInfo.phone || !sentGeneralInfo.phone.trim()) {
      errors.phone = 'Phone is required';
    }

    // Проверяем период активности
    if (!sentGeneralInfo.activeFrom) {
      errors.activeFrom = 'Active From date is required';
    }
    if (!sentGeneralInfo.activeTo) {
      errors.activeTo = 'Active To date is required';
    }

    // Проверяем, выбраны ли группы
    if (!sentGeneralInfo.userGroups || !sentGeneralInfo.userGroups.length) {
      errors.userGroups = 'At least one User Group must be selected';
    }

    // Если нужно, можно проверять наличие карт или других полей

    return errors;
  };

  const addUser = () => {
    const errors = validateForm();
    setFormErrors(errors);

    if (Object.keys(errors).length > 0) {
      toast.error('Please fill out the form correctly');
      return;
    }

    dispatch(setUserInfo(sentGeneralInfo))
      .then((response) => {
        if (response && response.payload?.isSuccess) {
          onClose();
        } else {
          toast.error(response.error?.message || 'Error occurred');
        }
      })
      .catch((error) => console.error('Error updating user info:', error));
  };

  // Работа с картами
  const setCardNumbers = () => {
    const newCard = cardRef.current.value.trim();

    if (newCard) {
      // Добавляем карту в список
      const addNewCardToCards = [...cards, newCard];
      setCards(addNewCardToCards);
      cardRef.current.value = null;

      // Запоминаем их в общем объекте
      setSentGeneralInfo((prev) => ({
        ...prev,
        cards: addNewCardToCards,
      }));
      sendGroupInfo('Card No', 'cards', addNewCardToCards);
    }
  };

  const handleDeleteCards = (value) => {
    const withoutDeletedCards = cards.filter((card) => card !== value);
    setCards(withoutDeletedCards);

    setSentGeneralInfo((prev) => ({
      ...prev,
      cards: withoutDeletedCards,
    }));
    sendGroupInfo('Card No', 'cards', withoutDeletedCards);
  };

  // Выбор групп
  const handleGroupsSelectChange = (selectedOption) => {
    setSelectedGroups(selectedOption);
    // Отправляем в общий объект ID выбранных групп
    const ids = selectedOption.map((item) => item.value);

    setSentGeneralInfo((prev) => ({
      ...prev,
      userGroups: ids,
    }));
    sendGroupInfo('User Groups', 'userGroups', ids);

    // Дополнительно можно вызвать userFilter
    dispatch(userFilter({ filterByGroupId: selectedOption.value }));
  };

  // Выбор филиалов
  const handleSelectBranch = (selectedOption) => {
    setSelectedBranches((prevSelected) => {
      const added = selectedOption.filter((item) => !prevSelected.includes(item));
      if (added.length > 0) {
        dispatch(getLockerGroupsByBranchId(added[0].value));
      }
      return selectedOption;
    });
  };

  // PIN
  const handePin = (value) => {
    setSentGeneralInfo((prev) => ({
      ...prev,
      isPinRequired: value,
    }));
    sendGroupInfo('Pin', 'pin', value);
  };

  // Локеры
  const handleBranchSelect = (itemId) => {
    setSelectedLockerId((prevSelected) => {
      if (!prevSelected.includes(itemId)) {
        return [...prevSelected, itemId];
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
    // Отправляем lockerIds
    sendGroupInfo('Locker', 'lockerIds', selectedLockerId);
    // Заодно запоминаем branchIds
    const branchIds = selectedBranches.map((branch) => branch.value);
    setSentGeneralInfo((prev) => ({
      ...prev,
      branchIds: branchIds,
    }));
    toast.success("Lockers added successfully");
  };

  // Вывод ошибки под конкретным инпутом
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

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__addUser rounded-lg shadow-lg w-full max-w-4xl overflow-auto h-full">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Add User</h2>
          <CloseButton onClick={onClose}>
            &times;
          </CloseButton>
        </div>

        <Tabs>
          <TabList>
            <Tab>Info</Tab>
            <Tab>Lockers</Tab>
          </TabList>

          {/* ====================== ПЕРВАЯ ВКЛАДКА ====================== */}
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
                <div className="add__modal__content__part__group mb-4 flex items-center">
                  <div className="add__modal__group__select mr-4 w-full">
                    <CustomSelect
                      options={userGroups}
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
                      <IoMdAdd className="fill-current mr-2" />
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
                        <label className="block text-gray-300">Card no.</label>
                        <input
                          type="text"
                          ref={cardRef}
                          className="w-full p-1 border rounded"
                        />
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
                              onChange={(checked) => handePin(checked)}
                            />
                            <span className="text-white">PIN</span>
                          </div>
                        </div>
                      </div>
                      <div className="section__add">
                        <button
                          onClick={setCardNumbers}
                          className="text-white font-bold rounded"
                        >
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

            {/* Кнопки */}
            <div className="flex justify-end space-x-4">
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
            </div>
          </TabPanel>

          <TabPanel>
            <div className="flex justify-between flex-col">
              <div className="add__modal__content__part__select">
                <span>Branch</span>
                <div className="add__modal__content__part__group grid gap-4 mb-4 mr-2">
                  <div>
                    <CustomSelect
                      options={branches}
                      onChange={handleSelectBranch}
                      multiChoose={true}
                    />
                    {/* NEW (branch required) */}
                    {renderError('branch')}
                  </div>
                </div>
              </div>

              <div>
                {filteredBranchGroups?.length ? (
                  <div className="pl-5">
                    {filteredBranchGroups.map((lockerGroup, groupIndex) => (
                      <React.Fragment key={groupIndex}>
                        <GroupName name={lockerGroup.groupName} />
                        <div className="flex flex-wrap mb-4">
                          {lockerGroup.groupLockers.map((item, itemIndex) => (
                            <div
                              key={itemIndex}
                              className={`mr-2 mb-2 select-none ${
                                selectedLockerId.includes(item.id)
                                  ? 'selected__branch__id'
                                  : ''
                              }`}
                              onMouseOver={(event) => {
                                if (event.buttons === 1) {
                                  handleBranchSelect(item.id);
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

export default AddUserModal;
