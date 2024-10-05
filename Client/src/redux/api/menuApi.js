import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getUserBranches = createAsyncThunk(
  'menu/getUserBranches',
  async (_, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'Branch/company-branches/?adminId=8',
        };
        
        const response = await instance(config);
        console.log(response.data)
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
    console.log(params, 7444444444)
      try {
        const config = {
          method: "get",
          url: 'User/filtered-users/?adminId=8',
          params: { ...params },
        };
        
        const response = await instance(config);
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
