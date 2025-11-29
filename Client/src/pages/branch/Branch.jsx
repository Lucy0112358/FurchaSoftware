import { useEffect } from "react";
import { useDispatch } from "react-redux";
import './branch.css';
import BranchTable from "../../components/tables/BranchTable";
import { getBranches } from "../../redux/api/branchApi";

const Branch = () => {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(getBranches());
  }, []);

  return (
      <BranchTable />
  );
};

export default Branch;
