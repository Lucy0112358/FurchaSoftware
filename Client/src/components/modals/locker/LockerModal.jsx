import React, { useState } from "react";
import "../modal.css";
import { useDispatch, useSelector } from "react-redux";
import "react-tabs/style/react-tabs.css";
import CustomSelect from "../../select/CustomSelect";
import { getBranchesData } from "../../../redux/slice/menuSlice";
import "./lockerModal.css";
import { setLockerGroup } from "../../../redux/api/menuApi";
import { toast } from "react-toastify";
import CloseButton from "../attributes/CloseButton";
import { IoMdAdd } from "react-icons/io";
import BranchModal from "../branch/BranchModal";
import { useFormik } from "formik";
import * as Yup from "yup";

const LockerModal = ({ isOpen, onClose }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();
  const branches = useSelector(getBranchesData);
  const [addBranchModalSwitch, setAddBranchModalSwitch] = useState(false);

  const validationSchema = Yup.object({
    branchId: Yup.string().required("Branch is required"),
    name: Yup.string()
      .trim()
      .required("Locker Group name is required"),
  });

  const formik = useFormik({
    initialValues: {
      branchId: "",
      name: "",
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      dispatch(setLockerGroup(values))
        .then((response) => {
          if (response && response.payload.isSuccess) {
            toast.success("Locker Group created successfully");
            onClose();
          }
        })
        .catch((error) => {
          toast.error("Something went wrong");
        });
    },
  });

  const handleSelectChange = (selectedOption) => {
    formik.setFieldValue("branchId", selectedOption.value);
    console.log("Выбранная опция:", selectedOption);
  };

  const handleSave = (e) => {
    e.preventDefault();
    formik.handleSubmit();
  };

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__addLockerGroup rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">
            Create Locker Group
          </h2>
          <CloseButton onClick={onClose}>×</CloseButton>
        </div>

        <form>
          <div>
            <div className="add__modal__content__part">
              <span>Choose branches</span>
              <div className="add__modal__content__part__group gap-4 mb-4 flex">
                <div className="w-5/6">
                  <CustomSelect
                    options={branches}
                    onChange={handleSelectChange}
                    value={branches?.find(
                      (option) => option.value === formik.values.branchId
                    )}
                  />
                  {formik.touched.branchId && formik.errors.branchId && (
                    <div className="text-red-500 text-xl mt-1">
                      {formik.errors.branchId}
                    </div>
                  )}
                </div>
                <div className="flex w-1/6">
                  <button
                    type="button"
                    onClick={() => setAddBranchModalSwitch(true)}
                    className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center h-[43px]"
                  >
                    <IoMdAdd
                      className="fill-current"
                      style={{ fontSize: "xx-large" }}
                    />
                    <BranchModal
                      isOpen={addBranchModalSwitch}
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
                    name="name"
                    onChange={formik.handleChange}
                    onBlur={formik.handleBlur}
                    value={formik.values.name}
                    className="w-full p-1 border rounded"
                  />
                  {formik.touched.name && formik.errors.name && (
                    <div className="text-red-500 text-xl mt-1">
                      {formik.errors.name}
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>

          <div className="flex justify-end space-x-4">
            <div className="modal__button">
              <button
                type="button"
                className="bg-gray-600 text-white rounded"
                onClick={onClose}
              >
                Cancel
              </button>
            </div>
            <div className="modal__button">
              <button
                type="button"
                onClick={handleSave}
                className="bg-gray-600 text-white rounded"
              >
                Save
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
};

export default LockerModal;