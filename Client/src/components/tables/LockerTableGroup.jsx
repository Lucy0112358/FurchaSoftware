import React from 'react';
import { lockerTable } from '../../data/tableIHeads';
import { useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import OfficeName from '../headers/OfficeName';
import GroupName from '../headers/GroupName';
import GenerateLocker from '../lockers/GenerateLocker';
import { getAllLockersData } from '../../redux/slice/lockerSlice';
import { useState } from 'react';
import PopupMenu from '../popups/locker/PopupMenu';

function LockerTableGroup() {
  const allLockers = useSelector(getAllLockersData);

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

  return (
    <div>
      {allLockers.length ? (
        allLockers.map((locker, index) => (
          <React.Fragment key={index}>
            <OfficeName name={locker.officeName} />
            <div className='pl-5'
              onClick={handleGlobalClick}>
              {locker.lockers.map((lockerGroup, groupIndex) => (
                <React.Fragment key={groupIndex}>
                  <GroupName name={lockerGroup.groupName} />
                  <div className='flex flex-wrap mb-4'>
                    {lockerGroup.groupLockers.map((item, itemIndex) => (
                      <div
                        className="mr-2 mb-2"
                        onContextMenu={(e) => handleRightClick(e, item)}
                        style={{
                          cursor: 'context-menu',
                        }}
                      >
                        <GenerateLocker item={item} index={itemIndex} />
                      </div>

                    ))}
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
              ))}
            </div>
          </React.Fragment>
        ))
      ) : (
        <NoData text="No Lockers" />
      )}
    </div>
  );
}

export default LockerTableGroup;
