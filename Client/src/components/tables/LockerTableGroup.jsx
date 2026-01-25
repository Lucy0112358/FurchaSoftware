import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import OfficeName from '../headers/OfficeName';
import GroupName from '../headers/GroupName';
import GenerateLocker from '../lockers/GenerateLocker';
import LockerPopup from '../popups/locker/LockerPopup';
import { getAllLockersData, getSelectedLockerIds, setSelectedLockerIds } from '../../redux/slice/lockerSlice';
import UnselectLockers from '../button/UnselectLockers';

function LockerTableGroup() {
  const dispatch = useDispatch();
  const selectedLockerIds = useSelector(getSelectedLockerIds)
  const allLockers = useSelector(getAllLockersData);

  const [popup, setPopup] = useState({
    visible: false,
    x: 0,
    y: 0,
    locker: null,
    branchId: null
  });

  const handleRightClick = (e, locker = null) => {
    e.preventDefault();
    let popupX, popupY;
    if (locker) {
      const rect = e.currentTarget.getBoundingClientRect();
      popupX = rect.left + window.scrollX + 10;
      popupY = rect.top + window.scrollY + rect.height + 5;
    } else {
      popupX = e.clientX + window.scrollX;
      popupY = e.clientY + window.scrollY;
    }
    if (!locker && selectedLockerIds.length === 0) {
      return closePopup();
    }

    setPopup({
      visible: true,
      x: popupX,
      y: popupY,
      locker: locker,
      branchId: locker ? locker.branchId : null,
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
                      <GroupName name={lockerGroup.groupName} />

                      <div
                        className="flex flex-wrap mb-4 ">
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
                              : ''
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
                            <GenerateLocker item={item} index={itemIndex} />

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
                    zIndex: 999,
                  }}
                  onClick={(e) => e.stopPropagation()}
                >
                  <LockerPopup locker={popup.locker} onClose={closePopup} branchId={popup.branchId} />
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
