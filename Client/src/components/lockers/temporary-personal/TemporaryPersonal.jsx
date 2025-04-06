import React from 'react'
import './temproraryPersonal.scss';

function TemporaryPersonal({ label, lockernumber }) {
  return (
    <div className='temprorary__locker'>
      <div className="locker">
        <div className='locker__number'>
          {lockernumber}
        </div>
        <div className="mini-part"></div>
        <div className="label">
          <div className="label-value">
            {label}
          </div>
        </div>
      </div>
    </div>
  )
}

export default TemporaryPersonal