import React, { useState } from 'react'
import { editLockersType, getLockers } from '../../../redux/api/lockerApi';
import { toast } from "react-toastify";
import { getLockerOptions } from '../../../enums/Locker/Types';
import { getLockerTypesData, setSelectedLockerIds } from '../../../redux/slice/lockerSlice';
import { useDispatch, useSelector } from 'react-redux';
import CustomSelect from '../../select/CustomSelect';

function LockerType({ lockers, onClose, manage=true }) {
    const dispatch = useDispatch();
    const [selectedTypeId, setSelectedTypeId] = useState(null);
    // const lockerOptions = getLockerOptions().map((lockerType) => ({
    //     label: lockerType,
    //     value: lockerType.charAt(0).toUpperCase() + lockerType.slice(1),
    //   }));
    const lockerOptions = useSelector(getLockerTypesData).map((lockerType) => ({
        value: lockerType.id,
        label: lockerType.name,
    }));

    const handle = () => {
        if (lockers.length) {
            dispatch(editLockersType({ lockerIds: lockers, type: selectedTypeId }))
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
                value={lockerOptions.find((option) => option.value === selectedTypeId)}
                onChange={(e) => setSelectedTypeId(e.value)}
            />
            {
                selectedTypeId && (
                    <button className='bg-gray-600 text-white rounded' disabled={!manage} onClick={handle}>
                        Send Locker Type
                    </button>
                )
            }
        </>
    )
}

export default LockerType