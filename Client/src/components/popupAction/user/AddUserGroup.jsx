import React, { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux';
import { getAllGroupsData } from '../../../redux/slice/groupSlice';
import { getAllGroups } from '../../../redux/api/groupApi';
import CustomSelect from '../../select/CustomSelect';
import ConfirmModal from '../../confirm/ConfirmModal';
import { changeUserGroup } from '../../../redux/api/userApi';
import { toast } from 'react-toastify';


function AddUserGroup({ ids, onClose }) {
    const userGroups = useSelector(getAllGroupsData);
    const [selectedValue, setSelectedValue] = useState(null);
    const dispatch = useDispatch();

    const handleChangeState = (selectedOption) => {
        setSelectedValue(selectedOption.value);
    }

    const [showConfirm, setShowConfirm] = useState(false);

    const handleChange = () => {
        if (ids?.length > 0) {
            const data = {
                ids: ids,
                groupId: selectedValue
            }
            dispatch(changeUserGroup(data))
                .unwrap()
                .then((res) => {
                    toast.success(res.message);
                    onClose();
                })
                .catch((err) => {
                    toast.error(err?.message || "Ошибка при изменении група пользователя");
                });
        }
    };

    useEffect(() => {
        if (!userGroups || userGroups.length === 0) {
            dispatch(getAllGroups());
        }
    }, [dispatch, userGroups]);

    const options = [
        { label: 'Select group', value: null },
        ...(
            Array.isArray(userGroups)
                ? userGroups.map(group => ({ label: group.name, value: group.id }))
                : []
        )
    ];


    return (
        <>
            <CustomSelect
                options={options}
                defaultValue={{ label: 'Select group', value: null }}
                onChange={handleChangeState}
            />
             {
                selectedValue && <button
                    className='bg-gray-600 text-white rounded cursor-pointer'
                    onClick={() => setShowConfirm(true)}
                >
                    Add Group
                </button>
            }


            {showConfirm && (
                <ConfirmModal
                    message={'Are you sure you want to change user group?'}
                    onConfirm={() => {
                        handleChange();
                        setShowConfirm(false);
                    }}
                    onCancel={() => setShowConfirm(false)}
                />
            )}
        </>
    )
}

export default AddUserGroup
