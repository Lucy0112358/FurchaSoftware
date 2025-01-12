import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getAllUsers } from "../../redux/api/userApi";
import { getAllUsersData } from "../../redux/slice/userSlice";
import { getSelectGroupSelect } from "../../redux/slice/menuSlice";
import UserTable from "../../components/tables/UserTable";
import './branch.css';
import UserTableGroup from "../../components/tables/UserTableGroup";
import OfficeName from "../../components/headers/OfficeName";
import AdminTable from "../../components/tables/AdminTable";
import { getAllAdmins } from "../../redux/api/adminApi";
import BranchTable from "../../components/tables/BranchTable";
import { getAllBranches } from "../../redux/api/branchApi";
import { getBranches } from "../../redux/api/menuApi";

const Branch = () => {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(getAllBranches());
  }, []);

  return (
    <div className="">
      <BranchTable />
    </div>
  );
};

export default Branch;
