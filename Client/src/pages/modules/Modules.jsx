/* eslint-disable no-unused-vars */
import React, { useEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from 'react-redux';
import NoData from '../../components/no-data/NoData';
import './modules.css';
import OfficeName from '../../components/headers/OfficeName';
import ModuleChain from '../../components/modules/PhotoCreator/ModuleChain';
import { getModules } from '../../redux/api/moduleApi';
import { getModulesData } from '../../redux/slice/moduleSlice';
import LockerPhoto from '../../components/modules/PhotoCreator/LockerPhoto';
import EditModulesModal from '../../components/modals/modules/EditModulesModal';
import { useContextMenu } from "../../hooks/useContextMenu";
import ModulePopup from '../../components/popups/modules/ModulePopup';

const Modules = () => {
    const allModules = useSelector(getModulesData);
    const [editItemId, setEditItemId] = useState(null);
    const [showModal, setShowModal] = useState(false);
    const [rightClickColumn, setRightClickColumn] = useState(null);

    const dispatch = useDispatch();
    const {
        popup,
        handleRightClick,
        handleGlobalClick,
        closePopup,
    } = useContextMenu();

    useEffect(() => {
        const handleClickOutside = (e) => {
            if (popup.visible && popupRef.current && !popupRef.current.contains(e.target)) {
                closePopup();
            }
        };
        window.addEventListener('click', handleClickOutside);
        return () => {
            window.removeEventListener('click', handleClickOutside);
        };
    }, [popup.visible]);
    const popupRef = useRef(null);

    // const handleRightClick = (e, target) => {
    //     e.preventDefault();
    //     setEditItemId(target.id);
    //     setShowModal(true);
    // };

    useEffect(() => {
        dispatch(getModules());
    }, [dispatch]);

    const rightClickHandler = (e, module, column) => {
        handleRightClick(e, module);
        setRightClickColumn(column);
    }

    return (
        <div
            className="select-none"
            onClick={handleGlobalClick}
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
                                            >
                                                <tr>
                                                    <td
                                                        className="py-3 px-4 border-none"
                                                        onContextMenu={(e) => rightClickHandler(e, module, 'module')}
                                                    >
                                                        <ModuleChain id={module.id} />
                                                        <h3 className="text-xl ml-[35px] " style={{ color: '#AAAAAA' }}>{module.info}</h3>
                                                    </td>
                                                    <td
                                                        className="py-3 px-4 border-none"
                                                        onContextMenu={(e) => rightClickHandler(e, module, 'locker')}
                                                    >
                                                        {module.lockerRange && <LockerPhoto />}
                                                        <div className="text-[#AAAAAA] flex">
                                                            <h3 className="text-md mr-1 ">
                                                                {module.groupName}
                                                            </h3>
                                                            ({module.lockerRange})
                                                        </div>
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
            {popup.visible && (
                <div
                    style={{
                        position: 'absolute',
                        top: popup.y,
                        left: popup.x,
                        zIndex: 1,
                    }}
                    onClick={(e) => e.stopPropagation()}
                >
                    <ModulePopup module={popup.target} onClose={closePopup} column={rightClickColumn} />
                </div>
            )}
        </div>
    );
};

export default Modules;
