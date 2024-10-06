import React from 'react'
import { userTable } from '../../data/tableIHeads'
import { useSelector } from 'react-redux';
import { getAllUsersData } from '../../redux/slice/userSlice';

function UserTable() {
  const allUsers = useSelector(getAllUsersData);

  return (
    <table className="outlet__table min-w-full bg-white " style={{color: '#AAAAAA', minWidth: '1110px'}}>
          <thead>
            <tr className="outlet__table__header">
              {userTable.map((header) => (
                <th className="text-left" >{header}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {allUsers.map((user, index) => (
              <tr key={user.id} className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}>
                <td>
                  <input type="checkbox" className="mr-2" /> {user.id}
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
  )
}

export default UserTable