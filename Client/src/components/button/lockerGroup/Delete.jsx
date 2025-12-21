import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../confirm/ConfirmModal';
import { deleteUsers, getUsers } from '../../../redux/api/userApi';
import { MdDelete } from 'react-icons/md';
import { MdDeleteOutline } from "react-icons/md";
import { deleteLockerGroup } from '../../../redux/api/lockerGroupApi';
import { getLockers } from '../../../redux/api/lockerApi';

function Delete({ lockerCount, id }) {
    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const checkIsNotEmpty = () => {
        if (lockerCount > 0) {
            toast.error(
                "This locker group cannot be deleted because it contains lockers."
            );
            return true;
        }
        return false;
    };

    const handleDelete = () => {

        dispatch(deleteLockerGroup(id))
            .unwrap()
            .then((res) => {
                toast.success(res.message);
                dispatch(getLockers());
            })
            .catch((err) => {
                toast.error(err?.message || "Ошибка при удалении");
            });
    };

    return (
        <>
            <button
                className='cursor-pointer'
                onClick={() => {
                    if (checkIsNotEmpty()) return;
                    setShowConfirm(true)
                }}
            >
                <MdDeleteOutline className='text-3xl' style={{ color: '#AAAAAA' }} />
            </button>

            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to delete the locker group?'}
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
