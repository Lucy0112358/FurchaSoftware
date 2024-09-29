import axios from "axios";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getAllUsers } from "../../redux/api/userApi";
import { getAllUsersData } from "../../redux/slice/userSlice";

const Users = () => {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(getAllUsers());
  }, []);

  const allUsers = useSelector(getAllUsersData);

  console.log(allUsers, "userBranchesuserBranchesuserBranches")

  const users = [
    {
      id: "U1",
      firstName: "John",
      lastName: "Smith",
      role: "Superadmin",
      cardNo: ["22531845648997", "22531845649245"],
      site: "Tallinn Office, Tallinn Office 2, Pärnu Office",
      group: "Admin",
      state: "Active"
    },
    {
      id: "U2",
      firstName: "Tom",
      lastName: "Smith",
      role: "User",
      cardNo: ["22531845648998"],
      site: "Tallinn Office",
      group: "Employee",
      state: "Suspended"
    },
    {
      id: "U3",
      firstName: "Tom",
      lastName: "Smith",
      role: "User",
      cardNo: ["22531845648998"],
      site: "Tallinn Office",
      group: "Employee",
      state: "Suspended"
    },
    {
      id: "U4",
      firstName: "John",
      lastName: "Smith",
      role: "Superadmin",
      cardNo: ["22531845648997", "22531845649245"],
      site: "Tallinn Office, Tallinn Office 2, Pärnu Office",
      group: "Admin",
      state: "Active"
    },
    {
      id: "U5",
      firstName: "Tom",
      lastName: "Smith",
      role: "User",
      cardNo: ["22531845648998"],
      site: "Tallinn Office",
      group: "Employee",
      state: "Suspended"
    },
    {
      id: "U6",
      firstName: "John",
      lastName: "Smith",
      role: "Superadmin",
      cardNo: ["22531845648997", "22531845649245"],
      site: "Tallinn Office, Tallinn Office 2, Pärnu Office",
      group: "Admin",
      state: "Active"
    },
    {
      id: "U7",
      firstName: "John",
      lastName: "Smith",
      role: "Superadmin",
      cardNo: ["22531845648997", "22531845649245"],
      site: "Tallinn Office, Tallinn Office 2, Pärnu Office",
      group: "Admin",
      state: "Active"
    },
    {
      id: "U8",
      firstName: "John",
      lastName: "Smith",
      role: "Superadmin",
      cardNo: ["22531845648997", "22531845649245"],
      site: "Tallinn Office, Tallinn Office 2, Pärnu Office",
      group: "Admin",
      state: "Active"
    },
    {
      id: "U9",
      firstName: "John",
      lastName: "Smith",
      role: "Superadmin",
      cardNo: ["22531845648997"],
      site: "Tallinn Office, Tallinn Office 2, Pärnu Office",
      group: "Admin",
      state: "Active"
    },
    {
      id: "U10",
      firstName: "John",
      lastName: "Smith",
      role: "Superadmin",
      cardNo: ["22531845648997", "22531845649245"],
      site: "Tallinn Office, Tallinn Office 2, Pärnu Office",
      group: "Admin",
      state: "Active"
    },
    {
      id: "U11",
      firstName: "John",
      lastName: "Smith",
      role: "Superadmin",
      cardNo: ["22531845648997", "22531845649245"],
      site: "Tallinn Office, Tallinn Office 2, Pärnu Office",
      group: "Admin",
      state: "Active"
    },
    {
      id: "U12",
      firstName: "John",
      lastName: "Smith",
      role: "Superadmin",
      cardNo: ["22531845648997", "22531845649245"],
      site: "Tallinn Office, Tallinn Office 2, Pärnu Office",
      group: "Admin",
      state: "Active"
    },
  ];
  
  return (
    <div className="">
        <h1 className="text-2xl" style={{color: '#AAAAAA'}}>Tallinn Office</h1>
        

      <div className="outlet__table__wrapper overflow-x-auto mt-2" style={{ /*backgroundColor: '#ffffff', */ borderRadius: '10px', height: '480px', width: '100%', maxWidth: '1130px', overflowX: 'auto' }}>
        <table className="outlet__table min-w-full bg-white " style={{color: '#AAAAAA', minWidth: '1110px'}}>
          <thead>
            <tr className="outlet__table__header">
              <th className="text-left" style={{paddingBottom: '10px', paddingTop: '10px'}}>User ID</th>
              <th className="text-left" style={{paddingBottom: '10px', paddingTop: '10px'}}>User Name</th>
              <th className="text-left" style={{paddingBottom: '10px', paddingTop: '10px'}}>Last Name</th>
              <th className="text-left" style={{paddingBottom: '10px', paddingTop: '10px'}}>Role</th>
              <th className="text-left" style={{paddingBottom: '10px', paddingTop: '10px'}}>Card No.</th>
              <th className="text-left" style={{paddingBottom: '10px', paddingTop: '10px'}}>Site</th>
              <th className="text-left" style={{paddingBottom: '10px', paddingTop: '10px'}}>User Group</th>
              <th className="text-left" style={{paddingBottom: '10px', paddingTop: '10px'}}>State</th>
            </tr>
          </thead>
          <tbody>
            {allUsers.map((user, index) => (
              <tr key={user.id} className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}>
                <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>
                  <input type="checkbox" className="mr-2" /> {user.id}
                </td>
                <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>{user.name}</td>
                <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>{user.surname}</td>
                <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>{user.role}</td>
                 <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>
                  {user.cards?.map((card, idx) => (
                    <div key={idx}>{card.cardNumber}</div>
                  ))}
                </td>
                <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>
                  {user.branches?.map((branch, idx) => (
                    <div key={idx}>{branch.name}</div>
                  ))}
                </td>
                <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>
                  {user.userGroups?.map((group, idx) => (
                    <div key={idx}>{group.groupName}</div>
                  ))}
                </td>
                <td className={` ${user.state === 'Suspended' ? 'text-red-500' : ''}`}>
                  {user.state}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default Users;
