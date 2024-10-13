import React from 'react'
import './personal.scss';

function Personal({ label, lockernumber }) {
  return (
    <div className='personal__locker'>
      <div className="design-box">
        <div className="label">{label}</div>
        <div className="number">{lockernumber}</div>
      </div>
    </div>
  )
}

export default Personal