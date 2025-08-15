// import React, { useEffect, useState } from "react";
// import '../modal.css';
// import { useDispatch, useSelector } from 'react-redux';
// import 'react-tabs/style/react-tabs.css';
// import './modulesModal.css';
// import { getBranches, getLockerGroupsData } from "../../../redux/api/menuApi";
// import { toast } from "react-toastify";
// import * as Yup from "yup";
// import { IoMdAdd } from "react-icons/io";
// import { filterGroupByBranch, getLockerGroupMinMax, getModuleModalBranches, getModuleModalGroupes, getNewBrainsData } from "../../../redux/slice/moduleSlice";
// import { addModuleFunc, getLockerGroupRange, getNewBrains } from "../../../redux/api/moduleApi";
// import CloseButton from "../attributes/CloseButton";
// import BranchModal from "../branch/BranchModal";
// import { useFormik } from 'formik';
// import CustomSelect from "../../select/CustomSelect";


// const ModulesModal = ({ onClose }) => {
//   const branches = useSelector(getModuleModalBranches);
//   const newBrains = useSelector(getNewBrainsData);

//   const dispatch = useDispatch();
//   const [sentGeneralInfo, setSentGeneralInfo] = useState({
//     lockerType: null,
//     startBegin: false,
//   });
//   const [addBranchModalSwitch, setAddBranchModalSwitch] = useState(false);

//   const newBrainsOptions = Object.values(newBrains)?.map(brain => ({
//     value: brain.id,
//     label: brain.macAddress + ' ' + (brain.info ? brain.info : ''),
//   }));

//   useEffect(() => {
//     dispatch(getBranches());
//     dispatch(getLockerGroupsData());
//     dispatch(getNewBrains());
//   }, []);


//   const addModules = () => {
//     dispatch(addModuleFunc(sentGeneralInfo))
//       .then((response) => {
//         if (response && response.payload.isSuccess) {
//           toast.success("Modules created successfully");
//           onClose();
//         }
//       })
//       .catch((error) => {
//         toast.error("Something went wrong");
//       });
//   }

//   const sendGroupInfo = (key, value) => {
//     setSentGeneralInfo((prev) => ({
//       ...prev,
//       [key]: value,
//     }));
//   };

//   const formik = useFormik({
//     initialValues: {
//       branchId: '',
//       startBegin: false,
//       brainId: '',
//     },
//     validationSchema: Yup.object().shape({
//       brainId: Yup.number().required('Brain ID is required'),
//       branchId: Yup.number().required('Branch ID is required'),
//     }),
//     onSubmit: (values) => {
//       addModules();
//     },

//     // onSubmit: (values) => {
//     //   dispatch(addModuleFunc(values));
//     // },
//   });

//   const handleBranchChange = (selectedOption) => {
//     sendGroupInfo('branchId', selectedOption.value);
//     dispatch(filterGroupByBranch(selectedOption.value));
//   };

//   const handleNewBrainChange = (selectedOption) => {
//     sendGroupInfo('id', selectedOption.value);
//     dispatch(getLockerGroupRange({ brainId: selectedOption.value, groupId: sentGeneralInfo.lockerGroupId }));
//   };


//   return (
//     <div
//       className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10"
//     >
//       <form onSubmit={formik.handleSubmit}>
//         <div className="add__modal__content add__modal__content__addModules rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
//           <div className="flex justify-between items-center mb-4">
//             <h2 className="text-2xl font-semibold text-white">Add brain modules</h2>
//             <CloseButton onClick={onClose}>
//               &times;
//             </CloseButton>
//           </div>
//           <div>
//             <div className="add__modal__content__part">
//               <span>General</span>
//               <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
//                 <label className="block text-gray-300">Brain Module</label>
//                 <div>
//                   <div className="w-5/6 mr-2">
//                     <CustomSelect
//                       options={newBrainsOptions}
//                       name="brainId"
//                       onChange={(option) => {
//                         formik.setFieldValue('brainId', option?.name);
//                         handleNewBrainChange(option);
//                       }}
//                     // onChange={(option) => formik.setFieldValue('brainId', option?.id)}
//                     />
//                     {formik.touched.brainId && formik.errors.brainId && (
//                       <div className="text-red-500">{formik.errors.brainId}</div>
//                     )}
//                   </div>

//                 </div>
//                 <label className="block text-gray-300">Branch</label>
//                 <div className="flex ">
//                   <div className="w-5/6 mr-2">
//                     <CustomSelect
//                       options={branches?.map((branch) => ({
//                         label: branch.name,
//                         value: branch.id,
//                       }))}
//                       value={branches?.find(option => option.value === formik.values.branchId)}
//                       onChange={(option) => {
//                         formik.setFieldValue('branchId', option?.value);
//                         handleBranchChange(option);
//                       }}
//                     />
//                     {formik.touched.branchId && formik.errors.branchId && (
//                       <div className="text-red-500">{formik.errors.branchId}</div>
//                     )}
//                   </div>
//                   <div className="flex">
//                     <button
//                       onClick={() => setAddBranchModalSwitch(true)}
//                       className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold px-4 rounded inline-flex items-center h-[43px]"
//                     >
//                       <IoMdAdd className="fill-current" style={{ fontSize: 'xx-large' }} />
//                       {addBranchModalSwitch && <BranchModal onClose={(e) => {
//                         if (e?.stopPropagation) e.stopPropagation();
//                         setAddBranchModalSwitch(false);
//                       }} />}
//                     </button>
//                   </div>
//                 </div>
//               </div>
//             </div>
//           </div>
//           <div className="flex justify-end space-x-4 m-5 ">
//             <div className="modal__button">
//               <button
//                 type="button"
//                 className="bg-gray-600 text-white rounded"
//                 onClick={onClose}
//               >
//                 Cancel
//               </button>
//             </div>
//             <div className="modal__button">
//               <button
//                 className="bg-gray-600 text-white rounded"
//                 type="submit"
//                 onClick={addModules}
//               >
//                 Save
//               </button>
//             </div>
//           </div>
//         </div>
//       </form>
//     </div>
//   );
// };

// export default ModulesModal;



import React, { useEffect, useState } from "react";
import '../modal.css';
import { useDispatch, useSelector } from 'react-redux';
import 'react-tabs/style/react-tabs.css';
import './modulesModal.css';
import { getBranches, getLockerGroupsData } from "../../../redux/api/menuApi";
import { toast } from "react-toastify";
import * as Yup from "yup";
import { IoMdAdd } from "react-icons/io";
import { filterGroupByBranch, getModule, getModuleModalBranches, getNewBrainsData } from "../../../redux/slice/moduleSlice";
import { addModuleFunc, getLockerGroupRange, getNewBrains } from "../../../redux/api/moduleApi";
import CloseButton from "../attributes/CloseButton";
import BranchModal from "../branch/BranchModal";
import { useFormik } from 'formik';
import CustomSelect from "../../select/CustomSelect";

const ModulesModal = ({ onClose }) => {
  const branches = useSelector(getModuleModalBranches);
  const newBrains = useSelector(getNewBrainsData);
  const dispatch = useDispatch();


  const [addBranchModalSwitch, setAddBranchModalSwitch] = useState(false);

  const newBrainsOptions = Object.values(newBrains)?.map(brain => ({
    value: brain.id,
    label: brain.macAddress + ' ' + (brain.info ? brain.info : ''),
  }));

  useEffect(() => {
    dispatch(getBranches());
    dispatch(getLockerGroupsData());
    dispatch(getNewBrains());
  }, [dispatch]);

  const formik = useFormik({
    initialValues: {
      branchId: '',
      brainId: '',
    },
    validationSchema: Yup.object().shape({
      brainId: Yup.number().required('Brain ID is required'),
      branchId: Yup.number().required('Branch ID is required'),
    }),
    onSubmit: (values) => {
      dispatch(addModuleFunc(values))
        .then((response) => {
          if (response?.payload?.isSuccess) {
            toast.success("Modules created successfully");
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
    dispatch(getLockerGroupRange({ brainId: option.value }));
  };

  return (
    <div className="add__modal fixed inset-0 bg-gray-600 bg-opacity-50 flex mt-2 justify-center z-10">
      <form onSubmit={formik.handleSubmit}>
        <div className="add__modal__content add__modal__content__addModules rounded-lg shadow-lg w-full max-w-4xl overflow-auto">
          
          {/* Header */}
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-2xl font-semibold text-white">Add brain modules</h2>
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
                  <CustomSelect
                    options={newBrainsOptions}
                    name="brainId"
                    onChange={handleNewBrainChange}
                  />
                  {formik.touched.brainId && formik.errors.brainId && (
                    <div className="text-red-500">{formik.errors.brainId}</div>
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
