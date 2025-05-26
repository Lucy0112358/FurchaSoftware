import { useEffect } from "react";
import { useDispatch } from "react-redux";
import './branch.css';
import BranchTable from "../../components/tables/BranchTable";
import { getAllBranches } from "../../redux/api/branchApi";

const Branch = () => {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(getAllBranches());
  }, []);

  return (
      <BranchTable />
  );
};

export default Branch;
