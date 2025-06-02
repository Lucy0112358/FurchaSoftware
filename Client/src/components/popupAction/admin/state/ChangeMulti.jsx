import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../../confirm/ConfirmModal';
import CustomSelect from '../../../select/CustomSelect';
import { changeAdminState } from '../../../../redux/api/adminApi';

function ChangeMulti({ ids = [], onClose }) {
    const [selectedStateValue, setSelectedStateValue] = useState(null);
    const options = [
        { label: 'Select state', value: null },
        { label: 'Active', value: 1 },
        { label: 'Suspended', value: 2 },
    ];

    const handleChangeState = (selectedOption) => {
        setSelectedStateValue(selectedOption.value);
    }

    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleChange = () => {
        if (ids?.length > 0) {
            const data = {
                ids: ids,
                state: selectedStateValue
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
            <CustomSelect
                options={options}
                defaultValue={{ label: 'Select state', value: null }}
                onChange={handleChangeState}
            />
            {
                selectedStateValue && <button
                    className='bg-gray-600 text-white rounded cursor-pointer'
                    onClick={() => setShowConfirm(true)}
                >
                    Save State
                </button>
            }


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

export default ChangeMulti;
