import React from 'react'
import { branchTable } from '../../data/tableIHeads'
import { useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import { getAllBranchesData } from '../../redux/slice/branchSlice';

function BranchTable() {
  const allBranches = useSelector(getAllBranchesData);

  return (
    <>
      {allBranches?.length ?
        <div className="outlet__table__wrapper overflow-x-auto mt-2"
          style={{ height: allBranches?.length >= 10 ? '480px' : 'auto' }}>
          <table className="outlet__table min-w-full bg-white " style={{ color: '#AAAAAA', minWidth: '1110px' }}>
            <thead>
              <tr className="outlet__table__header">
                {branchTable.map((header, index) => (
                  <th key={index} className="text-left" >{header}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {allBranches.map((branch, index) => (
                <tr key={branch.id} className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}>
                  <td>
                    <input type="checkbox" className="mr-2" /> {branch.id}
                  </td>
                  <td>{branch.name}</td>
                  <td>{branch.address}</td>
                  <td>{branch.comment}</td>
                  <td>
                    {branch.lockerTypes?.toString()}
                  </td>
                  <td>
                    <td>{branch.lockersCount}</td>
                  </td>
                  <td className={` ${branch.mode === 0 ? 'text-red-500' : 'text-green-500'}`}>
                    {branch.mode === 0 ? 'Inactive' : 'Active'}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div> : <NoData text="No Branches" />
      }
    </>
  )
}

export default BranchTable