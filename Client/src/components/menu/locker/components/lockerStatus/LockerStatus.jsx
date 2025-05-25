import { useState } from 'react';
import { LockerStatusEnum } from '../../../../../enums/Locker/Status';

function LockerStatus({ addFilters }) {
  const [status, setStatus] = useState('all');

  const handleStatusChange = (selectedStatus) => {
    setStatus(selectedStatus);
    if(selectedStatus === 'all') {
      selectedStatus = null;
    }
    addFilters(selectedStatus, 'isOpen');
  };

  return (
    <div className="flex flex-col text-gray-300 rounded-lg">
      <div className="flex space-x-4">
        <label className="flex flex-col ">
          <input
            type="radio"
            name="isOpen"
            value="all"
            checked={status === 'all'}
            onChange={() => handleStatusChange ('all')}
            className="form-radio text-black focus:ring-0 cursor-pointer"
          />
          <span className={`${status === 'all' ? 'text-white' : 'text-gray-400'}`}>All</span>
        </label>
        <label className="flex flex-col ">
          <input
            type="radio"
            name="isOpen"
            value="free"
            checked={status === LockerStatusEnum.free}
            onChange={() => handleStatusChange (LockerStatusEnum.free)}
            className="form-radio text-black focus:ring-0 cursor-pointer"
          />
          <span className={`${status === LockerStatusEnum.free ? 'text-white' : 'text-gray-400'}`}>Free</span>
        </label>
        <label className="flex flex-col ">
          <input
            type="radio"
            name="isOpen"
            value={LockerStatusEnum.occupied}
            checked={status === LockerStatusEnum.occupied}
            onChange={() => handleStatusChange (LockerStatusEnum.occupied)}
            className="form-radio text-gray-400 focus:ring-0 cursor-pointer"
          />
          <span className={`${status === LockerStatusEnum.occupied ? 'text-white' : 'text-gray-400'}`}>Occupied</span>
        </label>
      </div>
      <label className="text-white block">Locker Status</label>

    </div>
  );
}

export default LockerStatus;
