import React from 'react';
import UserPopupMultiItem from './AdminPopupMultiItem';
import Edit from '../../popupAction/admin/Edit';
import Delete from '../../popupAction/admin/Delete';
import State from '../../popupAction/admin/State';
import AdminPopupMultiItem from './AdminPopupMultiItem';


function AdminPopup({ admin, onClose, selectedIds }) {

  return (
    <div
      className="bg-white shadow-xl rounded-lg p-5 w-72 border border-gray-200"
      onClick={(e) => e.stopPropagation()}
    >
      {selectedIds?.length > 0 ? (
        <AdminPopupMultiItem selectedIds={selectedIds} onClose={onClose} />
      ) : (
        admin && (
          <>
            <div className="border-b border-gray-300 pb-2 mb-4">
              <h3 className="text-lg font-bold text-gray-800">
                Admin #{admin.id}
              </h3>
            </div>

            <div className="flex flex-col gap-2">
              <Edit id={admin.id} onClose={onClose} />
              <Delete ids={[admin.id]} onClose={onClose} />
              <State admin={admin} onClose={onClose} />
            </div>
          </>
        )
      )}
    </div>
  );
}

export default AdminPopup;
