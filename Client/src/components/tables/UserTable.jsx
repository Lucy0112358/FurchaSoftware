import React, { useState } from 'react'
import { userTable } from '../../data/tableIHeads'
import { useSelector } from 'react-redux';
import { getAllUsersData } from '../../redux/slice/userSlice';
import NoData from '../no-data/NoData';
import CustomCheckbox from '../checkbox/CustomCheckbox';
import UnselectIds from '../button/UnselectIds';
import UserPopup from '../popups/user/UserPopup';
import { useContextMenu } from '../../hooks/useContextMenu';
import { format, parseISO } from 'date-fns';

function UserTable() {
  const allUsers = useSelector(getAllUsersData);
  const [selectedUserIds, setSelectedUserIds] = useState([]);

  const {
    popup,
    handleRightClick,
    handleGlobalClick,
    closePopup,
  } = useContextMenu(selectedUserIds);

  const handleSelectLocker = (itemId) => {
    setSelectedUserIds((prevSelected) => {
      const isSelected = prevSelected.includes(itemId);
      if (isSelected) {
        return prevSelected.filter((id) => id !== itemId);
      } else {
        return [...prevSelected, itemId];
      }
    });
  };

  return (
    <div onContextMenu={(e) => e.preventDefault()}
      onClick={handleGlobalClick}>
      <div className='flex justify-end mb-5'>
        <UnselectIds setSelectedIds={setSelectedUserIds} />
      </div>
      {allUsers?.length ?
        <div className="outlet__table__wrapper overflow-x-auto mt-2 select-none"
          style={{ height: allUsers?.length >= 10 ? '480px' : 'auto' }}
        >
          <table className="outlet__table min-w-full bg-white " style={{ color: '#AAAAAA' }}>
            <thead>
              <tr className="outlet__table__header">
                {userTable.map((header, index) => (
                  <th key={index} className="text-left" >{header}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {allUsers.map((user, index) => (
                <tr
                  key={user.id}
                  className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}
                  onContextMenu={(e) => handleRightClick(e, user)}>
                  <td>
                    <CustomCheckbox
                      checked={selectedUserIds.includes(user.id)}
                      onChange={() => handleSelectLocker(user.id)}
                    />
                    {user.id}
                  </td>
                  <td>{user.name}</td>
                  <td>{user.surname}</td>
                  <td>{user.role}</td>
                  <td>
                    {user.cards?.map((card, idx) => (
                      <div key={idx}>{card.cardNumber}</div>
                    ))}
                  </td>
                  <td>
                    {user.branches?.map((branch, idx) => (
                      branch.name
                    ))}
                  </td>
                  <td className='w-[157px]'>
                    {user.userGroups?.map((group, idx) => (
                      group.name + ', '
                    ))}
                  </td>
                  <td className={user.state === 1 ? 'text-green-500' : 'text-red-500 '}>
                    {user.state === 1 ? 'Active' : 'Suspended'}
                  </td>
                  <td>{user.activeFrom ? format(parseISO(user.activeFrom), 'yyyy-MM-dd') : '-'}</td>
                  <td>{user.activeTo ? format(parseISO(user.activeTo), 'yyyy-MM-dd') : '-'}</td>
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
                zIndex: 1,
              }}
              onClick={(e) => e.stopPropagation()}
            >
              <UserPopup
                user={popup.target}
                onClose={closePopup}
                clearSelected={() => setSelectedUserIds([])}
                selectedIds={selectedUserIds}
              />
            </div>
          )}
        </div> : <NoData text="No Users" />
      }
    </div>
  )
}

export default UserTable