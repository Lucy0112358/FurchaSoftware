import React from 'react'
import './unspecified.scss';

function Unspecified({ lockernumber, color }) {
  return (
    <div className='unspecified__locker' style={{ backgroundColor: color }}>
      <div className="design-box">
        <div className="number">{lockernumber}</div>
      </div>
    </div>
  )
}

export default Unspecified