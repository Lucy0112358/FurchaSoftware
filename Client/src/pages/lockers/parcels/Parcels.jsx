import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getLockerStatusSelect, getParcelLockerStatusSelect } from "../../../redux/slice/menuSlice";
import './parcel.css';
import LockerTable from "../../../components/tables/LockerTable";
import LockerTableGroup from "../../../components/tables/LockerTableGroup";
import { getParcelLockers } from "../../../redux/api/lockerApi";
import * as signalR from "@microsoft/signalr";
import { updateLockerDoorState } from "../../../redux/slice/lockerSlice";
import ParcelLockerTable from "../../../components/tables/ParcelLockerTable";


const Parcels = () => {
  const lockerStatusSelect = useSelector(getParcelLockerStatusSelect)
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(getParcelLockers());
  }, []);

  return (
    <div id="lockers">
      <ParcelLockerTable />
      {/* {!lockerStatusSelect ?
        <ParcelLockerTable />
        :
        <LockerTableGroup />
      } */}
    </div>
  );
};

export default Parcels;
