import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import CustomSelect from '../../select/CustomSelect';
import { getModuleModalGroupes } from '../../../redux/slice/moduleSlice';
import { getLockerGroupsData } from '../../../redux/api/menuApi';
import { getLockerOptions } from '../../../enums/LockerTypes';
import { editLockers, getLockers } from '../../../redux/api/lockerApi';
import { toast } from "react-toastify";
import { setSelectedLockerIds } from '../../../redux/slice/lockerSlice';


function PopupMenuMultiItem({ selectedLockerIds, onClose }) {
  const lockerGroups = useSelector(getModuleModalGroupes)
  const dispatch = useDispatch();
  const [data, setData] = useState({
    groupId: null,
    lockerIds: selectedLockerIds,
    type: null,
  })

  useEffect(() => {
    dispatch(getLockerGroupsData());
  }, []);

  const lockerOptions = getLockerOptions().map((lockerType) => ({
    id: lockerType,
    name: lockerType.charAt(0).toUpperCase() + lockerType.slice(1),
  }));

  const handleSave = () => {
    dispatch(editLockers(data))
          .then((response) => {
            if (response && response.payload?.isSuccess) {
              dispatch(getLockers());
              dispatch(setSelectedLockerIds([]));
              onClose();
            } else {
              toast.error(response.error?.message || 'Error occurred');
            }
          })
          .catch((error) => console.error('Error updating user info:', error));
  }

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
    <div>
      <h4>Selected Lockers</h4>
      Locker IDS:
      {selectedLockerIds.map((id, i) => (
        <span key={id}>
          {id}
          {i < selectedLockerIds.length - 1 && ', '}
        </span>
      ))}
      <div className="add__modal__content__part__group grid grid-cols-1 gap-4 mb-4">
        <label className="block text-gray-300">Locker group(optional)</label>
        <div className="flex ">
          <div className="w-5/6 mr-2">
            <CustomSelect
              options={lockerGroups}
              onChange={(e) => setData({ ...data, groupId: e.value })}
            />
          </div>
        </div>
        <label className="block text-gray-300">Locker type(optional)</label>
        <div className="flex ">
          <div className="w-5/6 mr-2">
            <CustomSelect options={lockerOptions} onChange={(e) => setData({ ...data, type: e.value })} />
          </div>
        </div>
      </div>
      <button style={buttonStyle} 
      onClick={handleSave}>
        Save
      </button>
      <button style={buttonStyle} onClick={() => {
        onClose();
        setData({ groupId: null, lockerIds: [], type: null });
      }}>
        Cancel
      </button>
    </div>
  )
}

export default PopupMenuMultiItem;