import React from 'react'
import './common.scss';

function Common({ label, lockernumber }) {
  return (
    <div className='common__locker'>
      <div className="design-box">
        <div className="corner">
          <div className="corner-white"></div>
        </div>
        <div className="number">{lockernumber}</div>
      </div>
    </div>
  )
}

export default Common