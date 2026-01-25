import React from 'react';

const ModalActionButton = ({ onClick, iconSrc, text = "Add", className = "", disabled = false }) => {
  return (
    <div className={`menu__add ${className}`}>
      <button
        onClick={onClick}
        className="menu__add__button text-white"
        disabled={disabled}
      >
        {iconSrc && <img className='menu__add__icon' src={iconSrc} alt="icon" />}
        <span>{text}</span>
      </button>
    </div>
  );
};

export default ModalActionButton;
