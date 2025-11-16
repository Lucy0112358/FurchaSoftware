import React, { useState } from 'react';
import { assets } from '../../../assets/assets';
import AddUserModal from './AddUserModal';
import { useSelector } from 'react-redux';
import { getSelectGroupSelect } from '../../../redux/slice/menuSlice';
import AddUserGroupModal from './addUserGroup/AddUserGroupModal';

function  ContainerAddModal() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const userGroupEnabled = useSelector(getSelectGroupSelect);

  const handleCloseModal = () => {
    setIsModalOpen(false);
  };

  return (
    <>
      <div className='menu__add'>
        <button
          onClick={() => setIsModalOpen(true)}
          className="menu__add__button text-white">
          <img className='menu__add__icon' src={assets.add_icon} alt="logo" />
          <span>{userGroupEnabled ? "Add Group User" : "Add User"}</span>
        </button>
      </div>

      {userGroupEnabled ? (
        <AddUserGroupModal
          isOpen={isModalOpen}
          onClose={handleCloseModal}
        />
      ) : (
        <AddUserModal
          isOpen={isModalOpen}
          onClose={handleCloseModal}
        />
      )}
    </>
  );
}

export default ContainerAddModal;
