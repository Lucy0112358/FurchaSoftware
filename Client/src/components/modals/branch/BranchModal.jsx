import React from "react";
import { useDispatch } from "react-redux";
import { useFormik } from "formik";
import * as Yup from "yup";
import "../modal.css";
import "./branchModal.css";
import { createBranch, getAllBranches } from "../../../redux/api/branchApi";
import { toast } from "react-toastify";
import CloseButton from "../attributes/CloseButton";

const BranchModal = ({ isOpen, onClose }) => {
  if (!isOpen) return null;
  const dispatch = useDispatch();
  const formik = useFormik({
    initialValues: {
      branchName: "",
      address: "",
      comment: ""
    },
    validationSchema: Yup.object({
      branchName: Yup.string().required("Brain Name is required")
    }),
    onSubmit: (values) => {
      dispatch(createBranch(values))
        .then((response) => {
          if (response && response.error) {
            toast.error(response.error.message);
          } else if (response && response.payload.isSuccess) {
            toast.success("Branch created successfully");
            dispatch(getAllBranches());
            onClose();
          } else {
            toast.error("Something went wrong");
          }
        })
    },
  });

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__addModules rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Add branch</h2>
          <CloseButton onClick={onClose}>
            &times;
          </CloseButton>
        </div>

        <form onSubmit={formik.handleSubmit}>
          <div className="add__modal__content__part">
            <span>General</span>
            <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
              <label htmlFor="branchName" className="block text-gray-300">
                Branch Name*
              </label>
              <div>
                <input
                  id="branchName"
                  name="branchName"
                  type="text"
                  className="w-full p-1 border rounded"
                  onChange={formik.handleChange}
                  onBlur={formik.handleBlur}
                  value={formik.values.branchName}
                />
                {formik.touched.branchName && formik.errors.branchName && (
                  <div className="text-red-600 text-xl mt-1">
                    {formik.errors.branchName}
                  </div>
                )}
              </div>

              <label htmlFor="address" className="block text-gray-300">
                Address
              </label>
              <div>
                <input
                  id="address"
                  name="address"
                  type="text"
                  className="w-full p-1 border rounded"
                  onChange={formik.handleChange}
                  onBlur={formik.handleBlur}
                  value={formik.values.address}
                />
              </div>

              <label htmlFor="comment" className="block text-gray-300">
                Comment
              </label>
              <div>
                <textarea
                  id="comment"
                  name="comment"
                  className="w-full p-1 border rounded h-24"
                  onChange={formik.handleChange}
                  onBlur={formik.handleBlur}
                  value={formik.values.comment}
                />
              </div>
            </div>
          </div>

          <div className="flex justify-end space-x-4 m-5">
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
                type="submit"
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

export default BranchModal;
