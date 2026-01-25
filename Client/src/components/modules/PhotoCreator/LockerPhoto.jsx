import React from 'react'

function LockerPhoto() {
  return (
    <div className="" style={{ width: '110px' }}>
      <div className='grid grid-cols-5 gap-1'>
        {Array.from({ length: 10 }).map((_, i) => (
          <div
            key={i}
            className="w-4 h-4 bg-cyan-500 rounded-sm"
          ></div>
        ))}
      </div>
    </div>
  )
}

export default LockerPhoto