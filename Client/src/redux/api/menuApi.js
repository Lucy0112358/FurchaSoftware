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
          url: 'User/users-groups/?adminId=8',
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)



