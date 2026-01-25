import React from 'react';

function ConfirmModal({ message, onConfirm, onCancel }) {
    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 w-80 shadow-xl text-center">
                <p className="mb-4">{message}</p>
                <div className="flex justify-around mt-4">
                    <button
                        className="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
                        onClick={onConfirm}
                    >
                        Yes
                    </button>
                    <button
                        className="bg-gray-300 text-black px-4 py-2 rounded hover:bg-gray-400"
                        onClick={onCancel}
                    >
                        Cancel
                    </button>
                </div>
            </div>
        </div>
    );
}

export default ConfirmModal;