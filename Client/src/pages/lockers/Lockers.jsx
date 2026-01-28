import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getLockerStatusSelect } from "../../redux/slice/menuSlice";
import './locker.css';
import LockerTable from "../../components/tables/LockerTable";
import LockerTableGroup from "../../components/tables/LockerTableGroup";
import { getLockers } from "../../redux/api/lockerApi";
import * as signalR from "@microsoft/signalr";
import { updateLockerDoorState } from "../../redux/slice/lockerSlice";


const Lockers = () => {
  const lockerStatusSelect = useSelector(getLockerStatusSelect)
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(getLockers());
  }, []);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_SOCKET_URL + "/hubs/doorStatus")
      .withAutomaticReconnect()
      .build();

    connection.on("ReceiveDoorStatus", (doorId, status) => {
      console.log(`Door ${doorId} status changed: ${status}`);

      dispatch(updateLockerDoorState({ lockerId: doorId, doorState: status }));
    });

    connection
      .start()
      .then(() => console.log("SignalR connected"))
      .catch((err) => console.error("SignalR connection error:", err));

    return () => {
      connection.stop();
    };
  }, [dispatch]);

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
