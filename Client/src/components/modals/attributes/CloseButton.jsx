import React from 'react';

const CloseButton = ({
  onClick,
  children,
  className = '',
  ...props
}) => {
  return (
    <button
      onClick={onClick}
      className={`text-gray-400 hover:text-gray-200 text-2xl ${className}`}
      {...props}
    >
      {children}
    </button>
  );
};

export default CloseButton;
