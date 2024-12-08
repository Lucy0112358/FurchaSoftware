import React, { useEffect, useState } from "react";
import '../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import 'react-tabs/style/react-tabs.css';
import CustomSelect from "../../select/CustomSelect";
import { getBranchesData, getLockerGroups } from "../../../redux/slice/menuSlice";
import './modulesModal.css';
import { getBranches, getLockerGroupsData, setLockerGroup } from "../../../redux/api/menuApi";
import { toast } from "react-toastify";
import { IoMdAdd } from "react-icons/io";
import CustomCheckbox from "../../checkbox/CustomCheckbox";
import { filterGroupByBranch, getModuleModalBranches, getModuleModalGroupes } from "../../../redux/slice/moduleSlice";
import { addModuleFunc } from "../../../redux/api/moduleApi";
import { getLockerOptions } from "../../../enums/LockerTypes";


const ModulesModal = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null;
  const branches = useSelector(getModuleModalBranches);
  const lockerGroups = useSelector(getModuleModalGroupes)
  const [selectedOption, setSelectedOption] = useState(null);

  const lockerOptions = getLockerOptions().map((lockerType) => ({
    id: lockerType,
    name: lockerType.charAt(0).toUpperCase() + lockerType.slice(1),
  }));

  const handleLockerTypeChange = (selectedOption) => {
    sendGroupInfo('lockerType', selectedOption.value);
  };

  const dispatch = useDispatch();
  // const branches = useSelector(getBranchesData);
  // const [selectedBranch, setSelectedBranch] = useState([]);
  const [sentGeneralInfo, setSentGeneralInfo] = useState({
    lockerType: "personal",
  });
  const [addBranchModalSwitch, setAddBranchModalSwitch] = useState(false);

  useEffect(() => {
    dispatch(getBranches());
    dispatch(getLockerGroupsData());
  }, []);

  console.log(sentGeneralInfo, 8889);



  const addModules = () => {
    dispatch(addModuleFunc(sentGeneralInfo))
      .then((response) => {
        console.log(response);
        if (response && response.payload.isSuccess) {
          toast.success("Modules created successfully");
          onClose();
        }
      })
      .catch((error) => {
        toast.error("Something went wrong");
      });
  }

  const sendGroupInfo = (key, value) => {
    setSentGeneralInfo((prev) => ({
      ...prev,
      [key]: value,
    }));
  };

  const handleBranchChange = (selectedOption) => {
    sendGroupInfo('branchId', selectedOption.value);
    dispatch(filterGroupByBranch(selectedOption.value));

    console.log("Выбранная опция:", selectedOption);
  };

  const handleLockerNmbers = (value) => {

    console.log(value)
  }

  const handleLockerGroupChange = (selectedOption) => {
    sendGroupInfo('groupId', selectedOption.value);
  };

  return (
    <div
      className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10"
    >
      <div className="add__modal__content add__modal__content__addModules rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Add brain modules</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-200 text-xl"
          >
            &times;
          </button>
        </div>
        <div>
          <div className="add__modal__content__part">
            <span>General</span>
            <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
              <label className="block text-gray-300">Brain Module ID</label>
              <div>
                <input
                  type="text"
                  onChange={(e) => sendGroupInfo('macAddress', e.target.value)}
                  className="w-full p-1 border rounded"
                />
              </div>
              <label className="block text-gray-300">Branch</label>
              <div className="flex ">
                <div className="w-5/6 mr-2">
                  <CustomSelect options={branches} onChange={handleBranchChange} />
                </div>
                <div className="flex w-1/6">
                  <button
                    onClick={() => setAddBranchModalSwitch(!addBranchModalSwitch)}
                    className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center"
                  >
                    <IoMdAdd className="fill-current" style={{ fontSize: 'xx-large' }} />
                    {/* <AddUserGroupModal isOpen={addGroupModalSwitch} onClose={() => setAddGroupModalSwitch(false)} /> */}

                  </button>
                </div>
              </div>
              <div className="section__add">
                <button
                  className="text-white font-bold rounded"
                // onClick={() => addBranchHandle()}
                >
                  Add
                </button>
              </div>
            </div>
          </div>
          <div className="add__modal__content__part">
            <span>Specify</span>
            <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
              <label className="block text-gray-300">Locker group(optional)</label>
              <div className="flex ">
                <div className="w-5/6 mr-2">
                  <CustomSelect options={lockerGroups} onChange={handleLockerGroupChange} />
                </div>
                <div className="flex w-1/6">
                  <button
                    onClick={() => setAddBranchModalSwitch(!addBranchModalSwitch)}
                    className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center"
                  >
                    <IoMdAdd className="fill-current" style={{ fontSize: 'xx-large' }} />
                    {/* <AddUserGroupModal isOpen={addGroupModalSwitch} onClose={() => setAddGroupModalSwitch(false)} /> */}

                  </button>
                </div>
              </div>
              <label className="block text-gray-300">Locker type(optional)</label>
              <div className="flex ">
                <div className="w-5/6 mr-2">
                  <CustomSelect options={lockerOptions} onChange={handleLockerTypeChange} />
                  "personal"
                </div>
              </div>
              <label className="block text-gray-300">Locker numbers</label>
              <div className="flex">
                <div className="generation w-full">
                  <div className="generation__checkbox flex items-center">
                    <CustomCheckbox
                      id={'numbers'}
                    // onChange={(checked) => handePin(checked)}
                    />
                    <span className="text-gray-800">Begin from last locker no, in the group</span>
                  </div>
                </div>
              </div>
              <div className="flex items-center space-x-2">
                <input
                  type="number"
                  placeholder="1"
                  onChange={(e) => sendGroupInfo('firstLocker', e.target.value)}
                  className="w-20 border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
                <span className="text-gray-800">to</span>
                <input
                  type="number"
                  placeholder="256"
                  onChange={(e) => sendGroupInfo('lastLocker', e.target.value)}
                  className="w-20 border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>
          </div>
        </div>
        <div className="flex justify-end space-x-4 m-5 ">
          <div className="modal__button">
            <button className="bg-gray-600 text-white rounded "
              onClick={onClose}>
              Cancel
            </button>
          </div>
          <div className="modal__button">
            <button
              className="bg-gray-600 text-white rounded"
              onClick={addModules}
            >
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ModulesModal;
