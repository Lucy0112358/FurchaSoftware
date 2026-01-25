import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../confirm/ConfirmModal';
import { suspendUserGroups } from '../../../redux/api/groupApi';

function Suspend({ ids, onClose }) {
    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleDelete = () => {
        if (ids?.length > 0) {
            dispatch(suspendUserGroups(ids))
                .unwrap()
                .then((res) => {
                    toast.success(res.message);
                    onClose();
                })
                .catch((err) => {
                    toast.error(err?.message || "Ошибка при удалении");
                });
        }
    };

    return (
        <>
            <button
                className='bg-gray-600 text-white rounded cursor-pointer'
                onClick={() => setShowConfirm(true)}
            >
                Suspend user group(s)
            </button>

            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to suspend the user group?'}
                    onConfirm={() => {
                        handleDelete();
                        setShowConfirm(false);
                    }}
                    onCancel={() => setShowConfirm(false)}
                />
            )}
        </>
    );
}

export default Suspend;
