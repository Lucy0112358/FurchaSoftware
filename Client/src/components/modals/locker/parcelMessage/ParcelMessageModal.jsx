import React, { useEffect, useState } from "react";
import "../../modal.css";
import { useDispatch, useSelector } from "react-redux";
import "react-tabs/style/react-tabs.css";
import { toast } from "react-toastify";
import CloseButton from "../../attributes/CloseButton";
import { CiEdit } from "react-icons/ci";
import { FaRegClone } from "react-icons/fa6";
import { useFormik } from "formik";
import * as Yup from "yup";

import { getLockerMessages } from "../../../../redux/slice/lockerSlice";
import {
  getParcelMessages,
  createMessage,
  updateMessage,
} from "../../../../redux/api/lockerApi";
import ShowFormikError from "../../../error/ShowFormikError";

const ParcelMessageModal = ({ onClose }) => {
  const dispatch = useDispatch();
  const messages = useSelector(getLockerMessages);
  const [isEditMode, setIsEditMode] = useState(false);

  useEffect(() => {
    dispatch(getParcelMessages());
  }, [dispatch]);

  const validationSchema = Yup.object({
    title: Yup.string().trim().required("Title is required"),
    content: Yup.string().trim().required("Content is required"),
    parcelType: Yup.number().required(),
  });

  const formik = useFormik({
    initialValues: {
      id: null,
      title: "",
      content: "",
      parcelType: 1,
    },
    validationSchema,
    onSubmit: async (values, { resetForm }) => {
      try {
        const payload = { ...values };
        const action = values.id ? updateMessage(payload) : createMessage(payload);
        const response = await dispatch(action);

        if (response?.payload?.isSuccess) {
          toast.success(`Message ${values.id ? "updated" : "created"} successfully`);
          dispatch(getParcelMessages().then((response) => {
            if (response && response.payload?.isSuccess) {
              dispatch(getParcelMessages());
              onClose();
            } else {
              toast.error(response.error?.message || 'Error occurred');
            }
          }));
          resetForm();
          setIsEditMode(false);
          onClose();
        } else {
          toast.error("Something went wrong");
        }
      } catch (error) {
        console.error("Error saving message:", error);
        toast.error("Failed to save message");
      }
    },
  });

  const handleEdit = (e, msg) => {
    e.preventDefault();
    setIsEditMode(true);
    formik.setValues({
      id: msg.id,
      title: msg.title,
      content: msg.content,
      parcelType: msg.parcelType,
    });
  };

  const handleClone = (e, msg) => {
    e.preventDefault();
    setIsEditMode(false);
    formik.setValues({
      id: null,
      title: msg.title + " (copy)",
      content: msg.content,
      parcelType: msg.parcelType,
    });
  };

  return (
    <div className="add__modal fixed inset-0 bg-opacity-50 flex mt-2 justify-center z-10">
      <div className="add__modal__content add__modal__content__addParcelLocker rounded-lg shadow-lg w-full max-w-4xl overflow-auto">

        {/* Заголовок */}
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-white">Message</h2>
          <div className="manage__page flex items-center gap-2">
            <label className="inline-flex items-center cursor-pointer">
              <input
                type="checkbox"
                className="sr-only"
                checked={isEditMode}
                onChange={() => {
                  setIsEditMode(false);
                  isEditMode &&
                  formik.resetForm();
                }}
              />
              <div
                className={`w-10 h-6 rounded-full relative transition duration-300 ease-in-out ${isEditMode ? "bg-green-500" : "bg-gray-400"
                  }`}
              >
                <div
                  className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transition duration-300 ease-in-out transform ${isEditMode ? "translate-x-4" : ""
                    }`}
                ></div>
              </div>
            </label>
            <span style={{ color: "#191c1d" }}>Edit Mode</span>
          </div>
          <CloseButton onClick={onClose}>×</CloseButton>
        </div>

        {/* Форма */}
        <form onSubmit={formik.handleSubmit}>
          <div className="add__modal__content__part w-full">
            <span>General</span>
            <div className="add__modal__content__part__group mb-4 flex flex-col">
              <div>
                <input
                  type="text"
                  name="title"
                  value={formik.values.title}
                  onChange={formik.handleChange}
                  placeholder="Title"
                  className="w-full p-1 border rounded"
                />
                {formik.touched.title && formik.errors.title && (
                  <ShowFormikError message={formik.errors.title} />
                )}

                <div className="flex justify-between mt-4">
                  <div className="flex items-center gap-4 mt-4 mb-4">
                    <label className="flex items-center gap-2 cursor-pointer">
                      <input
                        type="radio"
                        name="parcelType"
                        className="accent-blue-500"
                        checked={formik.values.parcelType === 1}
                        onChange={() => formik.setFieldValue("parcelType", 1)}
                      />
                      Send
                    </label>

                    <label className="flex items-center gap-2 cursor-pointer">
                      <input
                        type="radio"
                        name="parcelType"
                        className="accent-blue-500"
                        checked={formik.values.parcelType === 2}
                        onChange={() => formik.setFieldValue("parcelType", 2)}
                      />
                      Receive
                    </label>
                  </div>
                </div>
              </div>
            </div>

            <div className="add__modal__content__part">
              <span>Message</span>
              <div className="add__modal__content__part__group gap-4 mb-4">
                <textarea
                  name="content"
                  rows="5"
                  placeholder="Write message..."
                  value={formik.values.content}
                  onChange={formik.handleChange}
                  className="w-full p-2 border rounded resize-none"
                ></textarea>
                {formik.touched.content && formik.errors.content && (
                  <ShowFormikError message={formik.errors.content} />
                )}
              </div>
            </div>

            {/* Список уже созданных сообщений */}
            <div className="add__modal__content__part">
              <span>Created messages</span>
              <div className="bg-white rounded mt-2">
                {messages.map((msg) => (
                  <div
                    key={msg.id}
                    className="p-2 border-b flex justify-between items-center"
                  >
                    <span>{msg.title}</span>
                    <div className="flex gap-3">
                      <button onClick={(e) => handleEdit(e, msg)}>
                        <CiEdit
                          className="text-2xl"
                          style={{ color: "#AAAAAA" }}
                        />
                      </button>
                      <button onClick={(e) => handleClone(e, msg)}>
                        <FaRegClone
                          className="text-xl"
                          style={{ color: "#AAAAAA" }}
                        />
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>

          {/* Кнопки */}
          <div className="flex justify-end space-x-4 mt-6">
            <div className="modal__button">
              <button
                type="button"
                className="text-white rounded"
                onClick={onClose}
              >
                Cancel
              </button>
            </div>
            <div className="modal__button">
              <button type="submit" className="text-white rounded">
                {isEditMode ? "Update" : "Save"}
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ParcelMessageModal;
