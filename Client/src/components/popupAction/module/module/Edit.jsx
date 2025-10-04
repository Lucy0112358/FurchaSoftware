import React, { useState } from 'react'
import EditModulesModal from '../../../modals/modules/EditModulesModal';
import ModulesModal from '../../../modals/modules/ModulesModal';

function Edit({ module, onClose }) {
    const [showModal, setShowModal] = useState(false);

    return (
        <>
            <button className='bg-gray-600 text-white rounded' onClick={() => { setShowModal(true)}}>
                Edit
            </button>

            {showModal && <ModulesModal id={module?.id} mode = "edit" onClose={() => setShowModal(false)} />}
        </>
    )
}

export default Edit