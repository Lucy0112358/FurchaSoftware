import React, { useState } from "react";
import '../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import 'react-tabs/style/react-tabs.css';
import CustomSelect from "../../select/CustomSelect";
import { getBranchesData } from "../../../redux/slice/menuSlice";
import './lockerModal.css';
import { setLockerGroup } from "../../../redux/api/menuApi";
import { toast } from "react-toastify";
import CloseButton from "../attributes/CloseButton";
import { IoMdAdd } from "react-icons/io";
import BranchModal from "../branch/BranchModal";


const LockerModal = ({ isOpen, onClose }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();
  const branches = useSelector(getBranchesData);
  const [sentGeneralInfo, setSentGeneralInfo] = useState({});
  const [formErrors, setFormErrors] = useState({});
  const [addBranchModalSwitch, setAddBranchModalSwitch] = useState(false);

  const sendGroupInfo = (key, value) => {
    setSentGeneralInfo((prev) => ({
      ...prev,
      [key]: value,
    }));
  };

  const validateForm = () => {
    const errors = {};
    if (!sentGeneralInfo.branchId) {
      errors.branchId = 'Branch is required';
    }
    if (!sentGeneralInfo.name || !sentGeneralInfo.name.trim()) {
      errors.name = 'Locker Group name is required';
    }
    return errors;
  };

  const renderError = (fieldName) => {
    if (formErrors[fieldName]) {
      return <div className="text-red-500 text-xl mt-1">{formErrors[fieldName]}</div>;
    }
    return null;
  };

  const addLockerGroup = () => {
    const errors = validateForm();
    setFormErrors(errors);

    if (Object.keys(errors).length > 0) {
      toast.error("Please fill in required fields");
      return;
    }

    dispatch(setLockerGroup(sentGeneralInfo))
      .then((response) => {
        if (response && response.payload.isSuccess) {
          toast.success("Locker Group created successfully");
          onClose();
        }
      })
      .catch((error) => {
        toast.error("Something went wrong");
      });
  };

  const handleSelectChange = (selectedOption) => {
    sendGroupInfo('branchId', selectedOption.value);
    console.log("Выбранная опция:", selectedOption);
  };

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__addLockerGroup rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Create Locker Group</h2>
          <CloseButton onClick={onClose}>
            &times;
          </CloseButton>
        </div>

        <div>
          <div className="add__modal__content__part">
            <span>Choose branches</span>
            <div className="add__modal__content__part__group  gap-4 mb-4 flex">
              <div className="w-5/6">
                <CustomSelect
                  options={branches}
                  onChange={handleSelectChange}
                />
                {renderError('branchId')}
              </div>
              <div className="flex w-1/6">
                <button
                  onClick={() => setAddBranchModalSwitch(true)}
                  className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center h-[43px]"
                >
                  <IoMdAdd className="fill-current" style={{ fontSize: 'xx-large' }} />
                  <BranchModal isOpen={addBranchModalSwitch}
                    onClose={(e) => {
                      if (e?.stopPropagation) e.stopPropagation();
                      setAddBranchModalSwitch(false);
                    }}
                  />
                </button>
              </div>
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
                {renderError('name')}
              </div>
            </div>
          </div>
        </div>

        <div className="flex justify-end space-x-4">
          <div className="modal__button">
            <button
              className="bg-gray-600 text-white rounded"
              onClick={onClose}
            >
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