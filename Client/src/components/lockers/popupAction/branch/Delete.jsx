import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import { deleteBranch } from '../../../../redux/api/branchApi';
import ConfirmModal from '../../../confirm/ConfirmModal';

function Delete({ branch, onClose }) {
    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleDelete = () => {
        if (branch?.id) {
            dispatch(deleteBranch(branch.id))
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
                Delete
            </button>

            {showConfirm && (
                <ConfirmModal
                
                    message={'Are you sure you want to delete the branch?'}
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
