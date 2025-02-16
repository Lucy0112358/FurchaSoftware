import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getAllGroups = createAsyncThunk(
  'user/getAllGroups',
  async (data, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'User/user-groups/?adminId=8'
        };
       
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)
