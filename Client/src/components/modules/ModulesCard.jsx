import React from 'react';

const ModulesCard = ({firstLocker, lastLocker}) => {
  return (
    <div style={{ width: '150px' }} className="bg-yellow-400 text-gray-800 font-medium text-center py-4 px-6 border-2 border-yellow-600 rounded-md">
      {firstLocker}-{lastLocker}
    </div>
  );
};

export default ModulesCard;
