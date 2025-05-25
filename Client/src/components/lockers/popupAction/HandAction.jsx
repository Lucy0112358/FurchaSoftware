import React from 'react'

function HandAction({ lockers, onClose }) {
    return (
        <button className='bg-gray-600 text-white rounded' onClick={() => { console.log('HandAction'); onClose(); }}>
            HandAction
        </button>
    )
}

export default HandAction