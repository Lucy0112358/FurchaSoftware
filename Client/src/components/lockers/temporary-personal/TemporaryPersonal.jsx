import React from 'react';
import './temporaryPersonal.scss';
import { getNameStartLetter } from '../../../Utils';

function TemporaryPersonal({ label, lockernumber, item, color }) {
  const fullName = item.users ? item.users[0] : '';
  const shortName = getNameStartLetter(fullName);

  return (
    <div className="temporary__locker" title={`Name: ${fullName || ''}`} style={{ backgroundColor: color }}>
      <div className="locker">
        <div className="locker__number">{lockernumber}</div>
        <div className="mini-part"></div>
        <div className="label">
          <div className="label-value">
            {shortName}
          </div>
        </div>
      </div>
    </div>
  );
}

export default TemporaryPersonal;