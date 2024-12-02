import React, { useState } from "react";
import '../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import 'react-tabs/style/react-tabs.css';
import CustomSelect from "../../select/CustomSelect";
import { getBranchesData } from "../../../redux/slice/menuSlice";
import './lockerModal.css';
import { setLockerGroup } from "../../../redux/api/menuApi";
import { toast } from "react-toastify";


const LockerModal = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null;
  const branches = useSelector(getBranchesData);

  const dispatch = useDispatch();
  // const branches = useSelector(getBranchesData);
  // const [selectedBranch, setSelectedBranch] = useState([]);
  const [sentGeneralInfo, setSentGeneralInfo] = useState({});

  const addLockerGroup = () => {
    dispatch(setLockerGroup(sentGeneralInfo))
    .then((response) => {
      console.log(response);
      if (response && response.payload.isSuccess) {
        toast.success("Locker Group created successfully");
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
  const handleSelectChange = (selectedOption) => {
    sendGroupInfo('branchId', selectedOption.value);
    console.log("Выбранная опция:", selectedOption);
  };

  return (
    <div
      className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10"
    >
      <div className="add__modal__content add__modal__content__addLockerGroup rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Create Locker Group</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-200 text-xl"
          >
            &times;
          </button>
        </div>
        <div>
        <div className="add__modal__content__part">
            <span>Choose branches</span>
            <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
              <div>
                <CustomSelect options={branches} onChange={handleSelectChange} />
              </div>
              {/* <div className="section__add">
                <button 
                  className="text-white font-bold rounded"
                  // onClick={() => addBranchHandle()}
                >
                  Add
                </button>
              </div> */}
            </div>
          </div>

          <div className="add__modal__content__part">
            <span>Locker Group Name</span>
            <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
              <div>
                <input
                  placeholder="Group Name"
                  type="text"
                  onChange={(e) => sendGroupInfo('name', e.target.value)}
                  className="w-full p-1 border rounded"
                />
              </div>
            </div>
          </div>
        </div>
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
              onClick={addLockerGroup}
            >
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LockerModal;
