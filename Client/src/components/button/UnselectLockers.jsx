import React from 'react';
import { useDispatch } from 'react-redux';
import { setSelectedLockerIds } from '../../redux/slice/lockerSlice';

function UnselectLockers() {
  const dispatch = useDispatch();

  const handleUnselect = () => {
    dispatch(setSelectedLockerIds([]));
  };

  return (
    <button
      type="button"
      className="rounded bg-white px-2 py-1 mr-5"
      onClick={handleUnselect}
    >
      Unselect Lockers
    </button>
  );
}

export default UnselectLockers;
