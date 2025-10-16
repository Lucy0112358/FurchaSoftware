import React from 'react'
import './common.scss';

function Common({ label, lockernumber, doorState }) {
  return (
    <div className='common__locker'>
      <div className="design-box">
         <div className={`corner ${doorState.toLowerCase() === 'open' ? 'open' : 'close'}`}>
          <div className="corner-white"></div>
        </div>
        <div className="number">{lockernumber}</div>
      </div>
    </div>
  )
}

export default Common