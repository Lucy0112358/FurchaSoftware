import React from "react";
import { useDispatch } from "react-redux";
import { useFormik } from "formik";
import * as Yup from "yup";
import "../modal.css";
import "./branchModal.css";
import { createBranch, updateBranch, getAllBranches } from "../../../redux/api/branchApi";
import { toast } from "react-toastify";
import CloseButton from "../attributes/CloseButton";
import ShowFormikError from "../../error/ShowFormikError";

const BranchModal = ({ onClose, mode = "add", initialData = {} }) => {
  const dispatch = useDispatch();

  const isEditMode = mode === "edit";

  const formik = useFormik({
    initialValues: {
      name: initialData.name || "",
      address: initialData.address || "",
      comment: initialData.comment || ""
    },
    validationSchema: Yup.object({
      name: Yup.string().required("Branch Name is required"),
    }),
    onSubmit: (values) => {
      const action = isEditMode
        ? updateBranch({ id: initialData.id, ...values })
        : createBranch(values);

      dispatch(action)
        .then((response) => {
          if (response?.error) {
            toast.error(response.error.message);
          } else if (response?.payload?.isSuccess) {
            toast.success(`Branch ${isEditMode ? "updated" : "created"} successfully`);
            dispatch(getAllBranches());
            onClose();
          } else {
            toast.error("Something went wrong");
          }
        });
    },
  });

  const handleSave = (e) => {
    e.preventDefault();
    formik.handleSubmit();
  };

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__addModules rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">
            {isEditMode ? "Edit branch" : "Add branch"}
          </h2>
          <CloseButton onClick={onClose}>&times;</CloseButton>
        </div>

        <form>
          <div className="add__modal__content__part">
            <span>General</span>
            <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
              <label htmlFor="name" className="block text-gray-300">
                Branch Name*
              </label>
              <div>
                <input
                  id="name"
                  name="name"
                  type="text"
                  className="w-full p-1 border rounded"
                  onChange={formik.handleChange}
                  onBlur={formik.handleBlur}
                  value={formik.values.name}
                />
                {formik.touched.name && formik.errors.name && (
                  <ShowFormikError message={formik.errors.name} />
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
                type="button"
                onClick={handleSave}
                className="bg-gray-600 text-white rounded"
              >
                {isEditMode ? "Update" : "Save"}
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
};

export default BranchModal;
