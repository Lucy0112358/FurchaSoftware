import React from 'react'
import { userTableGroups } from '../../data/tableIHeads'
import { useSelector } from 'react-redux';
import { getAllGroupsData } from '../../redux/slice/groupSlice';
import NoData from '../no-data/NoData';

function UserTableGroup() {
  const allUserGroups = useSelector(getAllGroupsData);

  return (
    <>
      {
        allUserGroups.length ?
          <div className="outlet__table__wrapper overflow-x-auto mt-2"
            style={{ height: allUserGroups?.length >= 10 ? '480px' : 'auto' }}>
            <table className="outlet__table min-w-full bg-white " style={{ color: '#AAAAAA'}}>
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
            </table>
          </div> : <NoData text="No Groups" />}
    </>
  )
}

export default UserTableGroup