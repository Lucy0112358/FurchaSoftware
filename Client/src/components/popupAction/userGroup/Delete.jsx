import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../confirm/ConfirmModal';
import { deleteUserGroups } from '../../../redux/api/groupApi';

function Delete({ ids, onClose, clearSelected, manage = true }) {
    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleDelete = () => {
        if (ids?.length > 0) {
            dispatch(deleteUserGroups(ids))
                .unwrap()
                .then((res) => {
                    toast.success("User group(s) deleted successfully");
                    clearSelected();
                    onClose();
                })
                .catch((err) => {
                    toast.error(err?.message || "Error deleting user group(s)");
                });
        }
    };

    return (
        <>
            <button
                className='bg-gray-600 text-white rounded'
                onClick={() => setShowConfirm(true)}
                disabled={!manage}
            >
                Delete user group(s)
            </button>

            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to delete the user group?'}
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
