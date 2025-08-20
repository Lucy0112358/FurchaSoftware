/* eslint-disable no-unused-vars */
import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from 'react-redux';
import NoData from '../../components/no-data/NoData';
import './modules.css';
import OfficeName from '../../components/headers/OfficeName';
import ModuleChain from '../../components/modules/PhotoCreator/ModuleChain';
import { getModules } from '../../redux/api/moduleApi';
import { getModulesData } from '../../redux/slice/moduleSlice';
import LockerPhoto from '../../components/modules/PhotoCreator/LockerPhoto';
import EditModulesModal from '../../components/modals/modules/EditModulesModal';

const Modules = () => {
    const allModules = useSelector(getModulesData);
    const [editItemId, setEditItemId] = useState(null);
    const [showModal, setShowModal] = useState(false);

    const dispatch = useDispatch();

    const handleRightClick = (e, target) => {
        e.preventDefault();
        setEditItemId(target.id);
        setShowModal(true);
    };

    useEffect(() => {
        dispatch(getModules());
    }, [dispatch]);

    return (
        <div
            className="select-none"
            onContextMenu={(e) => e.preventDefault()}
        >
            {showModal && <EditModulesModal id={editItemId} onClose={() => setShowModal(false)} />}
            {allModules.length ? (
                allModules.map((branch, index) =>
                    branch.modules?.length > 0 && (
                        <React.Fragment key={index}>
                            <OfficeName name={branch.branchName} />
                            <div className='modules-part'>
                                <div className='mb-4 flex flex-col'>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th className="py-3 px-4 text-left text-gray-300 border-none">Module Chain</th>
                                                <th className="py-3 px-4 text-left text-gray-300 border-none">Lockers Range</th>
                                                  {/* <th className="py-3 px-4 text-left text-gray-300 border-none">Access Control</th> */}
                                                {/* <th className="py-3 px-4 text-left text-gray-300">Alarm System</th> */}
                                            </tr>
                                        </thead>
                                        {branch.modules.map((module) => (
                                            <tbody
                                                key={module.id}
                                                onContextMenu={(e) => handleRightClick(e, module)}
                                            >
                                                <tr>
                                                    <td className="py-3 px-4 border-none">
                                                        <ModuleChain id={module.id} />
                                                    </td>
                                                    <td className="py-3 px-4 border-none">
                                                        {module.lockerRange && <LockerPhoto lockerRange={module.lockerRange} />}
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
                            </div>
                        </React.Fragment>
                    )
                )
            ) : (
                <NoData text="No Modules" />
            )}
        </div>
    );
};

export default Modules;
