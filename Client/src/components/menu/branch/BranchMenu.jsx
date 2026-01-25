import { useState, useRef, useEffect } from 'react';
import '../menu.css';
import { assets } from '../../../assets/assets';
import { useDispatch, useSelector } from 'react-redux';
import { TbPlugConnected } from "react-icons/tb";
import MediaQuery from 'react-responsive'
import Connection from '../../connection/Connection';
import BranchModal from '../../modals/branch/BranchModal';
import { getBranches } from '../../../redux/api/branchApi';
import UserInfoModal from '../../userInfo/UserInfoModal';
import Manage from '../../manage/Manage';
import { getManage } from '../../../redux/slice/systemSlice';


function BranchMenu() {
    const dispatch = useDispatch();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [inputValue, setInputValue] = useState('');
    const [debounceTimeout, setDebounceTimeout] = useState(null);
    // const [manageEnabled, setManageEnabled] = useState(true);
    const manage = useSelector(getManage);

    const handleFilterName = (e) => {
        let name = e.target.value;
        setInputValue(name);

        if (debounceTimeout) {
            clearTimeout(debounceTimeout);
        }

        setDebounceTimeout(
            setTimeout(() => {
                dispatch(getBranches({ 'name': name }));
            }, 500)
        );
    };

    return (
        <>
            <div className="menu flex justify-around">
                <div className='menu__add'>
                    <button
                        disabled={!manage}
                        onClick={() => setIsModalOpen(true)}
                        className="menu__add__button text-white">
                        <img className='menu__add__icon' src={assets.add_icon} alt="logo" />
                        <span className="">
                            {"Add Branch"}
                        </span>
                    </button>
                </div>
                {isModalOpen && <BranchModal onClose={() => setIsModalOpen(false)} />}
                <div className="menu__filter flex space-x-4">
                    <div className='menu__filter__search'>
                        <input
                            type="text"
                            value={inputValue}
                            onChange={handleFilterName}
                            className="w-full rounded"
                        />
                        <label className="text-white block">Search</label>
                    </div>
                </div>

                <div className="menu__connection flex items-start text-white">
                    <MediaQuery minWidth={769}>
                        <Connection />
                    </MediaQuery>
                    <MediaQuery maxWidth={768}>
                        <TbPlugConnected className='menu__connection__status__icon' />
                    </MediaQuery>
                </div>

                <div className="flex items-center text-white flex-col">
                    <MediaQuery minWidth={550}>
                        <UserInfoModal />
                    </MediaQuery>
                    <Manage />
                </div>
            </div>
            <div className="menu__filter__mobile hidden">
                <div className='flex justify-between'>
                    <div className='menu__filter__search'>
                        <input
                            type="text"
                            className="w-full p-2 rounded"
                        />
                        <label className="text-white block">Search User</label>
                    </div>
                </div>
            </div>
        </>
    );
}

export default BranchMenu