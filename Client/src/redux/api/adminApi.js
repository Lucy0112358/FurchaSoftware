import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getAllAdmins = createAsyncThunk(
  'admin/getAllAdmins',
  async (params, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'Auth/get-admins',
        params: { ...params },
      };
      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const setAdminInfo = createAsyncThunk(
  'admin/setAdminInfo',
  async (data, thunkAPI) => {
    // console.log(data, "dataaaaaaa");

    try {
      const config = {
        method: "post",
        url: 'Auth/create-admin',
        data: data
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const deleteAdmins = createAsyncThunk(
  'admin/deleteAdmins',
  async (ids, thunkAPI) => {
    try {
      const config = {
        method: "post",
        url: 'auth/delete-admins',
        data: { ids: ids }
      };
      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const changeAdminState = createAsyncThunk(
  'admin/changeAdminState',
  async ({ ids, state }, thunkAPI) => {
    try {
      const config = {
        method: "patch",
        url: 'auth/set-admin-state',
        data: { ids: ids, state: state }
      };
      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const adminShow = createAsyncThunk(
  'admin/show',
  async ({ id }, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'auth/get-admin-by-id?id=' + id,
      };
      const response = await instance(config);
      return response?.data;
      // return {
      //   data: {
      //     "id": 1039,
      //     "name": "Lusine",
      //     "surname": "Baghdasaryan-Sahakyan",
      //     "role": "LVL5_MasterAdmin",
      //     "roleId": 1000000000,
      //     "permissions": [
      //       {
      //         "typeId": 1,
      //         "typeName": "Users",
      //         "permissions": [
      //           {
      //             "id": 1,
      //             "name": "ManageUsers",
      //             "description": "ManageUsers",
      //             "isOptional": false,
      //             "objectTypeId": 1
      //           },
      //           {
      //             "id": 2,
      //             "name": "CreateBranch",
      //             "description": "CreateBranch",
      //             "isOptional": false,
      //             "objectTypeId": 1
      //           },
      //           {
      //             "id": 5,
      //             "name": "CreateUser",
      //             "description": "CreateUser",
      //             "isOptional": false,
      //             "objectTypeId": 1
      //           },
      //           {
      //             "id": 8,
      //             "name": "ManageUserGroup",
      //             "description": "ManageUserGroup",
      //             "isOptional": false,
      //             "objectTypeId": 1
      //           },
      //           {
      //             "id": 9,
      //             "name": "ImportUsers",
      //             "description": "export users",
      //             "isOptional": false,
      //             "objectTypeId": 1
      //           }
      //         ]
      //       },
      //       {
      //         "typeId": 2,
      //         "typeName": "Lockers",
      //         "permissions": [
      //           {
      //             "id": 3,
      //             "name": "CreateLocker",
      //             "description": "CreateLocker",
      //             "isOptional": false,
      //             "objectTypeId": 2
      //           },
      //           {
      //             "id": 4,
      //             "name": "OpenLocker",
      //             "description": "OpenLocker",
      //             "isOptional": false,
      //             "objectTypeId": 2
      //           },
      //           {
      //             "id": 11,
      //             "name": "ManageLockerGroups",
      //             "description": "Create locker groups",
      //             "isOptional": false,
      //             "objectTypeId": 2
      //           },
      //           {
      //             "id": 12,
      //             "name": "AssignType",
      //             "description": "Locker type",
      //             "isOptional": false,
      //             "objectTypeId": 2
      //           },
      //           {
      //             "id": 13,
      //             "name": "TemporaryPersonal",
      //             "description": "TemporaryPersonal",
      //             "isOptional": false,
      //             "objectTypeId": 2
      //           }
      //         ]
      //       },
      //       {
      //         "typeId": 3,
      //         "typeName": "Admins",
      //         "permissions": [
      //           {
      //             "id": 7,
      //             "name": "ManageAdmins",
      //             "description": "ManageAdmins",
      //             "isOptional": false,
      //             "objectTypeId": 3
      //           }
      //         ]
      //       },
      //       {
      //         "typeId": 4,
      //         "typeName": "Modules",
      //         "permissions": [
      //           {
      //             "id": 10,
      //             "name": "AddModule",
      //             "description": "Add module chains",
      //             "isOptional": false,
      //             "objectTypeId": 4
      //           }
      //         ]
      //       },
      //       {
      //         "typeId": 5,
      //         "typeName": "LockerPermissions",
      //         "permissions": [
      //           {
      //             "id": 14,
      //             "name": "PersonalOpen",
      //             "description": "Permission for personal open",
      //             "isOptional": false,
      //             "objectTypeId": 5
      //           },
      //           {
      //             "id": 15,
      //             "name": "PersonalFree",
      //             "description": "Permission for personal free",
      //             "isOptional": false,
      //             "objectTypeId": 5
      //           },
      //           {
      //             "id": 16,
      //             "name": "CommonOpen",
      //             "description": "Permission for common open",
      //             "isOptional": false,
      //             "objectTypeId": 5
      //           },
      //           {
      //             "id": 17,
      //             "name": "CommonFree",
      //             "description": "Permission for common free",
      //             "isOptional": false,
      //             "objectTypeId": 5
      //           },
      //           {
      //             "id": 18,
      //             "name": "TemporaryOpen",
      //             "description": "Permission for temporary open",
      //             "isOptional": false,
      //             "objectTypeId": 5
      //           },
      //           {
      //             "id": 19,
      //             "name": "TemporaryFree",
      //             "description": "Permission for temporary free",
      //             "isOptional": false,
      //             "objectTypeId": 5
      //           },
      //           {
      //             "id": 20,
      //             "name": "ParcelOpen",
      //             "description": "Permission for parcel open",
      //             "isOptional": false,
      //             "objectTypeId": 5
      //           },
      //           {
      //             "id": 21,
      //             "name": "ParcelFree",
      //             "description": "Permission for parcel free",
      //             "isOptional": false,
      //             "objectTypeId": 5
      //           }
      //         ]
      //       }
      //     ],
      //     "branches": [
      //       {
      //         "branchId": 1,
      //         "branchName": "Tallin Branch",
      //         "groupIds": [1, 2]
      //       },
      //        {
      //         "branchId": 2,
      //         "branchName": "Gyumri Branch",
      //         "groupIds": [3, 4]
      //       }
      //     ],
      //     "isActive": false
      //   }

      // }
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)


export const updateAdminInfo = createAsyncThunk(
  'admin/updateAdminInfo',
  async (data, thunkAPI) => {
    console.log(data, "dataaaaaaa");
    try {
      const config = {
        method: "post",
        url: 'Auth/edit-admin',
        data: data
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)


