import { useSelector } from "react-redux";
import { getAuthUserPermissionsData } from "../redux/slice/authSlice";


export const useHasPermission = () => {
  const userPermissions = useSelector(getAuthUserPermissionsData);

  const hasPermission = (requiredRole = [], requiredPermission = []) => {

    if (
      userPermissions?.role?.includes('LVL5_MasterAdmin') ||
      userPermissions?.role?.includes('LVL4_SuperAdmin')
    ) return true;
    let hasRole = false;
    let hasPermissions = false;
    if (requiredRole.length !== 0) {
      hasRole = requiredRole.some(item => userPermissions?.role?.includes(item));
    };
    if (requiredPermission.length != 0) {
      hasPermissions = requiredPermission.some(item => userPermissions?.permissions?.includes(item));
    };
    console.log(hasRole, hasPermissions);
    
    return hasRole && hasPermissions;
  };

  return { hasPermission };
};
