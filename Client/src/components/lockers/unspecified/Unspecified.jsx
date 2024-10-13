import React from 'react'
import './unspecified.scss';

function Unspecified({ lockernumber }) {
  return (
    <div className='unspecified__locker'>
      <div className="design-box">
        <div className="number">{lockernumber}</div>
      </div>
    </div>
  )
}

export default Unspecified