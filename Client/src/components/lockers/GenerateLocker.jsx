import React from 'react'
import Personal from './personal/Personal';
import TemporaryPersonal from './temporary-personal/TemporaryPersonal';
import Common from './common/Common';
import Hand from './hand/Hand';
import Parcel from './parcel/Parcel';
import Unspecified from './unspecified/Unspecified';

function GenerateLocker({ item, index, doorState }) {
  const renderLocker = () => {
    const name = 'JB';
    const nameShort = 'Joohn Brain';
    
    switch(item.lockerType?.type.toLowerCase()) {
      case 'personal':
        return <Personal name={name} nameShort={nameShort} lockernumber={index} item={item} />;
      case 'temporary':
        return <TemporaryPersonal label={name} lockernumber={index} item={item} />;
      case 'common':
        return <Common label={name} lockernumber={index} doorState={doorState}/>;
      case 'handover':
        return <Hand lockernumber={index} />;
      case 'parcel':
        return <Parcel item={item} lockernumber={index} size="L" orderNum="44623598" />;
      case 'unspecified':
        return <Unspecified lockernumber={index} />;
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
