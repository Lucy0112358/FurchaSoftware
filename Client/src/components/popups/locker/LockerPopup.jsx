import React from 'react';
import { useSelector } from 'react-redux';
import { getSelectedLockerIds } from '../../../redux/slice/lockerSlice';
import LockerPopupMultiItem from './LockerPopupMultiItem';
import Edit from '../../lockers/popupAction/Edit';
import OpenLocker from '../../lockers/popupAction/OpenLocker';
import SuspendLocker from '../../lockers/popupAction/SuspendLocker';
import SetUser from '../../lockers/popupAction/SetUser';
import HandAction from '../../lockers/popupAction/HandAction';

function LockerPopup({ locker, onClose, branchId=null }) {
  const selectedLockerIds = useSelector(getSelectedLockerIds);

  return (
    <div
      className="bg-white shadow-xl rounded-lg p-5 w-72 border border-gray-200"
      onClick={(e) => e.stopPropagation()}
    >
      {selectedLockerIds?.length > 0 ? (
        <LockerPopupMultiItem selectedLockerIds={selectedLockerIds} onClose={onClose} />
      ) : (
        locker && (
          <>
            <div className="border-b border-gray-300 pb-2 mb-4">
              <h3 className="text-lg font-bold text-gray-800">
                Locker #{locker.id}
                <span className="ml-2 inline-block text-sm text-gray-500 bg-gray-100 rounded px-2 py-1">
                  {locker.lockerType}
                </span>
              </h3>
            </div>

            <div className="flex flex-col gap-2">
              <Edit lockers={[locker]} onClose={onClose} />
              <OpenLocker lockers={[locker.id]} onClose={onClose} />
              <SuspendLocker lockers={[locker.id]} onClose={onClose} />

              {locker.lockerType === 'handOver' && (
                <HandAction lockers={[locker]} onClose={onClose} />
              )}

              {(locker.lockerType === 'parcel' || locker.lockerType === 'personal') && (
                <SetUser lockers={[locker]} onClose={onClose} branchId={branchId} />
              )}
            </div>
          </>
        )
      )}
    </div>
  );
}

export default LockerPopup;
