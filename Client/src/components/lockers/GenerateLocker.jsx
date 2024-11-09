import React from 'react'
import Personal from './personal/Personal';
import TemporaryPersonal from './temporary-personal/TemporaryPersonal';
import Common from './common/Common';
import Hand from './hand/Hand';
import Parcel from './parcel/Parcel';
import Unspecified from './unspecified/Unspecified';

function GenerateLocker({ item, index }) {
  const renderLocker = () => {
    // const name = item.user ? getInitials(item.user) : '';
    const incrementedIndex = index + 1;
    const name = 'JB';
    switch(item.type) {
      case 'personal':
        return <Personal label={name} lockernumber={incrementedIndex} />;
      case 'temporary':
        return <TemporaryPersonal label={name} lockernumber={incrementedIndex} />;
      case 'common':
        return <Common label={name} lockernumber={incrementedIndex} />;
      case 'hand':
        return <Hand lockernumber={incrementedIndex} />;
      case 'parcel':
        return <Parcel label={name} lockernumber={incrementedIndex} size="L" orderNum="44623598" />;
      case 'unspecified':
        return <Unspecified lockernumber={incrementedIndex} />;
      default:
        return <div>No matching type</div>;
    }
  }

  const getInitials = (name) => {
    if (!name) return ''; 
    const names = name.split(" "); 
    const initials = names.map(n => n.charAt(0).toUpperCase()).join(""); 
    return initials;
  }

  return (
    <div>
      {renderLocker()}
    </div>
  );
}

export default GenerateLocker;
