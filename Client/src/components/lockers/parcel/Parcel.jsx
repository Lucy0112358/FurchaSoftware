import React from 'react'
import './parcel.scss';

function Parcel({ label, lockernumber, size, orderNum }) {
  return (
    <div className="parcel__locker">
      <div className="labels">
        <div className="left-label">{size}</div>
        <div className="right-label">{label}</div>
      </div>
      <div className="number">{lockernumber}</div>
      <div className="order-num">{orderNum}</div>
    </div>
  )
}

export default Parcel