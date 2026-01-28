import React from 'react';
import Edit from '../../popupAction/branch/Edit';
import Delete from '../../popupAction/branch/Delete';
import { getManage } from '../../../redux/slice/systemSlice';
import { useSelector } from 'react-redux';

function BranchPopup({ branch, onClose }) {
  const manage = useSelector(getManage);

  return (
    <div
      className="bg-white shadow-xl rounded-lg p-5 w-72 border border-gray-200"
      onClick={(e) => e.stopPropagation()}
    >
      {branch && (
        <>
          <div className="border-b border-gray-300 pb-2 mb-4">
            <h3 className="text-lg font-bold text-gray-800">
              Branch #{branch.id}
              <span className="ml-2 inline-block text-sm text-gray-500 bg-gray-100 rounded px-2 py-1">
                {branch.lockerType}
              </span>
            </h3>
          </div>

          <div className="flex flex-col gap-2">
            <Delete branch={branch} onClose={() => onClose()} manage={manage} />
            <Edit branch={branch} onClose={() => onClose()} manage={manage} />
          </div>
        </>
      )
      }
    </div>
  );
}

export default BranchPopup;
