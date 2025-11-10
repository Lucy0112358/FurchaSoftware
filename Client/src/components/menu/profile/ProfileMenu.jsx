import { useState, useRef, useEffect } from 'react';
import '../menu.css';
import MediaQuery from 'react-responsive'
import UserInfoModal from '../../userInfo/UserInfoModal';
import Manage from '../../manage/Manage';


function ProfileMenu() {

    return (
        <>
            <div className="flex justify-end">
                <div className="flex items-center text-white flex-col">
                    <MediaQuery minWidth={550}>
                        <UserInfoModal />
                    </MediaQuery>
                    <Manage />
                </div>
            </div>
        </>
    );
}

export default ProfileMenu