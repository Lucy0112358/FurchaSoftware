import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getAllUsers } from "../../redux/api/userApi";
import { getAllUsersData } from "../../redux/slice/userSlice";
import { getSelectGroupSelect } from "../../redux/slice/menuSlice";
import UserTable from "../../components/tables/UserTable";
import './admin.css';
import UserTableGroup from "../../components/tables/UserTableGroup";
import OfficeName from "../../components/headers/OfficeName";
import AdminTable from "../../components/tables/AdminTable";
import { getAllAdmins } from "../../redux/api/adminApi";

const Admins = () => {
  const dispatch = useDispatch();
  const userGroupSelected = useSelector(getSelectGroupSelect);

  useEffect(() => {
    dispatch(getAllAdmins());
  }, []);



  return (
    <div>
      <AdminTable />
    </div>
  );
};

export default Admins;
