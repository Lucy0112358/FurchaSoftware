import { useState, useRef } from 'react';
import './menu.css';
import { RiAddBoxLine } from "react-icons/ri";
import { assets } from '../../assets/assets';
import CustomSelect from '../select/CustomSelect';

function Menu() {
  const [groupEnabled, setGroupEnabled] = useState(false);
  const [manageEnabled, setManageEnabled] = useState(true);

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file) {
      console.log('Выбран файл:', file.name);
    }
  };

  const fileInputRef = useRef(null);

  const handleFileClick = () => {
    fileInputRef.current.click();
  };


  return (
    <div className="flex justify-around">
      <div className='menu__add'>
        <button className="menu__add__button text-white">
          <img className='menu__add__icon' src={assets.add_icon} alt="logo" />
          <span className="">Add User</span>
        </button>
      </div>


      <div className="menu__group flex items-center">
        <label className="cursor-pointer">
          <input
            type="checkbox"
            className="sr-only"
            checked={groupEnabled}
            onChange={() => setGroupEnabled(!groupEnabled)}
          />
          <div className='menu__group__general'>
            <div className={`w-10 h-6 bg-gray-400 rounded-full relative transition duration-300 ease-in-out ${groupEnabled ? 'bg-green-500' : ''}`}>
              <div className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transition duration-300 ease-in-out transform ${groupEnabled ? 'translate-x-4' : ''}`}></div>
            </div>
            <span className="text-white">Group</span>
          </div>
        </label>
      </div>

      <div className="menu__filter flex space-x-4">
        <div className='flex flex-col'>
          <div className='menu__filter__select'>
            <CustomSelect />
            <label className="text-white block">Site</label>
          </div>
          <div className='menu__filter__select'>
            <CustomSelect />
            <label className="text-white block">User Group</label>
          </div>
        </div>
        <div className='menu__filter__search'>
          <input
            type="text"
            className="w-full p-2 rounded"
          />
          <label className="text-white block">Search User</label>
        </div>

      </div>

      <div className="flex flex-col items-center space-y-2">
        <div className="flex space-x-2">
          <div className="menu__file__uploader">
            <img src={assets.import_icon} className="cursor-pointer" onClick={handleFileClick} alt="import" />
            <input
              type="file"
              ref={fileInputRef}
              style={{ display: 'none' }}
              onChange={handleFileChange}
            />
          </div>
          <div className="menu__file__uploader">
            <img src={assets.export_icon} className="cursor-pointer" alt="export" />
          </div>
        </div>
        <div>
          <span className='text-white'>Export/Import</span>
        </div>
      </div>


      <div className="menu__connection flex items-start text-white">
        <span className="menu__connection__status"></span>
        <span>Connection</span>
      </div>

      <div className="flex items-center text-white flex-col">
        <div className="flex flex-col items-end">
          <span>Michael</span>
          <span>(Administrator)</span>
        </div>
        <div className="flex items-center ml-4 flex-col mt-3">
          <label className="inline-flex items-center cursor-pointer">
            <input
              type="checkbox"
              className="sr-only"
              checked={manageEnabled}
              onChange={() => setManageEnabled(!manageEnabled)}
            />
            <div className={`w-10 h-6 bg-gray-400 rounded-full relative transition duration-300 ease-in-out ${manageEnabled ? 'bg-green-500' : ''}`}>
              <div className={`w-4 h-4 bg-white rounded-full absolute top-1 left-1 transition duration-300 ease-in-out transform ${manageEnabled ? 'translate-x-4' : ''}`}></div>
            </div>
          </label>
          <span>Manage</span>
        </div>
      </div>
    </div>
  );
}

export default Menu