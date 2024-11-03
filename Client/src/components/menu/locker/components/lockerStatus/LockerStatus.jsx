import { useState } from 'react';

function LockerStatus({ addFilters }) {
  const [status, setStatus] = useState('all');

  const handleStatusChange = (selectedStatus) => {
    setStatus(selectedStatus);
    addFilters(selectedStatus, 'lockerStatus');
  };

  return (
    <div className="flex flex-col text-gray-300 rounded-lg">
      <div className="flex space-x-4">
        <label className="flex flex-col ">
          <input
            type="radio"
            name="lockerStatus"
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
            name="lockerStatus"
            value="free"
            checked={status === 'free'}
            onChange={() => handleStatusChange ('free')}
            className="form-radio text-black focus:ring-0 cursor-pointer"
          />
          <span className={`${status === 'free' ? 'text-white' : 'text-gray-400'}`}>Free</span>
        </label>
        <label className="flex flex-col ">
          <input
            type="radio"
            name="lockerStatus"
            value="occupied"
            checked={status === 'occupied'}
            onChange={() => handleStatusChange ('occupied')}
            className="form-radio text-gray-400 focus:ring-0 cursor-pointer"
          />
          <span className={`${status === 'occupied' ? 'text-white' : 'text-gray-400'}`}>Occupied</span>
        </label>
      </div>
      <label className="text-white block">Locker Status</label>

    </div>
  );
}

export default LockerStatus;
