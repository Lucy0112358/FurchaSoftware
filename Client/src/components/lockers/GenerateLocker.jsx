import React from 'react'
import Personal from './personal/Personal';
import TemporaryPersonal from './temporary-personal/TemporaryPersonal';
import Common from './common/Common';
import Hand from './hand/Hand';
import Parcel from './parcel/Parcel';
import Unspecified from './unspecified/Unspecified';

function GenerateLocker({ item, index, doorState }) {
  const renderLocker = () => {
    const incrementedIndex = index + 1;
    const name = 'JB';
    const nameShort = 'Joohn Brain';
    console.log(item, 'wwwwwwwwwwwwwwwwwwwwwwwww');
    
    switch(item.lockerType?.type.toLowerCase()) {
      case 'personal':
        return <Personal name={name} nameShort={nameShort} lockernumber={incrementedIndex} item={item} />;
      case 'temporary':
        return <TemporaryPersonal label={name} lockernumber={incrementedIndex} item={item} />;
      case 'common':
        return <Common label={name} lockernumber={incrementedIndex} doorState={doorState}/>;
      case 'handover':
        return <Hand lockernumber={incrementedIndex} />;
      case 'parcel':
        return <Parcel item={item} lockernumber={incrementedIndex} size="L" orderNum="44623598" />;
      case 'unspecified':
        return <Unspecified lockernumber={incrementedIndex} />;
      default:
        return <div>No matching type</div>;
    }
  }

  return (
    <div>
      {renderLocker()}
    </div>
  );
}

export default GenerateLocker;
