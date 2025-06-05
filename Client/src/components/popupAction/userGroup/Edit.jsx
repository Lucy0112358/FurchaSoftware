import React from 'react'

function Edit({ lockers, onClose }) {
    return (
        <button className='bg-gray-600 text-white rounded' onClick={() => { alert('Coming soon userGroup'); onClose(); }}>
            Edit
        </button>
    )
}

export default Edit
