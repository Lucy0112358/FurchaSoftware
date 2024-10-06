import React, { useState } from 'react';
import { assets } from '../../assets/assets';
import AddUserModal from './addUser/AddUserModal';
import { useSelector } from 'react-redux';
import { getSelectGroupSelect } from '../../redux/slice/menuSlice';
import AddUserGroupModal from './addUserGroup/AddUserGroupModal';

function GeneralAddModal() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const userGroupEnabled = useSelector(getSelectGroupSelect);

  return (
    <>
      <div className='menu__add'>
        <button
          onClick={() => setIsModalOpen(true)}
          className="menu__add__button text-white">
          <img className='menu__add__icon' src={assets.add_icon} alt="logo" />
          {/* Изменяем текст кнопки в зависимости от состояния userGroupEnabled */}
          <span className="">
            {userGroupEnabled ? "Add Group User" : "Add User"}
          </span>
        </button>
      </div>

      {/* Условно отображаем соответствующее модальное окно */}
      {
        userGroupEnabled
          ? <AddUserGroupModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
          : <AddUserModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
      }
    </>
  );
}

export default GeneralAddModal;
