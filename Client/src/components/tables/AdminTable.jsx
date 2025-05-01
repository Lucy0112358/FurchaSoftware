import React from 'react'
import { adminTable, userTable } from '../../data/tableIHeads'
import { useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import { getAllAdminsData } from '../../redux/slice/adminSlice';

function AdminTable() {
  const allAdmins = useSelector(getAllAdminsData);

  return (
    <>
      {allAdmins?.length ?
        <div className="outlet__table__wrapper overflow-x-auto mt-2"
          style={{ height: allAdmins?.length >= 10 ? '480px' : 'auto' }}>
          <table className="outlet__table min-w-full bg-white " style={{ color: '#AAAAAA', minWidth: '1110px' }}>
            <thead>
              <tr className="outlet__table__header">
                {adminTable.map((header, index) => (
                  <th key={index} className="text-left" >{header}</th>
                ))}
              </tr>
            </thead>
            <tbody>

              {allAdmins.map((admin, index) => (
                <tr key={admin.id} className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}>
                  <td>
                    <input type="checkbox" className="mr-2" /> {admin.id}
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
                      <div key={idx}>{branch.name}</div>
                    ))}
                  </td>
                  <td>
                    {admin.userGroups?.map((group, idx) => (
                      <div key={idx}>{group.groupName}</div>
                    ))}
                  </td>
                  <td className={` ${admin.state === 'Suspended' ? 'text-red-500' : ''}`}>
                    {admin.state}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div> : <NoData text="No Admins" />
      }
    </>
  )
}

export default AdminTable