import React, { useEffect } from 'react';
import Edit from '../../popupAction/admin/Edit';
import Delete from '../../popupAction/admin/Delete';
import State from '../../popupAction/admin/State';
import AdminPopupMultiItem from './AdminPopupMultiItem';
import { useDispatch } from 'react-redux';
import { adminShow } from '../../../redux/api/adminApi';


function AdminPopup({ admin, onClose, selectedIds }) {
  const dispatch = useDispatch();

  useEffect(() => {
    if (admin.id) {
      dispatch(adminShow({ id: admin.id }));
    }
  }, [admin]);

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
              <Edit onClose={onClose} />
              <Delete ids={[admin.id]} onClose={onClose} />
              <State onClose={onClose} />
            </div>
          </>
        )
      )}
    </div>
  );
}

export default AdminPopup;
