import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../confirm/ConfirmModal';
import { deleteUsers, getUsers } from '../../../redux/api/userApi';

function Delete({ ids, onClose, clearSelected, manage = true }) {
    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleDelete = () => {
        if (ids?.length > 0) {
            dispatch(deleteUsers(ids))
                .unwrap()
                .then((res) => {
                    toast.success(res.message);
                    dispatch(getUsers())
                    clearSelected();
                    if (onClose) onClose();
                })
                .catch((err) => {
                    toast.error(err?.message || "Ошибка при удалении");
                });
        }
    };

    return (
        <>
            <button
                className='bg-gray-600 text-white rounded'
                disabled={!manage}
                onClick={() => setShowConfirm(true)}
            >
                Delete user(s)
            </button>

            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to delete the user?'}
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
