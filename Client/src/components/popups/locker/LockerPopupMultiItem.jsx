import React, { useState } from 'react';
import OpenLocker from '../../popupAction/locker/OpenLocker';
import ChangeLockerMode from '../../popupAction/locker/ChangeLockerMode';
import LockerType from '../../popupAction/locker/LockerType';
import { useHasPermission } from '../../../hooks/useHasPermission';

function LockerPopupMultiItem({ selectedLockerIds, onClose }) {
  const { hasPermission } = useHasPermission();

  return (
    <div className="bg-white rounded-xl max-w-md mx-auto">
      <h2 className="text-xl font-semibold text-gray-800 mb-4">Edit Selected Lockers</h2>
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700 mb-1">Selected Locker IDs:</label>
        <div className="text-gray-600 text-sm bg-gray-100 p-2 rounded">
          {selectedLockerIds.map((id, i) => (
            <span key={id}>
              {id}{i < selectedLockerIds.length - 1 && ', '}
            </span>
          ))}
        </div>
      </div>
      <div className='flex flex-col gap-2'>
        {
          hasPermission(['LVL3_Admin'], ['Open_Locker']) && <OpenLocker lockers={selectedLockerIds} onClose={onClose} />
        }
        <ChangeLockerMode ids={selectedLockerIds} action='Suspend' onClose={onClose} />
        <ChangeLockerMode ids={selectedLockerIds} action='Active' onClose={onClose} />
        {
          hasPermission() && <LockerType lockers={selectedLockerIds} onClose={onClose} />
        }
      </div>
    </div>
  )
}

export default LockerPopupMultiItem;