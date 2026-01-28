import React, { useEffect, useState } from 'react'
import AddUserModal from '../../modals/user/AddUserModal';
import { useDispatch, useSelector } from 'react-redux';
import { getUserData } from '../../../redux/slice/userSlice';
import { userShow } from '../../../redux/api/userApi';
import AddUserGroupModal from '../../modals/user/addUserGroup/AddUserGroupModal';
import { userGroupShow } from '../../../redux/api/groupApi';
import { getUserGroupData } from '../../../redux/slice/groupSlice';

function Edit({ id, onClose, manage = true }) {
    const dispatch = useDispatch();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const data = useSelector(getUserGroupData);

    useEffect(() => {
        if (id) {
            dispatch(userGroupShow({id}));
        }
    }, [id]);

    const handleCloseModal = () => {
        setIsModalOpen(false);
        onClose();
    };

    return (
        <>
            <button className='bg-gray-600 text-white rounded' disabled={!manage} onClick={() => setIsModalOpen(true)}>
                Edit
            </button>
            <AddUserGroupModal
                isOpen={isModalOpen}
                onClose={handleCloseModal}
                mode="edit"
                initialData={data}
            />
        </>
    )
}

export default Edit

