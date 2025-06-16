import React, { useState } from 'react';
import { lockerTable } from '../../data/tableIHeads';
import { useDispatch, useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import OfficeName from '../headers/OfficeName';
import GroupName from '../headers/GroupName';
import LockerPopup from '../popups/locker/LockerPopup';
import { getAllLockersData, getSelectedLockerIds, setSelectedLockerIds } from '../../redux/slice/lockerSlice';
import CustomCheckbox from '../checkbox/CustomCheckbox';
import UnselectLockers from '../button/UnselectLockers';
import { useContextMenu } from '../../hooks/useContextMenu';

function LockerTable() {
  const dispatch = useDispatch();
  const allLockers = useSelector(getAllLockersData);
  const selectedLockerIds = useSelector(getSelectedLockerIds);

  const {
    popup,
    handleRightClick,
    handleGlobalClick,
    closePopup,
  } = useContextMenu(selectedLockerIds);

  const handleSelectLocker = (itemId) => {
    const newSelected = selectedLockerIds.includes(itemId)
      ? selectedLockerIds.filter((id) => id !== itemId)
      : [...selectedLockerIds, itemId];
    dispatch(setSelectedLockerIds(newSelected));
  };

  return (
    <div 
      className='table-main select-none'
      onClick={handleGlobalClick}
      onContextMenu={(e) => e.preventDefault()}
    >
      <div className='flex justify-end'>
        <UnselectLockers />
      </div>
      {allLockers.length ? (
        allLockers.map((locker, index) => (
          <React.Fragment key={index}>
            {locker.lockers.length === 0 ? null : (
              <>
                <OfficeName name={locker.officeName} />
                {locker.lockers.map((lockerGroup, groupIndex) => (
                  <React.Fragment key={groupIndex}>
                    {lockerGroup.groupLockers.length !== 0 ? (
                      <>
                        <div className="ml-2 mt-3">
                          <GroupName name={lockerGroup.groupName} />
                        </div>

                        <div
                          className="outlet__table__wrapper overflow-x-auto mt-2"
                          style={{
                            height: lockerGroup.groupLockers.length >= 10 ? '480px' : 'auto',
                          }}
                        >
                          <table
                            className="outlet__table min-w-full bg-white"
                            style={{
                              color: '#AAAAAA',
                              minWidth: '1110px',
                              borderRadius: lockerGroup.groupLockers.length >= 10 ? '0px' : '10px',
                            }}
                          >
                            <thead>
                              <tr className="outlet__table__header">
                                {lockerTable.map((header, headerIndex) => (
                                  <th key={headerIndex} className="text-left">
                                    {header}
                                  </th>
                                ))}
                              </tr>
                            </thead>
                            <tbody>
                              {lockerGroup.groupLockers.map((item, itemIndex) => (
                                <tr
                                  key={item.id}
                                  onContextMenu={(e) => handleRightClick(e, item)}
                                  className={itemIndex % 2 === 0 ? 'bg-gray-50' : 'bg-white'}
                                >
                                  <td className='flex items-center'>
                                    <CustomCheckbox
                                      checked={selectedLockerIds.includes(item.id)}
                                      onChange={() => handleSelectLocker(item.id)}
                                    />
                                    {item.id}
                                  </td>
                                  <td>{'Locker name'}</td>
                                  <td>{item.lockerType}</td>
                                  <td>
                                    {item.users?.map((userName, i) => (
                                      <span key={i}>
                                        {userName}
                                        {i < item.users.length - 1 && ', '}
                                      </span>
                                    ))}
                                  </td>
                                  <td>{item.isOpen ? 'open' : 'closed'}</td>
                                  <td className={item.state === 'suspended' ? 'text-red-500 capitalize' : 'capitalize'}>
                                    {item.state}
                                  </td>
                                </tr>
                              ))}
                            </tbody>
                          </table>
                        </div>
                      </>
                    ) : null}
                  </React.Fragment>
                ))}
              </>
            )}
          </React.Fragment>
        ))
      ) : (
        <NoData text="No Lockers" />
      )}

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
          <LockerPopup locker={popup.target} onClose={closePopup} branchId={popup.branchId} />
        </div>
      )}
    </div>
  );
}

export default LockerTable;
