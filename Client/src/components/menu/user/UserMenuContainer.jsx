import { useState, useRef, useEffect } from 'react';
import '../menu.css';
import UserGroupMenu from './UserGroupMenu';
import UserMenu from './UserMenu';
import { getSelectGroupSelect } from '../../../redux/slice/menuSlice';
import { useSelector } from 'react-redux';

function UserMenuContainer() {
    const userGroupEnabled = useSelector(getSelectGroupSelect);

    return (
        !userGroupEnabled ? <UserMenu /> : <UserGroupMenu />
    );
}

export default UserMenuContainer