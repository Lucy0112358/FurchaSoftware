import React, { useState } from 'react'
import { userTable } from '../../data/tableIHeads'
import { useSelector } from 'react-redux';
import { getAllUsersData } from '../../redux/slice/userSlice';
import NoData from '../no-data/NoData';
import CustomCheckbox from '../checkbox/CustomCheckbox';

function UserTable() {
  const allUsers = useSelector(getAllUsersData);
  const [selectedLockerIds, setSelectedLockerIds] = useState([]);

  const handleSelectLocker = (itemId) => {
    setSelectedLockerIds((prevSelected) => {
      const isSelected = prevSelected.includes(itemId);
      if (isSelected) {
        return prevSelected.filter((id) => id !== itemId);
      } else {
        return [...prevSelected, itemId];
      }
    });
      // const newSelected = selectedLockerIds.includes(itemId)
      //   ? selectedLockerIds.filter((id) => id !== itemId)
      //   : [...selectedLockerIds, itemId];
      // dispatch(setSelectedLockerIds(newSelected));
    };

  return (
    <>
      {allUsers?.length ?
        <div className="outlet__table__wrapper overflow-x-auto mt-2"
          style={{ height: allUsers?.length >= 10 ? '480px' : 'auto' }}>
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
                <tr key={user.id} className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}>
                  <td className='flex items-center'>
                    <CustomCheckbox
                      checked={selectedLockerIds.includes(user.id)}
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
        </div> : <NoData text="No Users" />
      }
    </>
  )
}

export default UserTable