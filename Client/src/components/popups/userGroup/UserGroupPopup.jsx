import React from 'react';
import Edit from '../../popupAction/userGroup/Edit';
import Delete from '../../popupAction/userGroup/Delete';
import UserGroupPopupMultiItem from './UserGroupPopupMultiItem';
import Suspend from '../../popupAction/userGroup/Suspend';


function UserGroupPopup({ userGroup, onClose, selectedIds }) {

  return (
    <div
      className="bg-white shadow-xl rounded-lg p-5 w-72 border border-gray-200"
      onClick={(e) => e.stopPropagation()}
    >
      {selectedIds?.length > 0 ? (
        <UserGroupPopupMultiItem selectedIds={selectedIds} onClose={onClose} />
      ) : (
        userGroup && (
          <>
            <div className="border-b border-gray-300 pb-2 mb-4">
              <h3 className="text-lg font-bold text-gray-800">
                User Group #{userGroup.id}
              </h3>
            </div>

            <div className="flex flex-col gap-2">
               <Edit groups={userGroup} onClose={onClose} />
               <Delete ids={[userGroup.id]} onClose={onClose} />
               <Suspend ids={[userGroup.id]} onClose={onClose} />
            </div>
          </>
        )
      )}
    </div>
  );
}

export default UserGroupPopup;
