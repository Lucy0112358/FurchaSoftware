import React from 'react';
import './temproraryPersonal.scss';
import { getNameStartLetter } from '../../../Utils';

function TemporaryPersonal({ label, lockernumber, item }) {
  const fullName = item.users ? item.users[0] : '';
  const shortName = getNameStartLetter(fullName);

  return (
    <div className="temprorary__locker" title={`Name: ${fullName}:`}>
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