import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../../confirm/ConfirmModal';
import { deleteModule, getModules } from '../../../../redux/api/moduleApi';

function Delete({ module, onClose }) {
    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleDelete = () => {
        if (module?.id) {
            dispatch(deleteModule(module.id))
                .unwrap()
                .then((res) => {
                    toast.success(res.message);
                    dispatch(getModules());
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
                className='bg-gray-600 text-white rounded cursor-pointer'
                onClick={() => setShowConfirm(true)}
            >
                Delete
            </button>

            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to delete the module?'}
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