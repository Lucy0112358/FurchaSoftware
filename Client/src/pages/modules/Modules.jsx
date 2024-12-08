/* eslint-disable no-unused-vars */
import axios from 'axios';
import React, { useEffect } from "react";
import NoData from '../../components/no-data/NoData';
import modules from '../../data/modules.json'
import OfficeName from '../../components/headers/OfficeName';
import GroupName from '../../components/headers/GroupName';
import ModulesCard from '../../components/modules/ModulesCard';
import { useDispatch, useSelector } from 'react-redux';
import { getModules } from '../../redux/api/moduleApi';
import { getModulesData } from '../../redux/slice/moduleSlice';

const Modules = () => {

    // const allModules = modules
    const allModules = useSelector(getModulesData);
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(getModules())
    }, []);

    console.log(allModules.data, "allModules");
    
    return (
        <div>
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
                                            <div key={itemIndex} className="mr-2 mb-2">
                                                <ModulesCard firstLocker={item.firstLocker} lastLocker={item.lastLocker} />
                                            </div>
                                        ))}
                                    </div>
                                </React.Fragment>
                            ))}
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
