import React, { useState } from 'react'; 
import './locker-types.css';

const LockerTypes = ({ addFilters }) => {
  const [selectedType, setSelectedType] = useState(null);

  const lockerSettings = [
    { background: '#4dd0e1', type: 'personal' },
    { background: '#f06292', type: 'common' },
    { background: '#81c784', type: 'handOver' },
    { background: '#ffeb3b', type: 'parcel' },
    { background: '#ffccbc', border: '2px solid #ff8a65', type: 'unspecified' }
  ];

  const handleClick = (newType) => {
    setSelectedType((prevType) => {
      const updatedType = prevType === newType ? "" : newType;
      addFilters(updatedType, 'lockerType');
      return updatedType;
    });
  };

  return (
    <div className="locker-types">
      <label className="text-white ">Locker Types</label>
      <div className="locker-container">
        {lockerSettings.map((setting, index) => (
          <div
            key={index}
            className="locker-box cursor-pointer"
            style={{
              ...setting,
              border: selectedType === setting.type ? '2px solid #000' : setting.border || 'none',
            }}
            onClick={() => handleClick(setting.type)}
          >
            <div className="circle"></div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default LockerTypes;
