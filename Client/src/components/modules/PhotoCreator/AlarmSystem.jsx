import React from 'react'
import { assets } from '../../../assets/assets';

function AlarmSystem({alarmSystem}) {
    return (
        <div style={{ width: '135px' }} className='flex flex-col'>
            <img src={assets.alarm_system} alt="modules" />
            <div className='text-left text-gray-300'>{alarmSystem}</div>
        </div>
    );
}

export default AlarmSystem