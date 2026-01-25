import React from 'react'
import './personal.scss';
import { getNameStartLetter } from '../../../Utils';

function Personal({ name, lockernumber, nameShort, item, color }) {
  const fullName = item.users ? item.users[0] : '';
  const shortName = getNameStartLetter(fullName);

  return (
    <div
      className="personal__locker "
      title={`Name: ${fullName || ''}`}
      style={{ borderColor: color }}
    >
      <div className="design-box">
        <div className="label">
            {shortName || <p style={{fontSize:'9px', color:'red'}}>{'Free'}</p>}
          </div>
        <div className="number">{lockernumber}</div>
      </div>
    </div>
  )
}

export default Personal