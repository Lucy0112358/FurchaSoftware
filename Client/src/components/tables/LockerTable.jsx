import React from 'react';
import { lockerTable } from '../../data/tableIHeads';
import { useSelector } from 'react-redux';
import NoData from '../no-data/NoData';
import OfficeName from '../headers/OfficeName';
import GroupName from '../headers/GroupName';
import { getAllLockersData } from '../../redux/slice/lockerSlice';

function LockerTable() {
  const allLockers = useSelector(getAllLockersData);

  return (
    <div>

      {allLockers.length ? (
        allLockers.map((locker, index) => (
          <React.Fragment key={index}>
            <OfficeName name={locker.officeName} />
            {locker.lockers.map((lockerGroup, groupIndex) => (
              <React.Fragment key={groupIndex}>
                <div className='ml-2 mt-3'>
                  <GroupName name={lockerGroup.groupName} />
                </div>
                <div className="outlet__table__wrapper overflow-x-auto mt-2"
                 style={{ height: lockerGroup?.groupLockers?.length >= 10 ? '480px' : 'auto' }}
                 >
                  <table
                    className="outlet__table min-w-full bg-white"
                    style={{color: '#AAAAAA', minWidth: '1110px', borderRadius: lockerGroup?.groupLockers?.length >= 10 ? '0px' : '10px' }}
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
                          <td>{'Locker name'}</td>
                          <td>{item.lockerType}</td>
                          <td>{item.user}</td>
                          <td>{item.isOpen ? 'open' : 'closed'}</td>
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
