import React, { useEffect, useState } from "react";
import '../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import 'react-tabs/style/react-tabs.css';
import './modulesModal.css';
import { getBranches, getLockerGroupsData } from "../../../redux/api/menuApi";
import { toast } from "react-toastify";
import * as Yup from "yup";
import { IoMdAdd } from "react-icons/io";
import CustomCheckbox from "../../checkbox/CustomCheckbox";
import { filterGroupByBranch, getLockerGroupMinMax, getModuleModalBranches, getModuleModalGroupes, getNewBrainsData } from "../../../redux/slice/moduleSlice";
import { addModuleFunc, getLockerGroupRange, getNewBrains } from "../../../redux/api/moduleApi";
import { getLockerOptions } from "../../../enums/Locker/Types";
import CloseButton from "../attributes/CloseButton";
import BranchModal from "../branch/BranchModal";
import { useFormik } from 'formik';
import LockerModal from "../locker/LockerModal";
import CustomSelect from "../../select/CustomSelect";


const ModulesModal = ({ onClose }) => {
  const branches = useSelector(getModuleModalBranches);
  const lockerGroups = useSelector(getModuleModalGroupes)
  const newBrains = useSelector(getNewBrainsData);
  const lockerGroupRange = useSelector(getLockerGroupMinMax);
  const [addGroupModalSwitch, setAddGroupModalSwitch] = useState(false);
  const lockerOptions = getLockerOptions().map((lockerType) => ({
    name: lockerType,
    label: lockerType.charAt(0).toUpperCase() + lockerType.slice(1),
  }));

  const handleLockerTypeChange = (selectedOption) => {
    sendGroupInfo('lockerType', selectedOption.value);
  };

  const dispatch = useDispatch();
  // const branches = useSelector(getBranchesData);
  // const [selectedBranch, setSelectedBranch] = useState([]);
  const [sentGeneralInfo, setSentGeneralInfo] = useState({
    lockerType: null,
    startBegin: false,
    lockerGroupId: null,
  });
  const [addBranchModalSwitch, setAddBranchModalSwitch] = useState(false);

  const newBrainsOptions = Object.values(newBrains)?.map(brain => ({
    value: brain.id,
    label: brain.macAddress + ' ' + (brain.info ? brain.info : ''),
  }));

  useEffect(() => {
    dispatch(getBranches());
    dispatch(getLockerGroupsData());
    dispatch(getNewBrains());
  }, []);


  const addModules = () => {
    dispatch(addModuleFunc(sentGeneralInfo))
      .then((response) => {
        if (response && response.payload.isSuccess) {
          toast.success("Modules created successfully");
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

  const formik = useFormik({
    initialValues: {
      lockerType: '',
      branchId: '',
      lockerGroupId: '',
      startBegin: false,
      lockerFrom: '',
      lockerTo: '',
      brainId: '',
    },
    validationSchema: Yup.object().shape({
      brainId: Yup.number().required('Brain ID is required'),
      branchId: Yup.number().required('Branch ID is required'),
      lockerGroupId: Yup.number().nullable(),
      lockerType: Yup.string().nullable(),
      lockerFrom: Yup.number()
        .required('Required')
        .min(1, 'Min 1')
        .max(256, 'Max 256'),

      lockerTo: Yup.number()
        .required('Required')
        .min(Yup.ref('lockerFrom'), 'Must be greater than or equal to "from"')
        .max(256, 'Max 256'),
    }),
    onSubmit: (values) => {
      addModules();
    },

    // onSubmit: (values) => {
    //   dispatch(addModuleFunc(values));
    // },
  });

  const handleBranchChange = (selectedOption) => {
    sendGroupInfo('branchId', selectedOption.value);
    dispatch(filterGroupByBranch(selectedOption.value));
  };

  const handleNewBrainChange = (selectedOption) => {
    sendGroupInfo('id', selectedOption.value);
    dispatch(getLockerGroupRange({ brainId: selectedOption.value, groupId: sentGeneralInfo.lockerGroupId }));
  };

  const handleLockerGroupChange = (selectedOption) => {
    sendGroupInfo('lockerGroupId', selectedOption.value);
    if (sentGeneralInfo?.id) {
      dispatch(getLockerGroupRange({ brainId: sentGeneralInfo.id, groupId: selectedOption.value }));
    }
  };

  return (
    <div
      className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10"
    >
      <form onSubmit={formik.handleSubmit}>
        <div className="add__modal__content add__modal__content__addModules rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-2xl font-semibold text-white">Add brain modules</h2>
            <CloseButton onClick={onClose}>
              &times;
            </CloseButton>
          </div>
          <div>
            <div className="add__modal__content__part">
              <span>General</span>
              <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
                <label className="block text-gray-300">Brain Module</label>
                <div>
                  <div className="w-5/6 mr-2">
                    <CustomSelect
                      options={newBrainsOptions}
                      name="brainId"
                      onChange={(option) => {
                        formik.setFieldValue('brainId', option?.name);
                        handleNewBrainChange(option);
                      }}
                    // onChange={(option) => formik.setFieldValue('brainId', option?.id)}
                    />
                    {formik.touched.brainId && formik.errors.brainId && (
                      <div className="text-red-500">{formik.errors.brainId}</div>
                    )}
                  </div>

                </div>
                <label className="block text-gray-300">Branch</label>
                <div className="flex ">
                  <div className="w-5/6 mr-2">
                    <CustomSelect
                      options={branches.map((branch) => ({
                        label: branch.name,
                        value: branch.id,
                      }))}
                      value={branches?.find(option => option.value === formik.values.branchId)}
                      onChange={(option) => {
                        formik.setFieldValue('branchId', option?.value);
                        handleBranchChange(option);
                      }}
                    />
                    {formik.touched.branchId && formik.errors.branchId && (
                      <div className="text-red-500">{formik.errors.branchId}</div>
                    )}
                  </div>
                  <div className="flex w-1/6">
                    <button
                      onClick={() => setAddBranchModalSwitch(true)}
                      className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center h-[43px]"
                    >
                      <IoMdAdd className="fill-current" style={{ fontSize: 'xx-large' }} />
                      {addBranchModalSwitch && <BranchModal onClose={(e) => {
                        if (e?.stopPropagation) e.stopPropagation();
                        setAddBranchModalSwitch(false);
                      }} />}
                    </button>
                  </div>
                </div>
              </div>
            </div>
            <div className="add__modal__content__part" >
              <span>Specify</span>
              <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
                <label className="block text-gray-300">Locker group(optional)</label>
                <div className="flex ">
                  <div className="w-5/6 mr-2">
                    <CustomSelect
                      options={lockerGroups.map((group) => ({
                        label: group.name,
                        value: group.id,
                      }))}
                      onChange={handleLockerGroupChange}
                    />
                  </div>
                  <div className="flex w-1/6">
                    <button
                      onClick={() => setAddGroupModalSwitch(true)}
                      className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center h-[43px]"
                    >
                      <IoMdAdd className="fill-current" style={{ fontSize: 'xx-large' }} />
                      {addGroupModalSwitch && <LockerModal
                        onClose={(e) => {
                          if (e?.stopPropagation) e.stopPropagation();
                          setAddGroupModalSwitch(false);
                        }}
                      />}
                    </button>
                  </div>
                </div>
                <label className="block text-gray-300">Locker type(optional)</label>
                <div className="flex ">
                  <div className="w-5/6 mr-2">
                    <CustomSelect options={Array.isArray(lockerOptions) ? lockerOptions.map((option) => ({
                      value: option.label === "All" ? "" : option.name,
                      label: option.label
                    })) : []}
                      onChange={handleLockerTypeChange} />
                  </div>
                </div>
                <label className="block text-gray-300">Locker numbers</label>
                <div className="flex">
                  <div className="generation w-full">
                    <div className="generation__checkbox flex items-center">
                      <CustomCheckbox onChange={(checked) => sendGroupInfo('startBegin', checked)} />
                      <span className="text-gray-800">Begin from last locker no, in the group</span>
                    </div>
                  </div>
                </div>
                {/* <div className="flex items-center space-x-2">
                  <input
                    type="number"
                    placeholder="1"
                    onChange={(e) => sendGroupInfo('firstLocker', e.target.value)}
                    className="w-20 border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                  <span className="text-gray-800">to</span>
                  <input
                    type="number"
                    placeholder="256"
                    onChange={(e) => sendGroupInfo('lastLocker', e.target.value)}
                    className="w-20 border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div> */}
                <div className="flex items-center gap-2">
                  <input
                    type="number"
                    name="lockerFrom"
                    placeholder={lockerGroupRange?.min || '1'}
                    value={formik.values.lockerFrom}
                    onChange={(e) => {
                      formik.handleChange(e);
                      const value = e.target.value ? parseInt(e.target.value, 10) : '';
                      formik.setFieldValue('lockerFrom', value);
                      sendGroupInfo('firstLocker', value);
                    }}
                    onBlur={formik.handleBlur}
                    className="w-20 border border-gray-300 rounded-md px-2 py-1"
                  />
                  <span className="text-gray-800">to</span>
                  <input
                    type="number"
                    name="lockerTo"
                    placeholder="256"
                    value={formik.values.lockerTo}
                    onChange={(e) => {
                      formik.handleChange(e);
                      const value = e.target.value ? parseInt(e.target.value, 10) : '';
                      formik.setFieldValue('lockerTo', value);
                      sendGroupInfo('lastLocker', value);
                    }}
                    onBlur={formik.handleBlur}
                    className="w-20 border border-gray-300 rounded-md px-2 py-1"
                  />
                </div>
                {formik.touched.lockerFrom && formik.errors.lockerFrom && <div className="text-red-500">{formik.errors.lockerFrom}</div>}
                {formik.touched.lockerTo && formik.errors.lockerTo && <div className="text-red-500">{formik.errors.lockerTo}</div>}
              </div>
            </div>
          </div>
          <div className="flex justify-end space-x-4 m-5 ">
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
                className="bg-gray-600 text-white rounded"
                type="submit"
              // onClick={addModules}
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

export default ModulesModal;
