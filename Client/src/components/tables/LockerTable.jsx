import React from 'react';
import { lockerTable } from '../../data/tableIHeads';
import { useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import TemporaryPersonal from '../lockers/temporary-personal/TemporaryPersonal';
import Personal from '../lockers/personal/Personal';
import Common from '../lockers/common/Common';
import Hand from '../lockers/hand/Hand';
import Parcel from '../lockers/parcel/Parcel';
import Unspecified from '../lockers/unspecified/Unspecified';
import OfficeName from '../headers/OfficeName';
import GroupName from '../headers/GroupName';

function LockerTable() {
  // const allLockers = useSelector(getAllUsersData);
  const allLockers = {
    data: [
      {
        officeName: "Tallin office",
        lockers: [
          {
            groupName: "LG1 - 1st floor Lockers",
            groupLockers: [
              {
                id: 1,
                type: "common",
                user: "Jeck Dwwsoe",
                status: "occupied",
                state: 'active',
              },
              {
                id: 2,
                type: "hand",
                user: "John Doe",
                status: "occupied",
                state: 'active',
              }
            ]
          },
          {
            groupName: "LG1 - 2st floor Lockers",
            groupLockers: [
              {
                id: 3,
                type: "common",
                user: "-",
                status: "free",
                state: 'suspended',
              }
            ]
          }
        ]
      }
    ]
  };

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
      {/* 
        <TemporaryPersonal label="TO" lockernumber="87" />
        <Personal label="SJ" lockernumber="87" />
        <Common label="QQ" lockernumber="99" />
        <Hand lockernumber="87" />
        <Parcel label="SJ" lockernumber="7" size="L" orderNum="44623598" />
        <Unspecified lockernumber="7" />
      */}

      {allLockers.data.length ? (
        allLockers.data.map((locker, index) => (
          <React.Fragment key={index}>
            <OfficeName name={locker.officeName} />
            {locker.lockers.map((lockerGroup, groupIndex) => (
              <React.Fragment key={groupIndex}>
                <GroupName name={lockerGroup.groupName} />
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
