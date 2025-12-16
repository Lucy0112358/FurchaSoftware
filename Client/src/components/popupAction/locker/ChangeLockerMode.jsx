import React, { useState } from 'react'
import { changeMode, getLockers } from '../../../redux/api/lockerApi';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../confirm/ConfirmModal';

function ChangeLockerMode({ ids, action = 'Suspend', onClose }) {
    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);
    const handle = () => {
        console.log(ids, "ids");
        
        if (ids.length) {
            const data = { ids, state: action === 'Suspend' ? 2 : 1 };
            dispatch(changeMode(data))
                .unwrap()
                .then((response) => {
                    if (response && response.payload.isSuccess) {
                        toast.success("Mode changed successfully");
                        dispatch(getLockers());
                        onClose();
                    }
                })
            onClose();
        } else {
            onClose();
        }
    }
    return (
        <>
            <button
                className='bg-gray-600 text-white rounded'
                onClick={() => setShowConfirm(true)}>
                {action} Locker(s)
            </button>

            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to change the locker mode?'}
                    onConfirm={() => {
                        handle();
                        setShowConfirm(false);
                    }}
                    onCancel={() => setShowConfirm(false)}
                />
            )}
        </>
    )
}

export default ChangeLockerMode

