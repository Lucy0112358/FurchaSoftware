import React from 'react'
import './temproraryPersonal.scss';

function TemporaryPersonal({ label, lockernumber }) {
  return (
    <div className='temprorary__locker'>
      <div class="locker">
        <div className='locker__number'>
          {lockernumber}
        </div>
        <div class="mini-qwer"></div>
        <div class="very-mini">
          <div class="aaaa">
            {label}
          </div>
        </div>
      </div>
    </div>
  )
}

export default TemporaryPersonal