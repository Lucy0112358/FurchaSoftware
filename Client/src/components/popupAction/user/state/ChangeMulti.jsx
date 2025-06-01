import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../../confirm/ConfirmModal';
import { changeUserState } from '../../../../redux/api/userApi';
import CustomSelect from '../../../select/CustomSelect';

function ChangeMulti({ ids = [], onClose }) {
    const [selectedStateValue, setSelectedStateValue] = useState(null);
    const options = [
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
            <CustomSelect
                options={options}
                onChange={handleChangeState}
            />
            {
                selectedStateValue && <button
                    className='bg-gray-600 text-white rounded cursor-pointer'
                    onClick={() => setShowConfirm(true)}
                >
                    Save
                </button>
            }


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

export default ChangeMulti;
