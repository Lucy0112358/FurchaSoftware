/* eslint-disable no-unused-vars */
import axios from 'axios';
import { useEffect } from "react";

const Modules = () => {
    useEffect(() => {
        axios.get('https://localhost:7123/user/all-users')
            .then(response => {
                console.log(response.data, "Userr informationnnnnnnnnnnnnn");
            })
            .catch(error => {
                console.error('Ошибка:', error);
            });
    }, []);
    const users = [
        {
            id: "U1",
            firstName: "John",
            lastName: "Smith",
            role: "Superadmin",
            cardNo: ["22531845648997", "22531845649245"],
            pin: "1234",
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
            pin: "",
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
            pin: "",
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
            pin: "1234",
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
            pin: "",
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
            pin: "1234",
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
            pin: "1234",
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
            pin: "1234",
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
            pin: "1234",
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
            pin: "1234",
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
            pin: "1234",
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
            pin: "1234",
            site: "Tallinn Office, Tallinn Office 2, Pärnu Office",
            group: "Admin",
            state: "Active"
        },
    ];
    return (
        <div className="">
            <h1 className="text-2xl" style={{ color: '#AAAAAA' }}>Tallinn Office</h1>

            <div className="outlet__table__wrapper overflow-x-auto mt-2" style={{ /*backgroundColor: '#ffffff', */ borderRadius: '10px', height: '480px' }}>
                <table className="outlet__table min-w-full bg-white " style={{ color: '#AAAAAA' }}>
                    <thead>
                        <tr className="outlet__table__header">
                            <th className="text-left" style={{ paddingBottom: '10px', paddingTop: '10px' }}>User ID</th>
                            <th className="text-left" style={{ paddingBottom: '10px', paddingTop: '10px' }}>User Name</th>
                            <th className="text-left" style={{ paddingBottom: '10px', paddingTop: '10px' }}>Last Name</th>
                            <th className="text-left" style={{ paddingBottom: '10px', paddingTop: '10px' }}>Role</th>
                            <th className="text-left" style={{ paddingBottom: '10px', paddingTop: '10px' }}>Card No.</th>
                            <th className="text-left" style={{ paddingBottom: '10px', paddingTop: '10px' }}>Pin</th>
                            <th className="text-left" style={{ paddingBottom: '10px', paddingTop: '10px' }}>Site</th>
                            <th className="text-left" style={{ paddingBottom: '10px', paddingTop: '10px' }}>User Group</th>
                            <th className="text-left" style={{ paddingBottom: '10px', paddingTop: '10px' }}>State</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map((user, index) => (
                            <tr key={user.id} className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}>
                                <td className="" style={{ paddingBottom: '10px', paddingTop: '10px' }}>
                                    <input type="checkbox" className="mr-2" /> {user.id}
                                </td>
                                <td className="" style={{ paddingBottom: '10px', paddingTop: '10px' }}>{user.firstName}</td>
                                <td className="" style={{ paddingBottom: '10px', paddingTop: '10px' }}>{user.lastName}</td>
                                <td className="" style={{ paddingBottom: '10px', paddingTop: '10px' }}>{user.role}</td>
                                <td className="" style={{ paddingBottom: '10px', paddingTop: '10px' }}>
                                    {user.cardNo.map((card, idx) => (
                                        <div key={idx}>{card}</div>
                                    ))}
                                </td>
                                <td className="" style={{ paddingBottom: '10px', paddingTop: '10px' }}>{user.pin}</td>
                                <td className="" style={{ paddingBottom: '10px', paddingTop: '10px' }}>{user.site}</td>
                                <td className="" style={{ paddingBottom: '10px', paddingTop: '10px' }}>{user.group}</td>
                                <td className={` ${user.state === 'Suspended' ? 'text-red-500' : ''}`}>
                                    {user.state}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
        //   <div className="custom-scrollbar">
        //   <table>
        //     <thead>
        //       <tr>
        //         <th>User ID</th>
        //         <th>User Name</th>
        //         <th>Last Name</th>
        //         {/* ... другие заголовки */}
        //       </tr>
        //     </thead>
        //        <tbody>
        //            {users.map((user, index) => (
        //             <tr key={user.id} className={`${index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}`}>
        //               <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>
        //                 <input type="checkbox" className="mr-2" /> {user.id}
        //               </td>
        //               <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>{user.firstName}</td>
        //               <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>{user.lastName}</td>
        //               <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>{user.role}</td>
        //               <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>
        //                 {user.cardNo.map((card, idx) => (
        //                   <div key={idx}>{card}</div>
        //                 ))}
        //               </td>
        //               <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>{user.pin}</td>
        //               <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>{user.site}</td>
        //               <td className="" style={{paddingBottom: '10px', paddingTop: '10px'}}>{user.group}</td>
        //               <td className={` ${user.state === 'Suspended' ? 'text-red-500' : ''}`}>
        //                 {user.state}
        //               </td>
        //             </tr>
        //           ))}
        //         </tbody>
        //   </table>
        // </div>
    );
};

export default Modules;
