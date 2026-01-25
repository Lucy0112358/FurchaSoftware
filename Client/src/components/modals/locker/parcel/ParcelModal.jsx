import React, { useState } from "react";
import "../../modal.css";
import { useDispatch, useSelector } from "react-redux";
import "react-tabs/style/react-tabs.css";
import { getBranchesData } from "../../../../redux/slice/menuSlice";
import "./parcelModal.css";
import { setLockerGroup } from "../../../../redux/api/menuApi";
import { toast } from "react-toastify";
import CloseButton from "../../attributes/CloseButton";
import { IoMdAdd } from "react-icons/io";
import BranchModal from "../../branch/BranchModal";
import { useFormik } from "formik";
import * as Yup from "yup";
import CustomSelect from "../../../select/CustomSelect";
import ShowFormikError from "../../../error/ShowFormikError";
import { getAllBranchesData } from "../../../../redux/slice/branchSlice";
import { LockerSizes } from "../../../../enums/Locker/Sizes";

const ParcelModal = ({ onClose }) => {
  const dispatch = useDispatch();
  const branches = useSelector(getAllBranchesData);

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
    <div className="add__modal fixed inset-0  bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__addParcelLocker rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">
            Store Parcel
          </h2>
          <CloseButton onClick={onClose}>×</CloseButton>
        </div>

        <form>
          <div>
            <div className="add__modal__content__part w-full">
              <span>General</span>
              <div className="add__modal__content__part__group  mb-4 flex flex-col">
                <div className="add__modal__content__part">
                  <div className="add__modal__content__part__group  mb-4 flex flex-col">
                    <div>
                      <input
                        type="text"
                        placeholder="Order no."
                        className="w-full p-1  border rounded  text-white"
                      />
                      <div className="flex justify-between mt-4">
                        <div className="flex items-center gap-4 mt-4 mb-4">
                          <label className="flex items-center gap-2">
                            <input type="radio" name="parcelType" className="accent-blue-500" />
                            Send
                          </label>
                          <label className="flex items-center gap-2">
                            <input type="radio" name="parcelType" className="accent-blue-500" />
                            Receive
                          </label>
                        </div>
                        <div className="flex center">
                          <label className="block m-2">Locker Size</label>
                          <CustomSelect
                            options={LockerSizes?.map((size) => ({
                              label: size,
                              value: size,
                            }))}
                          // onChange={handleSelectChange}
                          // value={branches?.find(
                          //   (option) => option.value === formik.values.branchId
                          // )}
                          />
                          {/* {formik.touched.branchId && formik.errors.branchId && (
                            <ShowFormikError message={formik.errors.branchId} />
                          )} */}
                        </div>
                      </div>

                    </div>

                    <div className="mb-4">
                      <input
                        placeholder="Customer Name"
                        type="text"
                        value={''}
                        // onChange={(e) =>
                        //   sendGroupInfo('user_info', 'name', e.target.value)
                        // }
                        className="w-full p-1 border rounded"
                      />
                    </div>
                    <div className="flex">
                      <input
                        placeholder="Email"
                        type="email"
                        value={''}
                        // onChange={(e) =>
                        //   sendGroupInfo('user_info', 'email', e.target.value)
                        // }
                        className="w-3/6 p-1 border rounded mr-1"
                      />
                      <input
                        placeholder="Phone"
                        type="text"
                        value={''}
                        // onChange={(e) =>
                        //   sendGroupInfo('user_info', 'phone', e.target.value)
                        // }
                        className="w-3/6 p-1 border rounded ml-1"
                      />
                    </div>

                  </div>
                </div>
                <div className="add__modal__content__part">
                  <span>Valid to</span>
                  <div className="add__modal__content__part__group grid grid-cols-2 gap-4 mb-4">
                    <div>
                      <label className="block text-gray-300">From</label>
                      <input
                        type="date"
                        value={''}
                        // onChange={(e) =>
                        //   sendGroupInfo('active_period', 'activeFrom', e.target.value)
                        // }
                        className="w-full p-1 border rounded"
                      />
                    </div>
                    <div>
                      <label className="block text-gray-300">To</label>
                      <input
                        type="date"
                        value={''}
                        // onChange={(e) =>
                        //   sendGroupInfo('active_period', 'activeTo', e.target.value)
                        // }
                        className="w-full p-1 border rounded"
                      />
                    </div>
                  </div>
                </div>
                <div className="add__modal__content__part">
                  <span>Message</span>
                  <div className="add__modal__content__part__group  gap-4 mb-4">
                    {/* MESSAGE SECTION */}
                    <div className="add__modal__content__part">
                      <div className="mb-4">
                        <CustomSelect
                            options={LockerSizes?.map((size) => ({
                              label: size,
                              value: size,
                            }))}
                          // onChange={handleSelectChange}
                          // value={branches?.find(
                          //   (option) => option.value === formik.values.branchId
                          // )}
                          />
                      </div>

                      <div>
                        <textarea
                          rows="5"
                          placeholder="Write message..."
                          className="w-full p-2 border  rounded  text-white resize-none"
                        ></textarea>
                      </div>
                    </div>
                  </div>
                </div>


              </div>
            </div>

          </div>

          <div className="flex justify-end space-x-4">
            <div className="modal__button">
              <button
                type="button"
                className=" text-white rounded"
                onClick={onClose}
              >
                Cancel
              </button>
            </div>
            <div className="modal__button">
              <button
                type="button"
                onClick={handleSave}
                className=" text-white rounded"
              >
                Save
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
    //  <div className="add__modal fixed inset-0  bg-opacity-50 flex mt-2 justify-center z-10">
    //   <div className="add__modal__content add__modal__content__storeParcel rounded-lg shadow-lg w-full max-w-4xl overflow-auto bg-gray-700 text-white p-6">
    //     {/* Header */}
    //     <div className="flex justify-between items-center mb-4">
    //       <h2 className="text-2xl font-semibold">Store Parcel</h2>
    //       <button
    //         onClick={onClose}
    //         className="text-3xl font-bold hover:text-gray-400"
    //       >
    //         ×
    //       </button>
    //     </div>

    //     {/* Form */}
    //     <form className="space-y-6">
    //       {/* GENERAL SECTION */}
    //       <div className="add__modal__content__part">
    //         <span className="block text-lg font-semibold mb-2">General</span>

    //         <div className="grid grid-cols-2 gap-4 mb-4">
    //           <div>
    //             <label className="block text-sm mb-1">Order no.</label>
    //             <input
    //               type="text"
    //               placeholder="Enter order number"
    //               className="w-full p-2 border  rounded  text-white"
    //             />
    //           </div>

    //           <div>
    //             <label className="block text-sm mb-1">Valid to</label>
    //             <div className="flex gap-2">
    //               <input
    //                 type="date"
    //                 className="flex-1 p-2 border  rounded  text-white"
    //               />
    //               <input
    //                 type="time"
    //                 className="flex-1 p-2 border  rounded  text-white"
    //               />
    //             </div>
    //           </div>
    //         </div>

    //         <div className="flex items-center gap-4 mb-4">
    //           <label className="flex items-center gap-2">
    //             <input type="radio" name="parcelType" className="accent-blue-500" />
    //             Send
    //           </label>
    //           <label className="flex items-center gap-2">
    //             <input type="radio" name="parcelType" className="accent-blue-500" />
    //             Receive
    //           </label>
    //         </div>

    //         <div className="mb-4">
    //           <label className="block text-sm mb-1">Customer Name</label>
    //           <input
    //             type="text"
    //             placeholder="Customer name"
    //             className="w-full p-2 border  rounded  text-white"
    //           />
    //         </div>

    //         <div className="grid grid-cols-2 gap-4 mb-4">
    //           <div>
    //             <label className="block text-sm mb-1">Email</label>
    //             <input
    //               type="email"
    //               placeholder="example@mail.com"
    //               className="w-full p-2 border  rounded  text-white"
    //             />
    //           </div>
    //           <div>
    //             <label className="block text-sm mb-1">Phone</label>
    //             <input
    //               type="tel"
    //               placeholder="+123456789"
    //               className="w-full p-2 border  rounded  text-white"
    //             />
    //           </div>
    //         </div>
    //       </div>

    //       {/* LOCKER SECTION */}
    //       <div className="add__modal__content__part">
    //         <span className="block text-lg font-semibold mb-2">Locker</span>
    //         <div className="flex items-center gap-4">
    //           <select className="w-full p-2 border  rounded  text-white">
    //             <option>Select locker size</option>
    //             <option>Small</option>
    //             <option>Medium</option>
    //             <option>Large</option>
    //           </select>
    //           <button
    //             type="button"
    //             className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-3 rounded inline-flex items-center h-[43px]"
    //           >
    //             <IoMdAdd className="text-2xl" />
    //           </button>
    //         </div>
    //       </div>

    //       {/* MESSAGE SECTION */}
    //       <div className="add__modal__content__part">
    //         <span className="block text-lg font-semibold mb-2">Message</span>
    //         <div className="mb-4">
    //           <label className="block text-sm mb-1">Message template</label>
    //           <select className="w-full p-2 border  rounded  text-white">
    //             <option>Choose template</option>
    //             <option>Template 1</option>
    //             <option>Template 2</option>
    //           </select>
    //         </div>

    //         <div>
    //           <label className="block text-sm mb-1">Message</label>
    //           <textarea
    //             rows="5"
    //             placeholder="Write message..."
    //             className="w-full p-2 border  rounded  text-white resize-none"
    //           ></textarea>
    //         </div>
    //       </div>

    //       {/* ACTION BUTTONS */}
    //       <div className="flex justify-end space-x-4">
    //         <button
    //           type="button"
    //           className=" text-white px-5 py-2 rounded hover:bg-gray-500"
    //           onClick={onClose}
    //         >
    //           Cancel
    //         </button>
    //         <button
    //           type="button"
    //           onClick={handleSave}
    //           className="bg-blue-600 text-white px-5 py-2 rounded hover:bg-blue-500"
    //         >
    //           Save
    //         </button>
    //       </div>
    //     </form>
    //   </div>
    // </div>
  );
};

export default ParcelModal;