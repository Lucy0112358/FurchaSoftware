import React, { useEffect, useState } from 'react'
import AddUserModal from '../../modals/user/AddUserModal';
import { useDispatch, useSelector } from 'react-redux';
import { getUserData } from '../../../redux/slice/userSlice';
import { userShow } from '../../../redux/api/userApi';

function Edit({ id, onClose }) {
    const dispatch = useDispatch();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const userData = useSelector(getUserData);
    console.log(userData, 'userData');

    useEffect(() => {
        if (id) {
            dispatch(userShow({ id }));
        }
    }, [id]);

    const handleCloseModal = () => {
        setIsModalOpen(false);
        if (onClose) onClose();
    };

    return (
        <>
            <button className='bg-gray-600 text-white rounded' onClick={() => setIsModalOpen(true)}>
                Edit
            </button>
            <AddUserModal
                isOpen={isModalOpen}
                onClose={handleCloseModal}
                mode="edit"
                initialData={userData}
            />
        </>
    )
}

export default Edit
