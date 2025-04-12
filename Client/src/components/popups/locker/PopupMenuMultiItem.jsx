import React, { useEffect, useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import CustomSelect from '../../select/CustomSelect';
import { getModuleModalGroupes } from '../../../redux/slice/moduleSlice';
import { getLockerOptions } from '../../../enums/Locker/Types';
import { setSelectedLockerIds } from '../../../redux/slice/lockerSlice';
import OpenLocker from '../../lockers/popupAction/OpenLocker';
import SuspendLocker from '../../lockers/popupAction/SuspendLocker';
import LockerType from '../../lockers/popupAction/LockerType';

function PopupMenuMultiItem({ selectedLockerIds, onClose }) {
  const dispatch = useDispatch();
  const [data, setData] = useState({
    lockerIds: selectedLockerIds,
    type: null,
  })

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
        <OpenLocker lockers={selectedLockerIds} onClose={onClose} />
        <SuspendLocker lockers={selectedLockerIds} onClose={onClose} />
        <LockerType lockers={selectedLockerIds} onClose={onClose} />
      </div>
     
    </div>
  )
}

export default PopupMenuMultiItem;