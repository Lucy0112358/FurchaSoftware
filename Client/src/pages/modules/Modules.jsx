/* eslint-disable no-unused-vars */
import axios from 'axios';
import React, { useEffect, useRef, useState } from "react";
import NoData from '../../components/no-data/NoData';
import modules from '../../data/fake/modules.json'
import './modules.css';
import OfficeName from '../../components/headers/OfficeName';
import GroupName from '../../components/headers/GroupName';
import ModuleChain from '../../components/modules/PhotoCreator/ModuleChain';
import { useDispatch, useSelector } from 'react-redux';
import { getModules } from '../../redux/api/moduleApi';
import { getModulesData } from '../../redux/slice/moduleSlice';
import { useContextMenu } from '../../hooks/useContextMenu';
import data from '../../data/fake/modules.json'; // Assuming this is the correct path to your fake data
import LockerPhoto from '../../components/modules/PhotoCreator/LockerPhoto';
import AccessControl from '../../components/modules/PhotoCreator/AccessControl';
import AlarmSystem from '../../components/modules/PhotoCreator/AlarmSystem';
import EditModulesModal from '../../components/modals/modules/EditModulesModal';

const Modules = () => {
    const allModules = useSelector(getModulesData);
    // const allModules = data.data;
    const [editItemId, setEditItemId] = useState(null);
    const [showModal, setShowModal] = useState(false);
    console.log(allModules, 'allModules');

    const dispatch = useDispatch();

    const handleRightClick = (e, target) => {
        e.preventDefault();
        setEditItemId(target.id);
        setShowModal(true);
        console.log('target', target);
        
    }

    useEffect(() => {
        dispatch(getModules())
    }, []);

    return (
        <div
            className="select-none"
            onContextMenu={(e) => e.preventDefault()}
        >
            {showModal && <EditModulesModal id={editItemId} onClose={() => setShowModal(false)} />}
            {allModules.length ? (
                allModules.map((branch, index) => (
                    <React.Fragment key={index}>
                        <OfficeName name={branch.branchName} />
                        <div className='modules-part'>
                            <React.Fragment>
                                <div className='mb-4 flex flex-col'>
                                    <table className="">
                                        <thead className="">
                                            <tr>
                                                <th className="py-3 px-4 text-left text-gray-300 border-none">Module Chain</th>
                                                <th className="py-3 px-4 text-left text-gray-300  border-none">Lockers Range</th>
                                                {/* <th className="py-3 px-4 text-left text-gray-300 border-none">Access Control</th> */}
                                                {/* <th className="py-3 px-4 text-left text-gray-300">Alarm System</th> */}
                                            </tr>
                                        </thead>
                                        {branch.modules.map((module) => (
                                            <tbody
                                                onContextMenu={(e) => handleRightClick(e, module)}>
                                                <tr
                                                    key={module.id}
                                                    className=""
                                                >
                                                    <td className="py-3 px-4 border-none">
                                                        <ModuleChain id={module.id} />
                                                    </td>
                                                    <td className="py-3 px-4 border-none">
                                                        {module.lockersRange && <LockerPhoto lockerRange={module.lockersRange} />}
                                                    </td>
                                                    {/* <td className="py-3 px-4 border-none">
                                                        <AccessControl accessControl={module.accessControl} />
                                                    </td> */}
                                                    {/* <td className="py-3 px-4 border-none">
                                                        <AlarmSystem alarmSystem={module.alarmSystem} />
                                                    </td> */}
                                                </tr>
                                            </tbody>
                                        ))}
                                    </table>
                                </div>
                            </React.Fragment>
                        </div>
                        
                    </React.Fragment>
                ))
            ) : (
                <NoData text="No Modules" />
            )}
        </div>
    );
};

export default Modules;