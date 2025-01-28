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

  //Generate lockers for testing
  // const lockerTypes = ["common", "hand", "personal", "temporary", "parcel", "unspecified"];

  // const generateGroupLockers = (groupSize) => {
  //   const groupLockers = [];
  //   for (let i = 1; i <= groupSize; i++) {
  //     const type = lockerTypes[Math.floor(Math.random() * lockerTypes.length)];
  //     groupLockers.push({
  //       id: i,
  //       lockerType: type,
  //       user: i % 2 === 0 ? "User " + i : "-", // alternate between occupied and free lockers
  //       status: i % 2 === 0 ? "occupied" : "free", // alternating statuses
  //       state: i % 2 === 0 ? "active" : "suspended", // alternating states
  //     });
  //   }
  //   return groupLockers;
  // };
  // const allLockers = {
  //   data: [
  //     {
  //       officeName: "Tallin office",
  //       lockers: [
  //         {
  //           groupName: "LG1 - 1st floor Lockers",
  //           groupLockers: generateGroupLockers(12), // 12 lockers in this group
  //         },
  //         {
  //           groupName: "LG1 - 2nd floor Lockers",
  //           groupLockers: generateGroupLockers(14), // 14 lockers in this group
  //         },
  //         {
  //           groupName: "LG2 - 1st floor Lockers",
  //           groupLockers: generateGroupLockers(10), // 10 lockers in this group
  //         },
  //         {
  //           groupName: "LG2 - 2nd floor Lockers",
  //           groupLockers: generateGroupLockers(15), // 15 lockers in this group
  //         },
  //         {
  //           groupName: "LG3 - 1st floor Lockers",
  //           groupLockers: generateGroupLockers(13), // 13 lockers in this group
  //         },
  //       ],
  //     }
  //   ]
  // }

  //end generation


  // const allLockers = useSelector(getAllUsersData);
  // const allLockers = {
  //   data: [
  //     {
  //       officeName: "Tallin office",
  //       lockers: [
  //         {
  //           groupName: "LG1 - 1st floor Lockers",
  //           groupLockers: [
  //             {
  //               id: 1,
  //               type: "common",
  //               user: "Jeck Dwwsoe",
  //               status: "occupied",
  //               state: "active",
  //             },
  //             {
  //               id: 2,
  //               type: "hand",
  //               user: "John Doe",
  //               status: "occupied",
  //               state: "active",
  //             },
  //           ],
  //         },
  //         {
  //           groupName: "LG1 - 2nd floor Lockers",
  //           groupLockers: [
  //             {
  //               id: 3,
  //               type: "personal",
  //               user: "-",
  //               status: "free",
  //               state: "suspended",
  //             },
  //             {
  //               id: 4,
  //               type: "temporary",
  //               user: "Alice Smith",
  //               status: "occupied",
  //               state: "active",
  //             },
  //             {
  //               id: 5,
  //               type: "parcel",
  //               user: "Martha Jone",
  //               status: "occupied",
  //               state: "active",
  //             },
  //           ],
  //         },
  //         {
  //           groupName: "LG2 - 1st floor Lockers",
  //           groupLockers: [
  //             {
  //               id: 6,
  //               type: "unspecified",
  //               user: "James Brown",
  //               status: "occupied",
  //               state: "active",
  //             },
  //           ],
  //         },
  //       ],
  //     }
  //   ]
  // };
  const allLockers = useSelector(getAllLockersData);

  //Part right click
  const [popup, setPopup] = useState({
    visible: false,
    x: 0,
    y: 0,
    locker: null,
  });

  // Правый клик по локеру
  const handleRightClick = (e, locker) => {
    e.preventDefault(); // отключаем стандартный контекст
console.log(locker, 9999999);

    // Расчёт координат (можно брать e.clientX / e.clientY
    // или ориентироваться на позицию самого элемента)
    const rect = e.currentTarget.getBoundingClientRect();

    // Например, рисуем попап чуть ниже и правее самого локера
    const popupX = rect.left + window.scrollX + 10;
    const popupY = rect.top + window.scrollY + rect.height + 5;

    setPopup({
      visible: true,
      x: popupX,
      y: popupY,
      locker: locker,
    });
  };

  // Закрыть попап
  const closePopup = () => {
    setPopup({ ...popup, visible: false });
  };

  // Обработчики кнопок (пример)
  const handleEdit = () => {
    console.log('Edit locker:', popup.locker);
    closePopup();
  };

  const handleOpenLocker = () => {
    console.log('Open locker:', popup.locker);
    closePopup();
  };

  const handleSetUser = () => {
    console.log('Set user for:', popup.locker);
    closePopup();
  };

  const handleSuspendLocker = () => {
    console.log('Suspend locker:', popup.locker);
    closePopup();
  };

  // Закрытие попапа при клике «где угодно» (кроме самого попапа)
  const handleGlobalClick = () => {
    if (popup.visible) {
      closePopup();
    }
  };

  const buttonStyle = {
    display: 'block',
    width: '100%',
    marginBottom: '4px',
    textAlign: 'left',
    backgroundColor: '#ddd',
    border: 'none',
    cursor: 'pointer',
    padding: '4px 6px',
  };

  const renderPopupContent = () => {
    if (!popup.locker) return null;
    return <PopupMenu locker={popup.locker} onClose={closePopup} />;
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
                            {/* {renderPopupContent()} */}
                            {/* <div style={{ fontWeight: 'bold', marginBottom: '8px' }}>
                              Locker #{popup.locker.id}
                            </div>
                            <button style={buttonStyle} onClick={handleEdit}>Edit</button>
                            <button style={buttonStyle} onClick={handleOpenLocker}>Open Locker</button>
                            <button style={buttonStyle} onClick={handleSetUser}>Set User</button>
                            <button style={buttonStyle} onClick={handleSuspendLocker}>Suspend Locker</button> */}
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
