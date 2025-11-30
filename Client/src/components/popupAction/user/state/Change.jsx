import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../../confirm/ConfirmModal';
import { changeUserState, getUsers } from '../../../../redux/api/userApi';

function Change({ user = {}, onClose }) {
    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleChange = () => {

        if (user?.id) {
            let newState;

            if (user.state === 3) {
                newState = 2;
            } else {
                newState = user.state === 1 ? 2 : 1;
            }

            const data = {
                ids: [user.id],
                state: newState,
            };
            dispatch(changeUserState(data))
                .unwrap()
                .then((res) => {
                    toast.success(res.message);
                    dispatch(getUsers());
                    if (onClose) onClose();
                })
                .catch((err) => {
                    toast.error(err?.message || "Ошибка при изменении состояния пользователя");
                });
        }
    };

    return (
        <>
            <button
                className='bg-gray-600 text-white rounded cursor-pointer'
                onClick={() => setShowConfirm(true)}
            >
                {user.state === 1
                    ? 'Suspended'
                    : user.state === 2
                        ? 'Active'
                        : 'Suspended'
                }
            </button>

            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to change user state?'}
                    onConfirm={() => {
                        handleChange();
                        setShowConfirm(false);
                    }}
                    onCancel={() => setShowConfirm(false)}
                />
            )}
        </>
    );
}

export default Change;
