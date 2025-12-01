import React, {  useState } from "react";
import { useSelector } from "react-redux";
import { getAdminData } from "../../../redux/slice/adminSlice";
import AddAdminModal from "../../modals/addAdmin/AddAdminModal";

function Edit() {
  const [isOpen, setIsOpen] = useState(false);
  const admin = useSelector(getAdminData);

    const handleClose = () => {
        setIsOpen(false);
        if (onClose) onClose();
    };

  return (
    <>
      <button
        className="bg-gray-600 text-white rounded"
        onClick={() => setIsOpen(true)}
      >
        Edit
      </button>

      {isOpen && (
        <AddAdminModal
          mode="edit"
          initialData={admin}
          onClose = { handleClose }
        />
      )}
    </>
  );
}

export default Edit;
