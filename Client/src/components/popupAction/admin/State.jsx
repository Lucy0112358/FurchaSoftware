import React from 'react';
import Change from './state/Change';
import ChangeMulti from './state/ChangeMulti';

function State({ admin = {}, ids = [], onClose }) {
    return (
        <>
            {
                ids?.length > 0
                    ? <ChangeMulti ids={ids} onClose={onClose} />
                    : <Change admin={admin} onClose={onClose} />
            }
        </>
    );
}

export default State;
