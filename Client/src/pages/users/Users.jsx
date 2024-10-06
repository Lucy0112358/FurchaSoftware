import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getAllUsers } from "../../redux/api/userApi";
import { getAllUsersData } from "../../redux/slice/userSlice";
import { getSelectGroupSelect } from "../../redux/slice/menuSlice";
import UserTable from "../../components/tables/UserTable";
import './user.css';
import UserTableGroup from "../../components/tables/UserTableGroup";

const Users = () => {
  const dispatch = useDispatch();
  const userGroupSelected = useSelector(getSelectGroupSelect);

  useEffect(() => {
    dispatch(getAllUsers());
  }, []);

  const allUsers = useSelector(getAllUsersData);

  console.log(allUsers, "userBranchesuserBranchesuserBranches")
  
  return (
    <div className="">
        <h1 className="text-2xl" style={{color: '#AAAAAA'}}>Tallinn Office</h1>
        

      <div className="outlet__table__wrapper overflow-x-auto mt-2" style={{ /*backgroundColor: '#ffffff', */ borderRadius: '10px', height: '480px', width: '100%', maxWidth: '1130px', overflowX: 'auto' }}>
        {
          !userGroupSelected ? <UserTable /> : <UserTableGroup />
        }
      </div>
    </div>
  );
};

export default Users;
