import React from 'react';
import { lockerTable } from '../../data/tableIHeads';
import { useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import OfficeName from '../headers/OfficeName';
import GroupName from '../headers/GroupName';

function LockerTable() {
  // const allLockers = useSelector(getAllUsersData);
  //Generate locker data part

  const lockerTypes = ["common", "hand", "personal", "temporary", "parcel", "unspecified"];
  
  const generateGroupLockers = (groupSize) => {
    const groupLockers = [];
    for (let i = 1; i <= groupSize; i++) {
      const type = lockerTypes[Math.floor(Math.random() * lockerTypes.length)];
      groupLockers.push({
        id: i,
        type: type,
        user: i % 2 === 0 ? "User " + i : "-", // alternate between occupied and free lockers
        status: i % 2 === 0 ? "occupied" : "free", // alternating statuses
        state: i % 2 === 0 ? "active" : "suspended", // alternating states
      });
    }
    return groupLockers;
  };

  const allLockers = {
    data: [
      {
        officeName: "Tallin office",
        lockers: [
          {
            groupName: "LG1 - 1st floor Lockers",
            groupLockers: generateGroupLockers(7), // 12 lockers in this group
          },
          {
            groupName: "LG1 - 2nd floor Lockers",
            groupLockers: generateGroupLockers(2), // 14 lockers in this group
          },
          {
            groupName: "LG2 - 1st floor Lockers",
            groupLockers: generateGroupLockers(10), // 10 lockers in this group
          },
          {
            groupName: "LG2 - 2nd floor Lockers",
            groupLockers: generateGroupLockers(6), // 15 lockers in this group
          },
          {
            groupName: "LG3 - 1st floor Lockers",
            groupLockers: generateGroupLockers(13), // 13 lockers in this group
          },
        ],
      }
    ]
  }
  // const handleRightClick = (event) => {
  //   event.preventDefault();
  //   console.log('Правое касание мыши!');
  // };


  //End Generate Locker data part
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
  //               state: 'active',
  //             },
  //             {
  //               id: 2,
  //               type: "hand",
  //               user: "John Doe",
  //               status: "occupied",
  //               state: 'active',
  //             }
  //           ]
  //         },
  //         {
  //           groupName: "LG1 - 2st floor Lockers",
  //           groupLockers: [
  //             {
  //               id: 3,
  //               type: "common",
  //               user: "-",
  //               status: "free",
  //               state: 'suspended',
  //             }
  //           ]
  //         }
  //       ]
  //     }
  //   ]
  // };

  // const allLockersGrid = {
  //   data: [
  //     {
  //       officeName: "Tallin office",
  //       lockers: [
  //         {
  //           groupName: "LG1 - 1st floor Lockers",
  //           groupLockers: [
  //             {
  //               id: 1,
  //               lockerType: "common",
  //               isActive: true,
  //               isOpen: true,
  //               lockerStatus: "open",
  //               groupId: 6
  //             },
  //             {
  //               id: 2,
  //               lockerType: "hand",
  //               isActive: true,
  //               isOpen: true,
  //               lockerStatus: "open",
  //               groupId: 6
  //             }
  //           ]
  //         },
  //         {
  //           groupName: "LG1 - 2st floor Lockers",
  //           groupLockers: [
  //             {
  //               id: 3,
  //               lockerType: "common",
  //               isActive: true,
  //               isOpen: true,
  //               lockerStatus: "open",
  //               groupId: 6
  //             }
  //           ]
  //         }
  //       ]
  //     }
  //   ]
  // };
  return (
    <div>

      {allLockers.data.length ? (
        allLockers.data.map((locker, index) => (
          <React.Fragment key={index}>
            <OfficeName name={locker.officeName} />
            {locker.lockers.map((lockerGroup, groupIndex) => (
              <React.Fragment key={groupIndex}>
                <div className='ml-2'>
                  <GroupName name={lockerGroup.groupName} />
                </div>
                <div className="outlet__table__wrapper overflow-x-auto mt-2"
                 style={{ height: lockerGroup?.groupLockers?.length >= 10 ? '480px' : 'auto' }}
                 >
                  <table
                    className="outlet__table min-w-full bg-white mb-4 "
                    style={{ color: '#AAAAAA', minWidth: '1110px' }}
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
                        <tr key={item.id} className={`${itemIndex % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}>
                          <td>
                            <input type="checkbox" className="mr-2" /> {item.id}
                          </td>
                          <td>{item.type}</td>
                          <td>{item.user}</td>
                          <td>{item.status}</td>
                          <td className={` ${item.state === 'suspended' ? 'text-red-500 capitalize' : 'capitalize'}`}>
                            {item.state}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>

                </div>
              </React.Fragment>
            ))}
          </React.Fragment>
        ))
      ) : (
        <NoData text="No Lockers" />
      )}
    </div>
  );
}

export default LockerTable;
