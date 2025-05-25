import React from 'react';

export default function ShowFormikError({ message }) {
    return (
        <div className="text-red-500 text-sm mt-1">
            {message}
        </div>
    )
}
