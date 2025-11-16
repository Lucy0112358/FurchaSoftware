import { useState, useRef, useEffect } from 'react';
import '../menu.css';
import { assets } from '../../../assets/assets';
import { useDispatch } from 'react-redux';
import { getBranches } from '../../../redux/api/menuApi';
import { TbPlugConnected } from "react-icons/tb";
import MediaQuery from 'react-responsive';
import Connection from '../../connection/Connection';
import ModulesModal from '../../modals/modules/ModulesModal';
import UserInfoModal from '../../userInfo/UserInfoModal';
import { useHasPermission } from '../../../hooks/useHasPermission';
import ModalActionButton from '../../button/ModalActionButton';
import Manage from '../../manage/Manage';

function ModulesMenu() {
  const dispatch = useDispatch();
  const [isModulesModalOpen, setIsModulesModalOpen] = useState(false);
  const { hasPermission } = useHasPermission();

  useEffect(() => {
    dispatch(getBranches());
  }, [dispatch]);

  return (
    <>
      <div className="menu flex justify-around">
        {
          hasPermission() && <ModalActionButton
            onClick={() => setIsModulesModalOpen(true)}
            iconSrc={assets.add_icon}
            text="Add Modules"
          />
        }
        {isModulesModalOpen && <ModulesModal onClose={() => setIsModulesModalOpen(false)} />}
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
    </>
  );
}

export default ModulesMenu;