import React from 'react'
import Personal from './personal/Personal';
import TemporaryPersonal from './temporary-personal/TemporaryPersonal';
import Common from './common/Common';
import Hand from './hand/Hand';
import Parcel from './parcel/Parcel';
import Unspecified from './unspecified/Unspecified';
import { getLockerColor } from '../../Utils';

function GenerateLocker({ item, index, doorState }) {
  const type = item.lockerType?.type;
  const color = getLockerColor(type);

  const renderLocker = () => {
    const name = 'JB';
    const nameShort = 'Joohn Brain';
    switch (type.toLowerCase()) {
      case 'personal':
        return <Personal name={name} nameShort={nameShort} lockernumber={item.number} item={item} color={color} />;
      case 'temporary':
        return <TemporaryPersonal label={name} lockernumber={item.number} item={item} color={color} />;
      case 'common':
        return <Common label={name} lockernumber={item.number} doorState={doorState} color={color} />;
      case 'handover':
        return <Hand lockernumber={item.number} color={color} />;
      case 'parcel':
        return <Parcel item={item} lockernumber={item.number} size="L" orderNum="44623598" color={color} />;
      case 'unspecified':
        return <Unspecified lockernumber={item.number} color={color} />;
      default:
        return <div>No matching type</div>;
    }
  }

  return (
    <div
      className="locker__border__open"
      style={{ borderColor: doorState? '#f00909' : color }}
    >
      {renderLocker()}
    </div>
  );
}

export default GenerateLocker;
