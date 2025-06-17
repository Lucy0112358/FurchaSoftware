/* eslint-disable no-unused-vars */
import axios from 'axios';
import React, { useEffect, useRef, useState } from "react";
import NoData from '../../components/no-data/NoData';
import modules from '../../data/fake/modules.json'
import OfficeName from '../../components/headers/OfficeName';
import GroupName from '../../components/headers/GroupName';
import ModulesCard from '../../components/modules/ModulesCard';
import { useDispatch, useSelector } from 'react-redux';
import { getModules } from '../../redux/api/moduleApi';
import { getModulesData } from '../../redux/slice/moduleSlice';
import ModulePopup from '../../components/popups/modules/ModulePopup';
import { useContextMenu } from '../../hooks/useContextMenu';

const Modules = () => {
    const allModules = useSelector(getModulesData);
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

    useEffect(() => {
        dispatch(getModules())
    }, []);

    return (
        <div
            className="select-none"
            onClick={handleGlobalClick}>
            {allModules.length ? (
                allModules.map((moduleGroupe, index) => (
                    <React.Fragment key={index}>
                        <OfficeName name={moduleGroupe.officeName} />
                        <div className='pl-5'>
                            {moduleGroupe.modules.map((groupModules, groupIndex) => (
                                <React.Fragment key={groupIndex}>
                                    <GroupName name={groupModules.groupName} />
                                    <div className='mb-4 flex flex-col'>
                                        {groupModules.groupModules.map((item, itemIndex) => (
                                            <div
                                                key={itemIndex}
                                                className="mr-2 mb-2"
                                                onContextMenu={(e) => handleRightClick(e, item)}>
                                                <ModulesCard firstLocker={item.firstLocker} lastLocker={item.lastLocker} />
                                            </div>
                                        ))}
                                    </div>
                                </React.Fragment>
                            ))}
                        </div>
                        {popup.visible && (
                            <div
                                ref={popupRef}
                                style={{
                                    position: 'absolute',
                                    top: popup.y,
                                    left: popup.x,
                                    zIndex: 999,
                                }}
                                onClick={(e) => e.stopPropagation()}
                            >
                                <ModulePopup module={popup.target} onClose={closePopup} />
                            </div>
                        )}
                    </React.Fragment>
                ))
            ) : (
                <NoData text="No Modules" />
            )}
        </div>
    );
};

export default Modules;
