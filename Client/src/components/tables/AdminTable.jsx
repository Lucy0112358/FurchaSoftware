import React, { useState } from 'react';
import { adminTable } from '../../data/tableIHeads';
import { useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import { getAllAdminsData } from '../../redux/slice/adminSlice';
import CustomCheckbox from '../checkbox/CustomCheckbox';
import UnselectIds from '../button/UnselectIds';
import AdminPopup from '../popups/admin/AdminPopup';
import { useContextMenu } from '../../hooks/useContextMenu';

function AdminTable() {
  const allAdmins = useSelector(getAllAdminsData);
  const [selectedIds, setSelectedIds] = useState([]);

  const handleSelectLocker = (itemId) => {
    setSelectedIds((prevSelected) => {
      const isSelected = prevSelected.includes(itemId);
      return isSelected
        ? prevSelected.filter((id) => id !== itemId)
        : [...prevSelected, itemId];
    });
  };

  const {
    popup,
    handleRightClick,
    handleGlobalClick,
    closePopup,
  } = useContextMenu(selectedIds);

  return (
    <div onContextMenu={(e) => e.preventDefault()} onClick={handleGlobalClick}>
      <div className="flex justify-end mb-5">
        <UnselectIds setSelectedIds={setSelectedIds} buttonText="Unselect" />
      </div>

      {allAdmins?.length ? (
        <div
          className="outlet__table__wrapper overflow-x-auto mt-2"
          style={{ height: allAdmins.length >= 10 ? '480px' : 'auto' }}
        >
          <table
            className="outlet__table min-w-full bg-white"
            style={{
              color: '#AAAAAA',
              borderRadius: allAdmins.length >= 10 ? '0px' : '10px',
            }}
          >
            <thead>
              <tr className="outlet__table__header">
                {adminTable.map((header, index) => (
                  <th key={index} className="text-left">{header}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {allAdmins.map((admin, index) => (
                <tr
                  key={admin.id}
                  className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}
                  onContextMenu={(e) => handleRightClick(e, admin)}
                >
                  <td>
                    <CustomCheckbox
                      checked={selectedIds.includes(admin.id)}
                      onChange={() => handleSelectLocker(admin.id)}
                    />
                    {admin.id}
                  </td>
                  <td>{admin.name}</td>
                  <td>{admin.surname}</td>
                  <td>{admin.role}</td>
                  <td>
                    {admin.cards?.map((card, idx) => (
                      <div key={idx}>{card.cardNumber}</div>
                    ))}
                  </td>
                  <td>
                    {admin.branches?.map((branch, idx) => (
                      <div key={idx}>{branch}</div>
                    ))}
                  </td>
                  <td className={admin.state === 'Suspended' ? 'text-red-500' : ''}>
                    {admin.state}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {popup.visible && (
            <div
              style={{
                position: 'absolute',
                top: popup.y,
                left: popup.x,
                zIndex: 999,
              }}
              onClick={(e) => e.stopPropagation()}
            >
              <AdminPopup
                admin={popup.target}
                onClose={closePopup}
                selectedIds={selectedIds}
              />
            </div>
          )}
        </div>
      ) : (
        <NoData text="No Admins" />
      )}
    </div>
  );
}

export default AdminTable;