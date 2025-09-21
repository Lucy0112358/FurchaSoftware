import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../confirm/ConfirmModal';
import { deleteAdmins, getAllAdmins } from '../../../redux/api/adminApi';

function Delete({ ids, onClose }) {
    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleDelete = () => {
        if (ids?.length > 0) {
            dispatch(deleteAdmins(ids))
                .unwrap()
                .then((res) => {
                    toast.success(res.message);
                    dispatch(getAllAdmins());
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
                Delete admin(s)
            </button>

            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to delete the admin?'}
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

export default Delete;
