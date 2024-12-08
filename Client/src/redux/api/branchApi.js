import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getLockerGroupsByBranchId = createAsyncThunk(
  'branch/getLockerGroupsByBranchId',
  async ({id}, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: `Locker/index/${id}`,
        };
       
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)
