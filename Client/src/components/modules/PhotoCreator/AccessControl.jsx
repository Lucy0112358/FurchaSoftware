import React from 'react'

function AccessControl({ accessControl }) {
    return (
        <div className="">
            <div className='flex gap-1 flex-wrap'>
                {Array.from({ length: 4 }).map((_, i) => (
                    <div
                        key={i}
                        className="w-4 h-8 bg-green-600 rounded-sm"
                    ></div>
                ))}
            </div>

            <div className='text-left text-gray-300'>
                {accessControl}
            </div>
        </div>
    )
}

export default AccessControl