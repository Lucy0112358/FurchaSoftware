import React, { useState } from 'react';
import OpenLocker from '../../popupAction/locker/OpenLocker';
import SuspendLocker from '../../popupAction/locker/SuspendLocker';
import LockerType from '../../popupAction/locker/LockerType';

function UserPopupMultiItem({ selectedIds, onClose }) {
  return (
    <div className="bg-white rounded-xl max-w-md mx-auto">
      <h2 className="text-xl font-semibold text-gray-800 mb-4">Edit Selected Users</h2>
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700 mb-1">Selected User IDs:</label>
        <div className="text-gray-600 text-sm bg-gray-100 p-2 rounded">
          {selectedIds.map((id, i) => (
            <span key={id}>
              {id}{i < selectedIds.length - 1 && ', '}
            </span>
          ))}
        </div>
      </div>
      <div className='flex flex-col gap-2'>
        MultiItem
        {/* <OpenLocker lockers={selectedLockerIds} onClose={onClose} />
        <SuspendLocker lockers={selectedLockerIds} onClose={onClose} />
        <LockerType lockers={selectedLockerIds} onClose={onClose} /> */}
      </div>
    </div>
  )
}

export default UserPopupMultiItem;