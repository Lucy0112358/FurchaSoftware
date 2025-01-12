import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getLockers = createAsyncThunk(
  'locker/getLockers',
  async (params, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'Locker',
          params: {...params}
        };
       
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)
