import React, { useState } from 'react'
import { userTable } from '../../data/tableIHeads'
import { useSelector } from 'react-redux';
import { getAllUsersData } from '../../redux/slice/userSlice';
import NoData from '../no-data/NoData';
import CustomCheckbox from '../checkbox/CustomCheckbox';
import UnselectUsers from '../button/UnselectUsers';
import UserPopup from '../popups/user/UserPopup';

function UserTable() {
  const allUsers = useSelector(getAllUsersData);
  const [selectedUserIds, setSelectedUserIds] = useState([]);

  const handleSelectLocker = (itemId) => {
    setSelectedUserIds((prevSelected) => {
      const isSelected = prevSelected.includes(itemId);
      if (isSelected) {
        return prevSelected.filter((id) => id !== itemId);
      } else {
        return [...prevSelected, itemId];
      }
    });
    // const newSelected = selectedUserIds.includes(itemId)
    //   ? selectedUserIds.filter((id) => id !== itemId)
    //   : [...selectedUserIds, itemId];
    // dispatch(setSelectedUserIds(newSelected));
  };
  const [popup, setPopup] = useState({
    visible: false,
    x: 0,
    y: 0,
    user: {}
  });

  const handleRightClick = (e, user = null) => {
    e.preventDefault();
    const popupX = e.clientX + window.scrollX;
    const popupY = e.clientY + window.scrollY;

    if (!user && selectedUserIds.length === 0) {
      return closePopup();
    }

    setPopup({
      visible: true,
      x: popupX,
      y: popupY,
      user: user,
    });
  };

  const closePopup = () => {
    setPopup({ ...popup, visible: false });
  };

  const handleGlobalClick = () => {
    if (popup.visible) {
      closePopup();
    }
  };

  return (
    <div onContextMenu={(e) => e.preventDefault()}
      onClick={handleGlobalClick}>
      <div className='flex justify-end mb-5'>
        <UnselectUsers setSelectedUserIds={setSelectedUserIds} />
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
                  <td className='flex items-center'>
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
                      <div key={idx}>{branch.name}</div>
                    ))}
                  </td>
                  <td>
                    {user.userGroups?.map((group, idx) => (
                      <div key={idx}>{group.groupName}</div>
                    ))}
                  </td>
                  <td className={` ${user.state === 'Suspended' ? 'text-red-500' : ''}`}>
                    {user.state}
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
              <UserPopup user={popup.user} onClose={closePopup} selectedIds={selectedUserIds} />
            </div>
          )}
        </div> : <NoData text="No Users" />
      }
    </div>
  )
}

export default UserTable