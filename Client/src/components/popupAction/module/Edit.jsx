import React, { useState } from 'react'
import EditModulesModal from '../../modals/modules/EditModulesModal';

function Edit({ module, onClose }) {
    const [showModal, setShowModal] = useState(false);

    return (
        <>
            <button className='bg-gray-600 text-white rounded' onClick={() => { setShowModal(true)}}>
                Edit
            </button>

            {showModal && <EditModulesModal id={module?.id} onClose={() => setShowModal(false)} />}
        </>
    )
}

export default Edit