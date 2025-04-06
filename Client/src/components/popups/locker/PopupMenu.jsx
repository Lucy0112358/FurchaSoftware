import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { getSelectedLockerIds } from '../../../redux/slice/lockerSlice';
import CustomSelect from '../../select/CustomSelect';
import { getModuleModalGroupes } from '../../../redux/slice/moduleSlice';
import { getLockerGroupsData } from '../../../redux/api/menuApi';
import PopupMenuMultiItem from './PopupMenuMultiItem';

function PopupMenu({ locker, onClose }) {
  const selectedLockerIds = useSelector(getSelectedLockerIds);
  const buttonStyle = {
    display: 'block',
    width: '100%',
    marginBottom: '4px',
    textAlign: 'left',
    backgroundColor: '#ddd',
    border: 'none',
    cursor: 'pointer',
    padding: '4px 6px',
  };

  return (
    <div onClick={(e) => e.stopPropagation()}>
      {selectedLockerIds?.length > 0 ? (
        <PopupMenuMultiItem
          selectedLockerIds={selectedLockerIds}
          onClose={onClose} />
      ) : (
        <div>
          {
            locker && (
              <>
                <h4>Locker #{locker.id} (type: {locker.lockerType})</h4>
                <button style={buttonStyle} onClick={() => { console.log('Edit'); onClose(); }}>
                  Edit
                </button>
                <button style={buttonStyle} onClick={() => { console.log('Open'); onClose(); }}>
                  Open Locker
                </button>
                <button style={buttonStyle} onClick={() => { console.log('Suspend'); onClose(); }}>
                  Suspend Locker
                </button>
                {locker.lockerType === 'Personal' && (
                  <button style={buttonStyle} onClick={() => { console.log('Set'); onClose(); }}>
                    Set User
                  </button>
                )}
                {locker.lockerType === 'handOver' && (
                  <button style={buttonStyle} onClick={() => { console.log('Hand action'); onClose(); }}>
                    Hand action
                  </button>
                )}
                {locker.lockerType === 'Parcel' && (
                  <button style={buttonStyle} onClick={() => { console.log('Set'); onClose(); }}>
                    Set User
                  </button>
                )}
              </>
            )
          }

        </div>
      )}
    </div>
  );
}

export default PopupMenu;