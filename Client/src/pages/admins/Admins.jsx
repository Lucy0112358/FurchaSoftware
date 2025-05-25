import { useEffect } from "react";
import { useDispatch } from "react-redux";
import './admin.css';
import AdminTable from "../../components/tables/AdminTable";
import { getAllAdmins } from "../../redux/api/adminApi";

const Admins = () => {
  const dispatch = useDispatch();
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
