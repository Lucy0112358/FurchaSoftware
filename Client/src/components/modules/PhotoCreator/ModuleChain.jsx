import React from 'react';
import { assets } from '../../../assets/assets';

const ModuleChain = ({ id }) => {
  return (
    <div style={{ width: '135px' }} className='flex items-center justify-center'>
      <div className='text-left text-gray-300 mr-2'>{id}</div>
      <img src={assets.modules_icon} alt="modules" />
    </div>
  );
};

export default ModuleChain;
