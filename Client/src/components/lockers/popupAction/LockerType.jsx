import React, { useState } from 'react'
import CustomSelect from '../../select/CustomSelect'
import {  editLockersType, getLockers } from '../../../redux/api/lockerApi';
import { toast } from "react-toastify";
import { getLockerOptions } from '../../../enums/Locker/Types';
import { setSelectedLockerIds } from '../../../redux/slice/lockerSlice';
import { useDispatch } from 'react-redux';

function LockerType({ lockers, onClose }) {
    const dispatch = useDispatch();
    const [selectedType, setSelectedType] = useState(null);
    const lockerOptions = getLockerOptions().map((lockerType) => ({
        id: lockerType,
        name: lockerType.charAt(0).toUpperCase() + lockerType.slice(1),
      }));

    const handle = () => {
        if (lockers.length) {
            dispatch(editLockersType({ lockerIds: lockers, type: selectedType }))
                .then((response) => {
                    if (response && response.payload.isSuccess) {
                        dispatch(getLockers());
                        dispatch(setSelectedLockerIds([]));
                        toast.success("Lockers set successfully");
                        onClose();
                    }
                    onClose();
                })
                .catch((error) => {
                    toast.error('Locker types must be the same in the group');
                })
      
        } else {
            onClose();
        }
    }

    return (
        <>
            <span>Locker Type</span>
            <CustomSelect
                options={lockerOptions}
                onChange={(e) => setSelectedType(e.value)}
            />
            {
                selectedType && (
                    <button className='bg-gray-600 text-white rounded' onClick={handle}>
                        Send Locker Type
                    </button>
                )
            }
        </>
    )
}

export default LockerType