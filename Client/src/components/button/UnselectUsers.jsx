import React from 'react';

function UnselectUsers({ setSelectedUserIds }) {
  const handleUnselect = () => {
    setSelectedUserIds([]);
  };

  return (
    <button
      type="button"
      className="rounded bg-white px-2 py-1 mr-5"
      onClick={handleUnselect}
    >
      Unselect Users
    </button>
  );
}

export default UnselectUsers;
