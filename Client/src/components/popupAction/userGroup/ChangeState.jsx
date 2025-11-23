import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../confirm/ConfirmModal';
import { getAllGroups, changeGroupsState } from '../../../redux/api/groupApi';
import { getUserGroupData } from '../../../redux/slice/groupSlice';

function ChangeState({ ids, action = 'Suspend', onClose }) {
    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleChangeState = () => {
        if (ids?.length > 0) {
            const data = { ids, state: action === 'Suspend' ? 2 : 1 };
            dispatch(changeGroupsState(data))
                .unwrap()
                .then((res) => {
                    toast.success(res.message);
                    dispatch(getAllGroups());
                    onClose();
                })
                .catch((err) => {
                    toast.error(err?.message || "Something went wrong");
                });
        }
    };

    return (
        <>
            <button
                className='bg-gray-600 text-white rounded cursor-pointer'
                onClick={() => setShowConfirm(true)}
            >
                {action} user group(s)
            </button>

            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to change the user group state?'}
                    onConfirm={() => {
                        handleChangeState();
                        setShowConfirm(false);
                    }}
                    onCancel={() => setShowConfirm(false)}
                />
            )}
        </>
    );
}

export default ChangeState;
