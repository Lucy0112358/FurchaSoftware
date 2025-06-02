import React, { useState } from 'react';
import State from '../../popupAction/admin/State';
import Delete from '../../popupAction/admin/Delete';

function AdminPopupMultiItem({ selectedIds, onClose }) {
  return (
    <div className="bg-white rounded-xl max-w-md mx-auto">
      <h2 className="text-xl font-semibold text-gray-800 mb-4">Edit Selected Admins</h2>
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700 mb-1">Selected Admin IDs:</label>
        <div className="text-gray-600 text-sm bg-gray-100 p-2 rounded">
          {selectedIds.map((id, i) => (
            <span key={id}>
              {id}{i < selectedIds.length - 1 && ', '}
            </span>
          ))}
        </div>
      </div>
      <div className='flex flex-col gap-2'>
        <Delete ids={selectedIds} onClose={onClose} />
        <State ids={selectedIds} onClose={onClose} />
      </div>
    </div>
  )
}

export default AdminPopupMultiItem;