import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getModules = createAsyncThunk(
    'modules/getModules',
    async (_, thunkAPI) => {
        try {
          const config = {
            method: "get",
            url: 'Locker/GetModules',
          };
          
          const response = await instance(config);
          return response?.data;
        } catch (error) {
          return thunkAPI.rejectWithValue(error.response.data.error.both);
        }
      }
  )

  export const addModuleFunc = createAsyncThunk(
    'modules/addModuleFunc',
    async (data, thunkAPI) => {
        try {
          const config = {
            method: "post",
            url: 'Locker/CreateModule',
            data: data
          };
          
          const response = await instance(config);
          thunkAPI.dispatch(getModules());
          return response?.data;
        } catch (error) {
          return thunkAPI.rejectWithValue(error.response.data.error.both);
        }
      }
  )
