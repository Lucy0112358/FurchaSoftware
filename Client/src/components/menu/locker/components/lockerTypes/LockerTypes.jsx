import React, { useState } from 'react';
import './locker-types.css';
import { getFilterLockerTypesData } from '../../../../../redux/slice/lockerSlice';
import { useSelector } from 'react-redux';

const LockerTypes = ({ addFilters }) => {
  const [selectedTypeId, setSelectedTypeId] = useState(null);
  const lockerFilterTypes = useSelector(getFilterLockerTypesData);
  
  // const lockerSettings = [
  //   { background: '#4dd0e1', type: 'personal' },
  //   { background: '#f06292', type: 'common' },
  //   { background: '#81c784', type: 'handover' },
  //   { background: '#ffeb3b', type: 'parcel' },
  //   { background: '#ffccbc', border: '2px solid #ff8a65', type: 'unspecified' }
  // ];

  const handleClick = (newTypeId) => {
    setSelectedTypeId((prevTypeId) => {
      const updatedTypeId = prevTypeId === newTypeId ? "" : newTypeId;
      addFilters(updatedTypeId, 'lockerType');
      return updatedTypeId;
    });
  };

  return (
    <div className="locker-types">
      <label className="text-white ">Locker Types</label>
      <div className="locker-container">
        {lockerFilterTypes.map((setting, index) => (
          <div
            key={index}
            className="locker-box cursor-pointer"
            style={{
              ...setting,
              border: selectedTypeId === setting.id ? '2px solid #000' : setting.border || 'none',
            }}
            onClick={() => handleClick(setting.id)}
          >
            <div className="circle"></div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default LockerTypes;
