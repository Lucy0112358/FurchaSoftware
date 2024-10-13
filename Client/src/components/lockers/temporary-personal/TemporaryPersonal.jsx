import React from 'react'
import './temproraryPersonal.scss';

function TemporaryPersonal({ label, lockernumber }) {
  return (
    <div className='temprorary__locker'>
      <div class="locker">
        <div className='locker__number'>
          {lockernumber}
        </div>
        <div class="mini-part"></div>
        <div class="label">
          <div class="label-value">
            {label}
          </div>
        </div>
      </div>
    </div>
  )
}

export default TemporaryPersonal