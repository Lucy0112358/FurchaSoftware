import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getAdminData } from "../../../redux/slice/adminSlice";
import AddAdminModal from "../../modals/addAdmin/AddAdminModal";
import { adminShow } from "../../../redux/api/adminApi";

function Edit({ id }) {
  const [isOpen, setIsOpen] = useState(false);
  const dispatch = useDispatch();
  const data = useSelector(getAdminData);

  useEffect(() => {
    if (id) {
      dispatch(adminShow({ id }));
    }
  }, [id]);

  console.log(data, id, 'datasssssssssssss');

  return (
    <>
      <button
        className="bg-gray-600 text-white rounded px-4 py-2"
        onClick={() => setIsOpen(true)}
      >
        Edit
      </button>

      {isOpen && (
        <AddAdminModal
          mode="edit"
          initialData={data}
          onClose={() => setIsOpen(false)}
        />
      )}
    </>
  );
}

export default Edit;
