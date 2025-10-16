import React, { useEffect, useRef, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux';
import { getFilteredUsers } from '../../../redux/slice/userSlice';
import { filterUserWithOutPaginte } from '../../../redux/api/userApi';
import { getLockers, setUser } from '../../../redux/api/lockerApi';
import { toast } from "react-toastify";
import CustomSelect from '../../select/CustomSelect';

function SetUser({ lockers, onClose, branchId = null }) {
    const dispatch = useDispatch();
    const filteredUsers = useSelector(getFilteredUsers);
    const [userOptions, setUserOptions] = useState([]);
    const [selectedUser, setSelectedUser] = useState(null);
    const hasFetched = useRef(false);

    useEffect(() => {
        if (!hasFetched.current) {
            hasFetched.current = true;
            if (lockers && lockers.length > 1) {
                dispatch(filterUserWithOutPaginte({}));
            } else if (lockers && lockers.length === 1) {
                dispatch(filterUserWithOutPaginte({ branchId }));
            }
        }

        let options = filteredUsers.map((user) => ({
            value: user.id,
            label: user.name,
        }));
        setUserOptions(options);
    }, [filteredUsers])

    const handle = () => {
        if (lockers.length) {
            dispatch(setUser({ LockerIds: lockers.map(user => user.id), UserId: selectedUser }))
                .then((response) => {
                    if (response && response.payload.isSuccess) {
                        toast.success("Lockers set successfully");
                        dispatch(getLockers());
                        onClose();
                    }
                })
            onClose();
        } else {
            onClose();
        }
    }

    return (
        <>
            <span>Set User</span>
            <CustomSelect
                options={userOptions}
                value={userOptions.find((option) => option.value === selectedUser)}
                onChange={(e) => setSelectedUser(e.value)}
            />
            {
                selectedUser && (
                    <button className='bg-gray-600 text-white rounded' onClick={handle}>
                        Set User
                    </button>
                )
            }
        </>
    )
}

export default SetUser