import React, { useState, useRef, useEffect } from 'react';
import { MdLogout } from "react-icons/md";
import { logout } from '../../redux/api/userApi';
import { useDispatch, useSelector } from 'react-redux';
import { getAuthUserData } from '../../redux/slice/authSlice';
import { CgProfile } from 'react-icons/cg';
import { Link } from 'react-router-dom';

const UserInfoModal = () => {
    const [open, setOpen] = useState(false);
    const ref = useRef();
    const dispatch = useDispatch();
    const authUser = useSelector(getAuthUserData);

    useEffect(() => {
        const handleClickOutside = (e) => {
            if (ref.current && !ref.current.contains(e.target)) {
                setOpen(false);
            }
        };
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    const handleClick = () => {
        dispatch(logout());
    }

    return (
        <div className="relative" ref={ref}>
            <button
                onClick={() => setOpen(!open)}
                className="flex items-center space-x-2"
            >
                <div className="flex flex-col items-end">
                    <span>{authUser.name}</span>
                    {authUser.role && <span>{authUser.role}</span>}
                </div>
            </button>

            {open && (
                <div className="absolute right-0 mt-2 w-56 bg-white border rounded-lg shadow-lg z-50">
                    <div className="p-4 border-b">
                        <div className="text-base text-gray-500">{authUser.email}</div>
                    </div>

                    <Link
                        to="/profile"
                        className="w-full text-left px-4 py-2 hover:bg-gray-100 border rounded-lg flex items-center"
                    >
                        <div className='flex items-center space-x-2'>
                            <CgProfile className='text-gray-500' />
                            <p className='text-lg text-gray-500'>Profile</p>
                        </div>
                    </Link>
                    <button
                        className="w-full text-left px-4 py-2 hover:bg-gray-100 border rounded-lg flex items-center"
                        onClick={handleClick}
                    >
                        <div className='flex items-center space-x-2'>
                            <MdLogout className='text-gray-500' />
                            <p className='text-lg text-gray-500'>Logout</p>
                        </div>
                    </button>
                </div>
            )}
        </div>
    );
};

export default UserInfoModal;
