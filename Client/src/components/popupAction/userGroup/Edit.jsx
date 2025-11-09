import React, { useEffect, useState } from 'react'
import AddUserModal from '../../modals/user/AddUserModal';
import { useDispatch, useSelector } from 'react-redux';
import { getUserData } from '../../../redux/slice/userSlice';
import { userShow } from '../../../redux/api/userApi';
import AddUserGroupModal from '../../modals/addUserGroup/AddUserGroupModal';

function Edit({ id, onClose }) {
    const dispatch = useDispatch();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const data = useSelector(getUserData);
console.log(data, 'userData');

    useEffect(() => {
        if (id) {
            dispatch(userShow({id}));
        }
    }, [id]);

    const handleCloseModal = () => {
        setIsModalOpen(false);
        onClose();
    };

    return (
        <>
            <button className='bg-gray-600 text-white rounded' onClick={() => setIsModalOpen(true)}>
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

