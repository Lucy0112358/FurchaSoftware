import React from 'react'
import './parcel.scss';
import { getNameStartLetter } from '../../../Utils';

function Parcel({ label, lockernumber, size, orderNum, item }) {
  const fullName = item.users ? item.users[0] : '';
  const shortName = getNameStartLetter(fullName);

  return (
    <div className="parcel__locker">
      <div className="labels">
        <div className="left-label">{size}</div>
        <div className="right-label" title={fullName}>{shortName}</div>
      </div>
      <div className="number">{lockernumber}</div>
      {/* <div className="order-num">{orderNum}</div> */}
    </div>
  )
}

export default Parcel