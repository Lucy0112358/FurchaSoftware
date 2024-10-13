import React from 'react'
import { userTableGroups } from '../../data/tableIHeads'
import { useSelector } from 'react-redux';
import { getAllGroupsData } from '../../redux/slice/groupSlice';
import NoData from '../no-data/NoData';

function UserTableGroup() {
  const allUserGroups = useSelector(getAllGroupsData);

  console.log(allUserGroups, "allUserGroups");
  

  return (
    <>
    {
      allUserGroups.length ?
        <table className="outlet__table min-w-full bg-white " style={{color: '#AAAAAA', minWidth: '1110px'}}>
          <thead>
            <tr className="outlet__table__header">
              {userTableGroups.map((header) => (
                <th className="text-left" >{header}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {allUserGroups.map((group, index) => (
              <tr key={group.id} className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}>
                <td>
                  {group.id}
                </td>
                <td>{group.name}</td>
                 <td>
                  {group.permittedLockers?.map((locker, idx) => (
                    <>
                      <div key={idx}>{locker.lockerGroupName}</div>
                      <span>(1-16)</span>
                    </>
                  ))}
                </td>
                <td>
                  {group.branchNames?.map((branch, idx) => (
                    <div key={idx}>{branch}</div>
                  ))}
                </td>
                <td className={`${group.state === 'Suspended' ? 'text-red-500' : ''}`}>
                  {group.state}
                </td>
              </tr>
            ))}
          </tbody>
        </table>:  <NoData text="No Groups" />}
    </>
  )
}

export default UserTableGroup