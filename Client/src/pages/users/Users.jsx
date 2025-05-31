import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getAllUsers } from "../../redux/api/userApi";
import { getAllUsersData } from "../../redux/slice/userSlice";
import { getSelectGroupSelect } from "../../redux/slice/menuSlice";
import UserTable from "../../components/tables/UserTable";
import './user.css';
import UserTableGroup from "../../components/tables/UserTableGroup";
import OfficeName from "../../components/headers/OfficeName";

const Users = () => {
  const dispatch = useDispatch();
  const userGroupSelected = useSelector(getSelectGroupSelect);

  useEffect(() => {
    dispatch(getAllUsers());
  }, []);

  return (
    !userGroupSelected ? <UserTable /> : <UserTableGroup />
  );
};

export default Users;
