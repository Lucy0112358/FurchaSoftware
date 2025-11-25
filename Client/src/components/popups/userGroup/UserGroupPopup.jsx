import React from 'react';
import Edit from '../../popupAction/userGroup/Edit';
import Delete from '../../popupAction/userGroup/Delete';
import UserGroupPopupMultiItem from './UserGroupPopupMultiItem';
import { getUserGroupData } from '../../../redux/slice/groupSlice';
import ChangeState from '../../popupAction/userGroup/ChangeState';
import { useSelector } from 'react-redux';


function UserGroupPopup({ userGroup, clearSelected , onClose, selectedIds }) {
    const groupData = useSelector(getUserGroupData);

  return (
    <div
      className="bg-white shadow-xl rounded-lg p-5 w-72 border border-gray-200"
      onClick={(e) => e.stopPropagation()}
    >
      {selectedIds?.length > 0 ? (
        <UserGroupPopupMultiItem selectedIds={selectedIds}  clearSelected ={clearSelected } onClose={onClose} />
      ) : (
        userGroup && (
          <>
            <div className="border-b border-gray-300 pb-2 mb-4">
              <h3 className="text-lg font-bold text-gray-800">
                User Group #{userGroup.id}
              </h3>
            </div>

            <div className="flex flex-col gap-2">
               <Edit id={userGroup.id} onClose={onClose} />
               <Delete ids={[userGroup.id]} clearSelected ={clearSelected } onClose={onClose} />
               <ChangeState ids={[userGroup.id]} action={groupData.state == 1 ? 'Suspend' : 'Active'} onClose={onClose} />
            </div>
          </>
        )
      )}
    </div>
  );
}

export default UserGroupPopup;
