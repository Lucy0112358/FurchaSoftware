import React, { useEffect, useState } from "react";
import '../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import 'react-tabs/style/react-tabs.css';
import './modulesModal.css';
import { getLockerGroupsData } from "../../../redux/api/menuApi";
import { toast } from "react-toastify";
import * as Yup from "yup";
import { IoMdAdd } from "react-icons/io";
import { filterGroupByBranch, getModuleData, getNewBrainsData } from "../../../redux/slice/moduleSlice";
import { addModuleFunc, getNewBrains, moduleShow, updateModule } from "../../../redux/api/moduleApi";
import CloseButton from "../attributes/CloseButton";
import BranchModal from "../branch/BranchModal";
import { useFormik } from 'formik';
import CustomSelect from "../../select/CustomSelect";
import { deleteBrainId, getBranches } from "../../../redux/api/branchApi";
import { getAllBranchesData } from "../../../redux/slice/branchSlice";
import { MdDelete } from "react-icons/md";
import ConfirmModal from "../../confirm/ConfirmModal";


const ModulesModal = ({ onClose, id, mode = "add" }) => {
  const branches = useSelector(getAllBranchesData);
  const newBrains = useSelector(getNewBrainsData);
  const module = useSelector(getModuleData);
  const dispatch = useDispatch();

  const [addBranchModalSwitch, setAddBranchModalSwitch] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);


  const newBrainsOptions = Object.values(newBrains)?.map(brain => ({
    value: brain.id,
    label: brain.macAddress + ' ' + (brain.info ? brain.info : ''),
  }));

  useEffect(() => {
    dispatch(getBranches());
    dispatch(getLockerGroupsData());
    dispatch(getNewBrains());
  }, [dispatch]);

  useEffect(() => {
    if (id) {
      dispatch(moduleShow({ id }));
    }
  }, [id]);
  console.log(module, 'module in modal');

  const formik = useFormik({
    initialValues: {
      branchId: module.branchId || '',
      brainId: module.brainUid || '',
    },
    enableReinitialize: true,
    validationSchema: Yup.object().shape({
      brainId: mode === "edit"
        ? Yup.string().required("Brain ID is required")
        : Yup.number().required("Brain ID is required"),
      branchId: Yup.number().required("Branch ID is required"),
    }),
    onSubmit: (values) => {
      console.log(values, 'form values');

      const action = mode === "edit"
        ? updateModule({
          id, data: {
            branchId: values.branchId,
            // brainUid:values.brainId
          }
        })
        : addModuleFunc(values);

      dispatch(action)
        .then((response) => {
          if (response?.payload?.isSuccess) {
            toast.success(
              mode === "edit" ? "Module updated successfully" : "Module created successfully"
            );
            onClose();
          }
        })
        .catch(() => {
          toast.error("Something went wrong");
        });
    },
  });

  const handleBranchChange = (option) => {
    formik.setFieldValue('branchId', option?.value);
    dispatch(filterGroupByBranch(option.value));
  };

  const handleNewBrainChange = (option) => {
    formik.setFieldValue('brainId', option?.value);
  };

  const handleDeleteBrainId = () => {
    dispatch(deleteBrainId(formik.values.brainId))
        .then((response) => {
          if (response.payload?.isSuccess) {
            formik.setFieldValue('brainId', '')
          }
        });
  }

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <form onSubmit={formik.handleSubmit}>
        <div className="add__modal__content add__modal__content__addModules rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
          {/* Header */}
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-2xl font-semibold text-white">
              {mode === "edit" ? "Edit brain module" : "Add brain module"}
            </h2>
            <CloseButton onClick={onClose}>&times;</CloseButton>
          </div>

          {/* Form fields */}
          <div>
            <div className="add__modal__content__part">
              <span>General</span>
              <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
                {/* Brain Module */}
                <label className="block text-gray-300">Brain Module</label>
                <div className="w-5/6 mr-2">
                  {mode === "edit" && formik.values.brainId ? (
                    <div className="flex items-center space-x-2">
                      <input
                        type="text"
                        name="brainId"
                        value={formik.values.brainId}
                        readOnly
                        className="w-full px-3 py-2 rounded"
                      />

                      <button
                        type="button"
                        onClick={() => setShowConfirm(true)}
                        className="bg-red-500 text-white p-3 rounded-md hover:bg-red-700"
                      >
                        <MdDelete />
                      </button>
                    </div>
                  ) : (
                    <CustomSelect
                      options={newBrainsOptions}
                      name="brainId"
                      onChange={handleNewBrainChange}
                      value={
                        newBrainsOptions.find(
                          (opt) => String(opt.value) === String(formik.values.brainId)
                        ) || null
                      }
                    />
                  )}
                </div>

                {/* Branch */}
                <label className="block text-gray-300">Branch</label>
                <div className="flex">
                  <div className="w-5/6 mr-2">
                    <CustomSelect
                      options={branches?.map(branch => ({
                        label: branch.name,
                        value: branch.id,
                      }))}
                      onChange={handleBranchChange}
                      value={branches
                        ?.map(branch => ({
                          label: branch.name,
                          value: branch.id,
                        }))
                        .find(opt => opt.value === formik.values.branchId) || null}
                    />
                    {formik.touched.branchId && formik.errors.branchId && (
                      <div className="text-red-500">{formik.errors.branchId}</div>
                    )}
                  </div>
                  <div className="flex">
                    <button
                      type="button"
                      onClick={() => setAddBranchModalSwitch(true)}
                      className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center h-[43px]"
                    >
                      <IoMdAdd className="fill-current text-2xl" />
                    </button>
                    {addBranchModalSwitch && (
                      <BranchModal
                        onClose={(e) => {
                          if (e?.stopPropagation) e.stopPropagation();
                          setAddBranchModalSwitch(false);
                        }}
                      />
                    )}

                    {showConfirm && (
                      <ConfirmModal
                        message={'Are you sure you want to delete the brain ID?'}
                        onConfirm={() => {
                          handleDeleteBrainId();
                          setShowConfirm(false);
                        }}
                        onCancel={() => setShowConfirm(false)}
                      />
                    )}
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Buttons */}
          <div className="flex justify-end space-x-4 m-5">
            <div className="modal__button">
              <button
                type="button"
                className="bg-gray-600 text-white rounded px-4 py-2"
                onClick={onClose}
              >
                Cancel
              </button>
            </div>
            <div className="modal__button">
              <button
                type="submit"
                className="bg-gray-600 text-white rounded px-4 py-2"
              >
                {mode === "edit" ? "Update" : "Save"}
              </button>
            </div>
          </div>

        </div>
      </form>
    </div>
  );
};

export default ModulesModal;