import React from 'react';
import Change from './state/Change';
import ChangeMulti from './state/ChangeMulti';
import { getAdminData } from '../../../redux/slice/adminSlice';
import { useSelector } from 'react-redux';

function State({ ids = [], onClose }) {
    const admin = useSelector(getAdminData);
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
