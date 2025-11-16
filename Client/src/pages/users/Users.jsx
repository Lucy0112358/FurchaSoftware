import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getUsers } from "../../redux/api/userApi";
import { getSelectGroupSelect } from "../../redux/slice/menuSlice";
import UserTable from "../../components/tables/UserTable";
import './user.css';
import UserTableGroup from "../../components/tables/UserTableGroup";

const Users = () => {
  const dispatch = useDispatch();
  const userGroupSelected = useSelector(getSelectGroupSelect);

  useEffect(() => {
    dispatch(getUsers());
  }, []);

  return (
    !userGroupSelected ? <UserTable /> : <UserTableGroup />
  );
};

export default Users;
