import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getUserGroups = createAsyncThunk(
  'menu/getUserGroups',
  async (_, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'User/user-groups',
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)


export const setLockerGroup = createAsyncThunk(
  'menu/setLockerGroup',
  async (data, thunkAPI) => {
      try {
        const config = {
          method: "post",
          url: 'Locker',
          data: data
        };
        
        const response = await instance(config);
        thunkAPI.dispatch(getLockerGroupsData());
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const getLockerGroupsData = createAsyncThunk(
  'menu/getLockerGroupsData',
  async (_, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'Locker/admin-lockerGroups',
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)