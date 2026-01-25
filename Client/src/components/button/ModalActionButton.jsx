import React from 'react';

const ModalActionButton = ({ onClick, iconSrc, text = "Add", className = "" }) => {
  return (
    <div className={`menu__add ${className}`}>
      <button
        onClick={onClick}
        className="menu__add__button text-white"
      >
        {iconSrc && <img className='menu__add__icon' src={iconSrc} alt="icon" />}
        <span>{text}</span>
      </button>
    </div>
  );
};

export default ModalActionButton;
