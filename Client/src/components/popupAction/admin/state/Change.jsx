import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../../confirm/ConfirmModal';
import { changeAdminState } from '../../../../redux/api/adminApi';

function Change({ admin={}, onClose }) {

    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleChange = () => {
        
        if (admin?.id) {
            const data = {
                ids: [admin.id],
                state: admin.state === 'active' ? 2 : 1
            }
            dispatch(changeAdminState(data))
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
                {admin.state === 'active' ? 'Suspend' : 'Activate'}
            </button>

            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to change admin state?'}
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
