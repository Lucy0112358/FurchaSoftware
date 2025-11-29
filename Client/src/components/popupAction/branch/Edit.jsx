import React, { useState } from 'react';
import BranchModal from '../../modals/branch/BranchModal';

function Edit({ branch, onClose }) {
  const [showModal, setShowModal] = useState(false);

  const handleEdit = () => {
    setShowModal(true);
  };

  const handleClose = () => {
    setShowModal(false);
    if (onClose) onClose();
  };

  return (
    <>
      <button className="bg-gray-600 text-white rounded" onClick={handleEdit}>
        Edit
      </button>

      {showModal && (
        <BranchModal
          mode="edit"
          initialData={branch}
          onClose={handleClose}
        />
      )}
    </>
  );
}

export default Edit;
