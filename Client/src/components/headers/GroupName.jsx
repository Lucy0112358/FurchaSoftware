import React, { useState } from 'react'
import { CiEdit } from "react-icons/ci";
import LockerGroupModal from '../modals/locker-group/LockerGroupModal'

function GroupName({ name, id, type="allLocker" }) {
  const [isModalOpen, setIsModalOpen] = useState(false)
  const handleOpen = () => setIsModalOpen(true)
  const handleClose = () => setIsModalOpen(false)

  return (
    <div className="flex items-center">
      <h1 className="text-xl" style={{ color: '#AAAAAA' }}>{name}</h1>
      {id ? (
          <button
            onClick={handleOpen}
            className="ml-5 text-xl"
            style={{ color: '#AAAAAA' }}
          >
            <CiEdit className='text-3xl' style={{ color: '#AAAAAA' }} />
          </button>
      ) : null}

      {isModalOpen && (
        <LockerGroupModal
          isOpen={isModalOpen}
          onClose={handleClose}
          id={id}
          type={type}
        />
      )}
    </div>
  )
}

export default GroupName