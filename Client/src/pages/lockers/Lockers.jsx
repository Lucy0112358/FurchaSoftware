import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getAllUsers } from "../../redux/api/userApi";
import { getAllUsersData } from "../../redux/slice/userSlice";
import { getLockerStatusSelect, getSelectGroupSelect } from "../../redux/slice/menuSlice";
import UserTable from "../../components/tables/UserTable";
import './locker.css';
import UserTableGroup from "../../components/tables/UserTableGroup";
import LockerTable from "../../components/tables/LockerTable";
import LockerTableGroup from "../../components/tables/LockerTableGroup";
import { getLockers } from "../../redux/api/lockerApi";

const Lockers = () => {
  const lockerStatusSelect = useSelector(getLockerStatusSelect)
  const dispatch = useDispatch();
  // const userGroupSelected = useSelector(getSelectGroupSelect);

  useEffect(() => {
    dispatch(getLockers());
  }, []);

  // const allUsers = useSelector(getAllUsersData);

  return (
    <div id="lockers">
      {!lockerStatusSelect ?
        <div >
        {
          <LockerTable />
        }
      </div>
      :
      <div >
        <LockerTableGroup />
      </div>
      }
    </div>
  );
};

export default Lockers;
