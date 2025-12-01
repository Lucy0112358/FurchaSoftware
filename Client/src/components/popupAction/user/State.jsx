import React from 'react';
import Change from './state/Change';
import ChangeMulti from './state/ChangeMulti';

function State({ user = {}, ids = [], onClose }) {
    return (
        <>
            {
                ids?.length > 0
                    ? <ChangeMulti ids={ids} onClose={onClose} />
                    : <Change user={user} onClose={onClose} />
            }
        </>
    );
}

export default State;
