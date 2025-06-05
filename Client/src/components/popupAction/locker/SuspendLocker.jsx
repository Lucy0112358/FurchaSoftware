import React from 'react'
import { suspendLockers } from '../../../redux/api/lockerApi';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";

function SuspendLocker({ lockers, onClose }) {
    const dispatch = useDispatch();
    const handle = () => {
        if (lockers.length) {
            dispatch(suspendLockers(lockers))
                .then((response) => {
                    if (response && response.payload.isSuccess) {
                        toast.success("Lockers suspended successfully");
                        onClose();
                    }
                })
            onClose();
        } else {
            onClose();
        }
    }
    return (
        <button className='bg-gray-600 text-white rounded' onClick={handle}>
            Suspend Locker(s)
        </button>
    )
}

export default SuspendLocker

