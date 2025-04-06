import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import { lockerTable } from '../../data/tableIHeads';
import NoData from '../no-data/NoData';
import OfficeName from '../headers/OfficeName';
import GroupName from '../headers/GroupName';
import GenerateLocker from '../lockers/GenerateLocker';
import PopupMenu from '../popups/locker/PopupMenu';
import { getAllLockersData } from '../../redux/slice/lockerSlice';
import { FaTimesCircle } from "react-icons/fa";

function LockerTableGroup() {
  const allLockers = useSelector(getAllLockersData);
  const [selectedLockerId, setSelectedLockerId] = useState([]);

  const [popup, setPopup] = useState({
    visible: false,
    x: 0,
    y: 0,
    locker: null,
  });

  const handleRightClick = (e, locker) => {
    e.preventDefault();
    const rect = e.currentTarget.getBoundingClientRect();
    const popupX = rect.left + window.scrollX + 10;
    const popupY = rect.top + window.scrollY + rect.height + 5;

    setPopup({
      visible: true,
      x: popupX,
      y: popupY,
      locker: locker,
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
    setSelectedLockerId((prevSelected) => {
      if (!prevSelected.includes(itemId)) {
        return [...prevSelected, itemId];
      }
      return prevSelected;
    });
  };

  const handleClickBranchSelect = (itemId) => {
    setSelectedLockerId((prevSelected) => {
      if (prevSelected.includes(itemId)) {
        return prevSelected.filter((id) => id !== itemId);
      } else {
        return [...prevSelected, itemId];
      }
    });
  };

  const handleBranchSelectRemove = (itemId) => {
    setSelectedLockerId((prevSelected) => {
      if (prevSelected.includes(itemId)) {
        return prevSelected.filter((id) => id !== itemId);
      }
      return prevSelected;
    });
  };

  return (
    <div className='select-none' onContextMenu={(e) => e.preventDefault()}>
      <div className='flex'>
        <button type="button" className='text-white rounded' onClick={() => setSelectedLockerId([])}>Unselect Lockers</button>
      </div>
      {allLockers.length ? (
        allLockers.map((locker, index) => {
          if (!locker.lockers || locker.lockers.length === 0) {
            return null;
          }

          return (
            <React.Fragment key={index}>
              <OfficeName name={locker.officeName} />
              <div className="pl-5" onClick={handleGlobalClick}>
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

                      <div className="flex flex-wrap mb-4 ">
                        {lockerGroup.groupLockers.map((item, itemIndex) => (
                          <div
                            key={itemIndex}
                            onContextMenu={(e) => handleRightClick(e, item)}
                            className={`mr-2 mb-2 ${selectedLockerId.includes(item.id)
                              ? 'selected__branch__id'
                              : ''
                              }`}
                            onMouseOver={(event) => {
                              if (event.buttons === 1) {
                                handleBranchSelectAdd(item.id);
                              } else if (event.buttons === 2) {
                                handleBranchSelectRemove(item.id);
                              }
                            }}
                          onClick={() => handleClickBranchSelect(item.id)}
                          >
                            <GenerateLocker item={item} index={itemIndex} />

                          </div>
                          // <div
                          //   key={itemIndex} // или item.id, если уникально
                          //   className="mr-2 mb-2"
                          //   onContextMenu={(e) => handleRightClick(e, item)}
                          //   style={{ cursor: 'context-menu' }}
                          // >
                          //    
                          // </div>
                        ))}

                        {/* Popup-меню (одно на все локеры) */}
                        {popup.visible && (
                          <div
                            style={{
                              position: 'absolute',
                              top: popup.y,
                              left: popup.x,
                              backgroundColor: '#eee',
                              border: '1px solid #ccc',
                              borderRadius: '4px',
                              padding: '5px 10px',
                              zIndex: 999,
                            }}
                            onClick={(e) => e.stopPropagation()}
                          >
                            <PopupMenu locker={popup.locker} onClose={closePopup} />
                          </div>
                        )}
                      </div>
                    </React.Fragment>
                  );
                })}
              </div>
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
