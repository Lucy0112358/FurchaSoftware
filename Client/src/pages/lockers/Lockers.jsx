import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getAllUsers } from "../../redux/api/userApi";
import { getAllUsersData } from "../../redux/slice/userSlice";
import { getSelectGroupSelect } from "../../redux/slice/menuSlice";
import UserTable from "../../components/tables/UserTable";
import './locker.css';
import UserTableGroup from "../../components/tables/UserTableGroup";
import LockerTable from "../../components/tables/LockerTable";

const Lockers = () => {
  // const dispatch = useDispatch();
  // const userGroupSelected = useSelector(getSelectGroupSelect);

  // useEffect(() => {
  //   dispatch(getAllUsers());
  // }, []);

  // const allUsers = useSelector(getAllUsersData);

  return (
    <div id="lockers">
      <div className="outlet__table__wrapper overflow-x-auto mt-2">
        {
          <LockerTable />
        }
      </div>
    </div>
  );
};

export default Lockers;
