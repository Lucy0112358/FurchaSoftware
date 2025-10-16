import React from 'react'
import './personal.scss';
import { getNameStartLetter } from '../../../Utils';

function Personal({ name, lockernumber, nameShort, item }) {
  const fullName = item.users ? item.users[0] : '';
  const shortName = getNameStartLetter(fullName);

  return (
    <div
      className="personal__locker"
      title={`Name: ${nameShort}: locker number: ${lockernumber}`}
    >
      <div className="design-box">
        <div className="label" title={fullName}>
            {shortName}
          </div>
        <div className="number">{lockernumber}</div>
      </div>
    </div>
  )
}

export default Personal