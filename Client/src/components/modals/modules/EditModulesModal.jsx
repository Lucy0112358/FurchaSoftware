import React, { useEffect, useState } from "react";
import '../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import 'react-tabs/style/react-tabs.css';
import './modulesModal.css';
import { getLockerGroupsData } from "../../../redux/api/menuApi";
import { toast } from "react-toastify";
import * as Yup from "yup";
import { IoMdAdd } from "react-icons/io";
import { getLockerGroupMinMax, getModuleLockerData, getModuleModalGroupes } from "../../../redux/slice/moduleSlice";
import { editModuleLocker, getModuleLocker } from "../../../redux/api/moduleApi";
import { getLockerOptions } from "../../../enums/Locker/Types";
import CloseButton from "../attributes/CloseButton";
import { useFormik } from 'formik';
import LockerModal from "../locker/LockerModal";
import CustomSelect from "../../select/CustomSelect";
import { getLockerTypesData } from "../../../redux/slice/lockerSlice";

const EditModulesModal = ({ id, onClose }) => {
  const dispatch = useDispatch();

  const lockerGroups = useSelector(getModuleModalGroupes);
  const lockerGroupRange = useSelector(getLockerGroupMinMax);
  const moduleLocker = useSelector(getModuleLockerData);

  const [addGroupModalSwitch, setAddGroupModalSwitch] = useState(false);
  // const lockerOptions = getLockerOptions().map((lockerType) => ({
  //   name: lockerType.type,
  //   label: lockerType.type.charAt(0).toUpperCase() + lockerType.type.slice(1),
  // }));

   const lockerOptions = useSelector(getLockerTypesData).map((lockerType) => ({
    name: lockerType.id,
    label: lockerType.name,
  }));
  
console.log(lockerOptions, 'lockerOptions22222222');

  useEffect(() => {
    dispatch(getLockerGroupsData());
  }, [dispatch]);

  useEffect(() => {
    if (id) {
      dispatch(getModuleLocker({ id }));
    }
  }, [id, dispatch]);

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      lockerType: moduleLocker?.lockerType || '',
      lockerFrom: moduleLocker?.lockerRange?.start || '',
      lockerTo: moduleLocker?.lockerRange?.end || '',
      lockerGroupId: moduleLocker?.lockerGroupId || null,
    },
    validationSchema: Yup.object().shape({
      lockerGroupId: Yup.number().nullable(),
      lockerType: Yup.string().nullable(),
      lockerFrom: Yup.number()
        .required('Required')
        .min(1, 'Min 1')
        .max(256, 'Max 256'),
      lockerTo: Yup.number()
        .required('Required')
        .min(Yup.ref('lockerFrom'), 'Must be >= "from"')
        .max(256, 'Max 256'),
    }),
    onSubmit: async (values) => {
      try {
        const response = await dispatch(editModuleLocker({ id, data: values }));
        if (response?.payload?.isSuccess) {
          toast.success("Module updated successfully");
          onClose();
        } else {
          toast.error("Update failed");
        }
      } catch (err) {
        toast.error("Something went wrong");
      }
    },
  });

  const handleLockerGroupChange = (selectedOption) => {
    formik.setFieldValue("lockerGroupId", selectedOption?.value || null);
    // if (selectedOption?.value) {
    //   dispatch(getLockerGroupRange({ brainId: id, groupId: selectedOption.value }));
    // }
  };

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <form onSubmit={formik.handleSubmit}>
        <div className="add__modal__content add__modal__content__addModules rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-2xl font-semibold text-white">Edit brain modules</h2>
            <CloseButton onClick={onClose}>
              &times;
            </CloseButton>
          </div>

          <div className="add__modal__content__part">
            <span>Specify</span>
            <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">

              {/* Locker group */}
              <label className="block text-gray-300">Locker group (optional)</label>
              <div className="flex">
                <div className="w-5/6 mr-2">
                  <CustomSelect
                    options={lockerGroups.map((group) => ({
                      label: group.name,
                      value: group.id,
                    }))}
                    value={lockerGroups.find(group => group.id === formik.values.lockerGroupId)
                      ? {
                        label: lockerGroups.find(group => group.id === formik.values.lockerGroupId).name,
                        value: formik.values.lockerGroupId
                      }
                      : null}
                    onChange={handleLockerGroupChange}
                  />
                </div>
                <div className="flex">
                  <button
                    type="button"
                    onClick={() => setAddGroupModalSwitch(true)}
                    className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center h-[43px]"
                  >
                    <IoMdAdd className="fill-current text-2xl" />
                  </button>
                  {addGroupModalSwitch && (
                    <LockerModal
                      onClose={(e) => {
                        if (e?.stopPropagation) e.stopPropagation();
                        setAddGroupModalSwitch(false);
                      }}
                    />
                  )}
                </div>
              </div>

              {/* Locker type */}
              <label className="block text-gray-300">Locker type (optional)</label>
              <div className="flex">
                <div className="w-5/6 mr-2">
                  <CustomSelect
                    options={lockerOptions.map((opt) => ({
                      value: opt.name,
                      label: opt.label,
                    }))}
                    value={lockerOptions.find(opt => opt.name === formik.values.lockerType)
                      ? {
                        value: formik.values.lockerType,
                        label: lockerOptions.find(opt => opt.name === formik.values.lockerType).label
                      }
                      : null}
                    onChange={(option) =>
                      formik.setFieldValue("lockerType", option?.value || "")
                    }
                  />
                </div>
              </div>

              {/* Locker range */}
              <label className="block text-gray-300">Locker numbers</label>
              <div className="flex items-center gap-2">
                <input
                  type="number"
                  name="lockerFrom"
                  placeholder={lockerGroupRange?.min || '1'}
                  value={formik.values.lockerFrom}
                  onChange={formik.handleChange}
                  onBlur={formik.handleBlur}
                  className="w-20 border border-gray-300 rounded-md px-2 py-1"
                />
                <span className="text-gray-800">to</span>
                <input
                  type="number"
                  name="lockerTo"
                  placeholder="256"
                  value={formik.values.lockerTo}
                  onChange={formik.handleChange}
                  onBlur={formik.handleBlur}
                  className="w-20 border border-gray-300 rounded-md px-2 py-1"
                />
              </div>
              {formik.touched.lockerFrom && formik.errors.lockerFrom && (
                <div className="text-red-500">{formik.errors.lockerFrom}</div>
              )}
              {formik.touched.lockerTo && formik.errors.lockerTo && (
                <div className="text-red-500">{formik.errors.lockerTo}</div>
              )}
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
                Save
              </button>
            </div>
          </div>
        </div>
      </form>
    </div>
  );
};

export default EditModulesModal;

