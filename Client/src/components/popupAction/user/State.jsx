import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import ConfirmModal from '../../confirm/ConfirmModal';
import { deleteUsers } from '../../../redux/api/userApi';
import Change from './state/Change';
import ChangeMulti from './state/ChangeMulti';

function State({ user = {}, ids = [], onClose }) {
    console.log(ids, 8888888888888);

    const dispatch = useDispatch();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleDelete = () => {
        // if (ids?.length > 0) {
        //     dispatch(deleteUsers(ids))
        //         .unwrap()
        //         .then((res) => {
        //             toast.success(res.message);
        //             onClose();
        //         })
        //         .catch((err) => {
        //             toast.error(err?.message || "Ошибка при удалении");
        //         });
        // }
    };

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
