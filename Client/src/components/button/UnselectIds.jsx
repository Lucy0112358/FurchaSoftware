import React from 'react';

function UnselectIds({ setSelectedIds, buttonText="Unselect" }) {
  const handleUnselect = () => {
    setSelectedIds([]);
  };

  return (
    <button
      type="button"
      className="rounded bg-white px-2 py-1 mr-5"
      onClick={handleUnselect}
    >
      {buttonText}
    </button>
  );
}

export default UnselectIds;
