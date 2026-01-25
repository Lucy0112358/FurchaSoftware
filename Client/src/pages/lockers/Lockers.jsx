import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getLockerStatusSelect } from "../../redux/slice/menuSlice";
import './locker.css';
import LockerTable from "../../components/tables/LockerTable";
import LockerTableGroup from "../../components/tables/LockerTableGroup";
import { getLockers } from "../../redux/api/lockerApi";

const Lockers = () => {
  const lockerStatusSelect = useSelector(getLockerStatusSelect)
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(getLockers());
  }, []);

  return (
    <div id="lockers">
      {!lockerStatusSelect ?
        <LockerTable />
        :
        <LockerTableGroup />
      }
    </div>
  );
};

export default Lockers;
