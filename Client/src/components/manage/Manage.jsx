import React from 'react'
import { getManage, setManageEnabled } from '../../redux/slice/systemSlice'
import { useDispatch, useSelector } from 'react-redux';
import { getAuthUserPermissionsData } from '../../redux/slice/authSlice';

function Manage({roles=[]}) {
    const dispatch = useDispatch();
    const manage = useSelector(getManage);
    const userPermissions = useSelector(getAuthUserPermissionsData);
    // const canManage = user?.permissions?.includes(permissionKey);
    // const userPermissions = useSelector(getAuthUserPermissionsData);
    //  if (
    //   userPermissions?.role?.includes('LVL5_MasterAdmin') ||
    //   userPermissions?.role?.includes('LVL4_SuperAdmin')
    // ) return true;
    
    return (
        <div className="manage__page">
            <label className="inline-flex items-center cursor-pointer">
                <input
                    type="checkbox"
                    className="sr-only"
                    checked={manage}
                    onChange={() => dispatch(setManageEnabled(!manage))}
                />
                <div className={`w-10 h-6 bg-gray-400 rounded-full relative transition duration-300 ease-in-out ${manage ? 'bg-green-500' : ''}`}>
                    <div className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transition duration-300 ease-in-out transform ${manage ? 'translate-x-4' : ''}`}></div>
                </div>
            </label>
            <span>Manage</span>
        </div>
    )
}

export default Manage