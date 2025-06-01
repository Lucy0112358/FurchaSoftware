import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../../confirm/ConfirmModal';
import { changeUserState } from '../../../../redux/api/userApi';

function Change({ user={}, onClose }) {
    console.log(user, 8888888888888);

    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleChange = () => {
        
        if (user?.id) {
            console.log(user, 9999999999999999999999999999);
            
            const data = {
                ids: [user.id],
                state: user.state === 'active' ? 2 : 1
            }
            dispatch(changeUserState(data))
                .unwrap()
                .then((res) => {
                    toast.success(res.message);
                    onClose();
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
                {user.state === 'active' ? 'Suspend' : 'Activate'}
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
