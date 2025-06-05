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
          method: "delete",
          url: 'Admin/admins',
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
          url: 'Admin/change-state',
          data: { ids: ids, state: state }
        };
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)