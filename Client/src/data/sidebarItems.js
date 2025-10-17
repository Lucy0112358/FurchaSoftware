import { FaUser } from "react-icons/fa";
import { MdViewModule, MdAdminPanelSettings } from "react-icons/md";
import { AiOutlineBranches } from "react-icons/ai";
import { PiLockersFill } from "react-icons/pi";
import { AiFillGift } from "react-icons/ai";

export const sitebarItems = [
    { name: 'Branches', path: '/branches', icon : AiOutlineBranches},
    { name: 'Lockers', path: '/lockers', icon : PiLockersFill },
    { name: 'Users', path: '/users', icon : FaUser  },
    { name: 'Parcels', path: '/parcels', icon : AiFillGift },
    { name: 'Administrators', path: '/admins', icon : MdAdminPanelSettings},
    { name: 'Modules', path: '/modules', icon : MdViewModule  },
];