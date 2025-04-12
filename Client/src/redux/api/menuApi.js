import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getBranches = createAsyncThunk(
  'menu/getBranches',
  async (_, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'Branch/company-branches/?adminId=8',
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const getUserGroups = createAsyncThunk(
  'menu/getUserGroups',
  async (_, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'User/user-groups/?adminId=8',
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const userFilter = createAsyncThunk(
  'menu/userFilter',
  async (params, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'User/filtered-users/',
          params: { ...params },
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

// TODO: Change location to UserGroupApi
export const setUserGroup = createAsyncThunk(
  'menu/setUserGroup',
  async (data, thunkAPI) => {
    data.adminId=8
      try {
        const config = {
          method: "post",
          url: 'User/add-user-group',
          data: data
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
    data.adminId=8
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

//Temprorary for testing
export const filterUserByName = createAsyncThunk(
  'menu/filterUserByName',
  async (params, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'User/search-user/?adminId=8',
          params: { ...params },
        };
        
        const response = await instance(config);
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
          url: 'Locker/admin-lockerGroups/?adminId=8',
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)


