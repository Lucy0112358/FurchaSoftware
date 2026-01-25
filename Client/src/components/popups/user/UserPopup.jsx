import React from 'react';
import UserPopupMultiItem from './UserPopupMultiItem';
import Edit from '../../popupAction/user/Edit';
import Delete from '../../popupAction/user/Delete';
import State from '../../popupAction/user/State';
import AddUserGroup from '../../popupAction/user/AddUserGroup';


function UserPopup({ user, onClose, selectedIds, clearSelected }) {

  return (
    <div
      className="bg-white shadow-xl rounded-lg p-5 w-72 border border-gray-200"
      onClick={(e) => e.stopPropagation()}
    >
      {selectedIds?.length > 0 ? (
        <UserPopupMultiItem selectedIds={selectedIds} clearSelected={clearSelected} onClose={onClose} />
      ) : (
        user && (
          <>
            <div className="border-b border-gray-300 pb-2 mb-4">
              <h3 className="text-lg font-bold text-gray-800">
                User #{user.id}
                <span className="ml-2 inline-block text-sm text-gray-500 bg-gray-100 rounded px-2 py-1">
                  {/* {user.lockerType.name} */}
                </span>
              </h3>
            </div>

            <div className="flex flex-col gap-2">
              <Edit id={user.id}  onClose={() => onClose()} />
              <Delete ids={[user.id]}  onClose={() => onClose()} clearSelected={clearSelected} />
              <State user={user}  onClose={() => onClose()} />
              <AddUserGroup ids={[user.id]}  onClose={() => onClose()} />
            </div>
          </>
        )
      )}
    </div>
  );
}

export default UserPopup;
