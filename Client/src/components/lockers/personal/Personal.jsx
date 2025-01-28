import React from 'react'
import './personal.scss';

function Personal({ name, lockernumber, nameShort }) {
  return (
    <div
      className="personal__locker"
      title={`Name: ${nameShort}: locker number: ${lockernumber}`}
    >
      <div className="design-box">
        <div className="label">{name}</div>
        <div className="number">{lockernumber}</div>
      </div>
    </div>
  )
}

export default Personal