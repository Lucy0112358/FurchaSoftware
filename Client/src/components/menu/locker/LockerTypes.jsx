import React from 'react';
import './locker-types.css';

const LockerTypes = () => {
  const lockerColors = [
    { background: '#4dd0e1' }, // Голубой
    { background: '#f06292' }, // Розовый
    { background: '#81c784' }, // Зеленый
    { background: '#ffeb3b' }, // Желтый
    { background: '#ffccbc', border: '2px solid #ff8a65' } // Оранжевый с серым
  ];

  return (
    <div className="locker-types">
      <h4>Locker types</h4>
      <div className="locker-container">
        {lockerColors.map((color, index) => (
          <div
            key={index}
            className="locker-box"
            style={color}
          >
            <div className="circle"></div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default LockerTypes;
