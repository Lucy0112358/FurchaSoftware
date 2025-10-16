import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import OfficeName from '../headers/OfficeName';
import GroupName from '../headers/GroupName';
import GenerateLocker from '../lockers/GenerateLocker';
import LockerPopup from '../popups/locker/LockerPopup';
import { getAllLockersData, getSelectedLockerIds, setSelectedLockerIds } from '../../redux/slice/lockerSlice';
import UnselectLockers from '../button/UnselectLockers';
import { useContextMenu } from '../../hooks/useContextMenu';

function LockerTableGroup() {
  const dispatch = useDispatch();
  const selectedLockerIds = useSelector(getSelectedLockerIds)
  const allLockers = useSelector(getAllLockersData);

  const {
    popup,
    handleRightClick,
    handleGlobalClick,
    closePopup,
  } = useContextMenu(selectedLockerIds);

  const handleBranchSelectAdd = (itemId) => {
    const newSelected = selectedLockerIds.includes(itemId)
      ? selectedLockerIds
      : [...selectedLockerIds, itemId];
    dispatch(setSelectedLockerIds(newSelected));
  };

  const handleClickBranchSelect = (itemId) => {
    const newSelected = selectedLockerIds.includes(itemId)
      ? selectedLockerIds.filter((id) => id !== itemId)
      : [...selectedLockerIds, itemId];
    dispatch(setSelectedLockerIds(newSelected));
  };

  const handleBranchSelectRemove = (itemId) => {
    const newSelected = selectedLockerIds.includes(itemId)
      ? selectedLockerIds.filter((id) => id !== itemId)
      : selectedLockerIds;
    dispatch(setSelectedLockerIds(newSelected));
  };

  return (
    <div className='select-none' onContextMenu={(e) => e.preventDefault()}>
      <div className='flex justify-end'>
        <UnselectLockers />
      </div>
      {allLockers.length ? (
        allLockers.map((locker, index) => {
          if (!locker.lockers || locker.lockers.length === 0) {
            return null;
          }

          return (
            <React.Fragment key={index}>
              <OfficeName name={locker.officeName} />
              <div className="pl-5"
                onClick={handleGlobalClick}
                onContextMenu={(e) => {
                  if (selectedLockerIds.length > 0) {
                    handleRightClick(e);
                  } else {
                    e.preventDefault();
                  }
                }}>
                {locker.lockers.map((lockerGroup, groupIndex) => {
                  if (
                    !lockerGroup.groupLockers ||
                    lockerGroup.groupLockers.length === 0
                  ) {
                    return null;
                  }

                  return (
                    <React.Fragment key={groupIndex}>
                      <GroupName name={lockerGroup.groupName} id={lockerGroup.id} />
                      <div
                        className="flex flex-wrap mb-4 mt-4">
                        {lockerGroup.groupLockers.map((item, itemIndex) => (
                          <div
                            key={itemIndex}
                            onContextMenu={(e) => {
                              if (!selectedLockerIds.length > 0) {
                                handleRightClick(e, item);
                              } else {
                                e.preventDefault();
                              }
                            }}
                            className={`mr-2 mb-2 ${selectedLockerIds.includes(item.id)
                              ? 'selected__branch__id'
                              : 'locker__border'
                              }`}
                            onMouseOver={(event) => {
                              if (event.buttons === 1 && event.ctrlKey) {
                                handleBranchSelectRemove(item.id);
                              } else if (event.buttons === 1) {
                                handleBranchSelectAdd(item.id);
                              }
                            }}
                            onClick={() => handleClickBranchSelect(item.id)}
                          >
                            <GenerateLocker item={item} index={item.id} doorState={item.doorState || ''} />

                          </div>
                        ))}
                      </div>
                    </React.Fragment>
                  );
                })}
              </div>
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
                  <LockerPopup locker={popup.target} onClose={closePopup} branchId={popup.branchId} />
                </div>
              )}
            </React.Fragment>
          );
        })
      ) : (
        <NoData text="No Lockers" />
      )}
    </div>
  );
}

export default LockerTableGroup;
