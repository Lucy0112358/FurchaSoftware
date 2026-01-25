import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../confirm/ConfirmModal';
import { deleteUsers } from '../../../redux/api/userApi';
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
