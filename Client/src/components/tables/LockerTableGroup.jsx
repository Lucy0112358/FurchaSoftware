import React from 'react';
import { lockerTable } from '../../data/tableIHeads';
import { useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import OfficeName from '../headers/OfficeName';
import GroupName from '../headers/GroupName';
import GenerateLocker from '../lockers/GenerateLocker';

function LockerTableGroup() {

  //Generate lockers for testing
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
            groupLockers: generateGroupLockers(12), // 12 lockers in this group
          },
          {
            groupName: "LG1 - 2nd floor Lockers",
            groupLockers: generateGroupLockers(14), // 14 lockers in this group
          },
          {
            groupName: "LG2 - 1st floor Lockers",
            groupLockers: generateGroupLockers(10), // 10 lockers in this group
          },
          {
            groupName: "LG2 - 2nd floor Lockers",
            groupLockers: generateGroupLockers(15), // 15 lockers in this group
          },
          {
            groupName: "LG3 - 1st floor Lockers",
            groupLockers: generateGroupLockers(13), // 13 lockers in this group
          },
        ],
      }
    ]
  }

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

  return (
    <div>
      {allLockers.data.length ? (
        allLockers.data.map((locker, index) => (
          <React.Fragment key={index}>
            <OfficeName name={locker.officeName} />
            <div className='pl-5'>
              {locker.lockers.map((lockerGroup, groupIndex) => (
                <React.Fragment key={groupIndex}>
                  <GroupName name={lockerGroup.groupName} />
                  <div className='flex flex-wrap mb-4'>
                    {lockerGroup.groupLockers.map((item, itemIndex) => (
                      <dvi className="mr-2 mb-2">
                        <GenerateLocker item={item} index={itemIndex} />
                      </dvi>
                    ))}
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
