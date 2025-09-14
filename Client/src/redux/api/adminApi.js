import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getAllAdmins = createAsyncThunk(
  'admin/getAllAdmins',
  async (_, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'Auth/get-admins',
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
    console.log(data, "dataaaaaaa");
    
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
  async ({ids, state}, thunkAPI) => {
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
      //   "isPinRequired": true,
      //   "name": "Name",
      //   "email": "email@mail.ru",
      //   "phone": "Phone",
      //   "surname": "LastName",
      //   "activeFrom": "2025-08-14",
      //   "activeTo": "2025-08-28",
      //   "userGroups": [
      //     1,
      //     2
      //   ],
      //   "cards": [
      //     "xzcxzvcxvcxv",
      //     "cxvxc"
      //   ],
      //   "pin": true,
      //   "adminId": 8
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
        alert('ok');
      //   const config = {
      //     method: "post",
      //     url: 'Auth/create-admin',
      //     data: data
      //   };
        
      //   const response = await instance(config);
      //   return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)


