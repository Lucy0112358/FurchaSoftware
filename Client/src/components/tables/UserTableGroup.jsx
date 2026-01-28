import React, { useState } from 'react'
import { userTableGroups } from '../../data/tableIHeads'
import { useSelector } from 'react-redux';
import { getAllGroupsData } from '../../redux/slice/groupSlice';
import NoData from '../no-data/NoData';
import UnselectIds from '../button/UnselectIds';
import CustomCheckbox from '../checkbox/CustomCheckbox';
import { useContextMenu } from '../../hooks/useContextMenu';
import UserGroupPopup from '../popups/userGroup/UserGroupPopup';
import { getManage } from '../../redux/slice/systemSlice';

function UserTableGroup() {
  const allUserGroups = useSelector(getAllGroupsData);
  const [selectedIds, setSelectedIds] = useState([]);
  const manage = useSelector(getManage);

  const handleSelectLocker = (itemId) => {
    setSelectedIds((prevSelected) => {
      const isSelected = prevSelected.includes(itemId);
      if (isSelected) {
        return prevSelected.filter((id) => id !== itemId);
      } else {
        return [...prevSelected, itemId];
      }
    });
  };

  const {
    popup,
    handleRightClick,
    handleGlobalClick,
    closePopup,
  } = useContextMenu(selectedIds);


  return (
    <div onContextMenu={(e) => e.preventDefault()}
      onClick={handleGlobalClick}>
      <div className='flex justify-end mb-5'>
        <UnselectIds setSelectedIds={setSelectedIds} buttonText="Unselect" />
      </div>
      {
        allUserGroups.length ?
          <div className="outlet__table__wrapper overflow-x-auto mt-2"
            style={{ height: allUserGroups?.length >= 10 ? '480px' : 'auto' }}>
            <table className="outlet__table min-w-full bg-white " style={{ color: '#AAAAAA' }}>
              <thead>
                <tr className="outlet__table__header">
                  {userTableGroups.map((header) => (
                    <th className="text-left" >{header}</th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {allUserGroups.map((group, index) => (
                  <tr
                    key={group.id}
                    className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}
                    onContextMenu={(e) => handleRightClick(e, group)}>
                    <td className='flex items-center'>
                      <CustomCheckbox
                        checked={selectedIds.includes(group.id)}
                        onChange={() => handleSelectLocker(group.id)}
                      />
                      {group.id}
                    </td>
                    <td>{group.name}</td>
                    <td>{group.userCount}</td>
                    <td>
                      {group.permittedLockers?.map((locker, idx) => (
                        <>
                          <div key={idx}>{locker.lockerGroupName}</div>
                        </>
                      ))}
                    </td>
                    <td>
                      {group.branchNames?.map((branch, idx) => (
                        <div key={idx}>{branch}</div>
                      ))}
                    </td>
                    <td className={group.state === 1 ? 'text-green-500' : 'text-red-500 '}>
                      {group.state === 1 ? 'Active' : 'Suspended'}
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
                  zIndex: 1,
                }}
                onClick={(e) => e.stopPropagation()}
              >
                <UserGroupPopup
                  userGroup={popup.target}
                  clearSelected={() => setSelectedIds([])}
                  onClose={closePopup}
                  selectedIds={selectedIds}
                  manage={manage}
                />
              </div>
            )}
          </div> : <NoData text="No Groups" />}
    </div>
  )
}

export default UserTableGroup 