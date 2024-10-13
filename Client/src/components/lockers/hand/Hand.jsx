import React from 'react'
import './hand.scss';

function Hand({ lockernumber }) {
  return (
    <div className='hand__locker'>
      <div className="design-box">
        <div className="number">{lockernumber}</div>
      </div>
    </div>
  )
}

export default Hand