import { FaUser } from "react-icons/fa";
import { MdViewModule, MdAdminPanelSettings } from "react-icons/md";
import { AiOutlineBranches } from "react-icons/ai";
import { PiLockersFill } from "react-icons/pi";

export const sitebarItems = [
    { name: 'Branches', path: '/branches', icon : AiOutlineBranches},
    { name: 'Lockers', path: '/lockers', icon : PiLockersFill },
    { name: 'Users', path: '/', icon : FaUser  },
    { name: 'Administrators', path: '/administrators', icon : MdAdminPanelSettings},
    { name: 'Modules', path: '/modules', icon : MdViewModule  },
];