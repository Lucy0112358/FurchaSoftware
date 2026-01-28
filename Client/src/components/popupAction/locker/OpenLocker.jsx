import React from 'react'
import { openLockers } from '../../../redux/api/lockerApi';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";

function OpenLocker({ lockers, onClose, manage=true }) {
    const dispatch = useDispatch();
    const handle = () => {       
        if (lockers.length) {
            dispatch(openLockers(lockers))
                .then((response) => {
                    if (response && response.payload.isSuccess) {
                        toast.success("Lockers opened successfully");
                        onClose();
                    }
                })
            onClose();
        } else {
            onClose();
        }
    }
    return (
        <button className='bg-gray-600 text-white rounded' onClick={handle} disabled={!manage}>
            Open Locker(s)
        </button>
    )
}

export default OpenLocker