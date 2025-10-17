import React, { useEffect } from "react";
import "../modal.css";
import { useDispatch, useSelector } from "react-redux";
import "react-tabs/style/react-tabs.css";
import "./lockerGroupModal.css";
import { setLockerGroup } from "../../../redux/api/menuApi";
import { toast } from "react-toastify";
import CloseButton from "../attributes/CloseButton";
import { useFormik } from "formik";
import * as Yup from "yup";
import ShowFormikError from "../../error/ShowFormikError";
import { editLockerGroup, getLockerGroup } from "../../../redux/api/lockerGroupApi";
import { getLockerGroupData } from "../../../redux/slice/lockerGroupSlice";
import { getLockers, getParcelLockers } from "../../../redux/api/lockerApi";

const LockerGroupModal = ({ onClose, id, type }) => {
  const dispatch = useDispatch();
  const lockerGroupData = useSelector(getLockerGroupData);

  useEffect(() => {
    if (id) {
      dispatch(getLockerGroup(id));
    }
  }, [id, dispatch]);

  const validationSchema = Yup.object({
    name: Yup.string().trim().required("Locker Group name is required"),
  });

  const formik = useFormik({
    initialValues: {
      name: lockerGroupData?.name || "",
    },
    enableReinitialize: true,
    validationSchema,
    onSubmit: (values) => {
      dispatch(editLockerGroup({ id, data: values }))
        .then((response) => {
          if (response && response.payload.isSuccess) {
            toast.success("Locker Group edited successfully");
            if (type === "allLocker") {
              dispatch(getLockers())
            } else if(type === "parcelLocker") {
              dispatch(getParcelLockers())
            }
            onClose();
          }
        })
        .catch(() => {
          toast.error("Something went wrong");
        });
    },
  });

  const handleSave = (e) => {
    e.preventDefault();
    formik.handleSubmit();
  };

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__editLockerGroup rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">
            Edit Locker Group
          </h2>
          <CloseButton onClick={onClose}>×</CloseButton>
        </div>

        <form>
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
                  <ShowFormikError message={formik.errors.name} />
                )}
              </div>
            </div>
          </div>

          <div className="flex justify-end space-x-4">
            <div className="modal__button">
              <button
                type="button"
                className="bg-gray-600 text-white rounded px-4 py-1"
                onClick={onClose}
              >
                Cancel
              </button>
            </div>
            <div className="modal__button">
              <button
                type="button"
                onClick={handleSave}
                className="bg-gray-600 text-white rounded px-4 py-1"
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

export default LockerGroupModal;
