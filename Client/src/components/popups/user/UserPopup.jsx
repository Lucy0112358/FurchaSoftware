import React from 'react';
import { useSelector } from 'react-redux';
import { getSelectedLockerIds } from '../../../redux/slice/lockerSlice';
import UserPopupMultiItem from './UserPopupMultiItem';


function UserPopup({ user, onClose, selectedIds}) {

  return (
    <div
      className="bg-white shadow-xl rounded-lg p-5 w-72 border border-gray-200"
      onClick={(e) => e.stopPropagation()}
    >
      {selectedIds?.length > 0 ? (
        <UserPopupMultiItem selectedIds={selectedIds} onClose={onClose} />
      ) : (
        user && (
          <>
            <div className="border-b border-gray-300 pb-2 mb-4">
              <h3 className="text-lg font-bold text-gray-800">
                User #{user.id}
                <span className="ml-2 inline-block text-sm text-gray-500 bg-gray-100 rounded px-2 py-1">
                  {user.lockerType}
                </span>
              </h3>
            </div>

            <div className="flex flex-col gap-2">
              {/* <Edit lockers={[user]} onClose={onClose} /> */}
              {/* <OpenLocker lockers={[user.id]} onClose={onClose} />
              <SuspendLocker lockers={[user.id]} onClose={onClose} /> */}
            </div>
          </>
        )
      )}
    </div>
  );
}

export default UserPopup;
