import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const editLockerGroup = createAsyncThunk(
  'lockerGroup/editLockerGroup',
  async ({ id, data }, thunkAPI) => {
    try {
      const config = {
        method: "post",
        url: 'Locker/editLockerGroup/' + id,
        data: data
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.errorMessage);
    }
  }
)

export const getLockerGroup = createAsyncThunk(
  'lockerGroup/getLockerGroup',
  async (id, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'Locker/getLockerGroup/' + id,
      };
      const response = await instance(config);
      return response?.data;
      // return {data: {name:"locker group name"}};
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const deleteLockerGroup = createAsyncThunk(
  'lockerGroup/deleteLockerGroup',
  async (id, thunkAPI) => {
    try {
      const config = {
        method: "post",
        url: 'Locker/deleteLockerGroup/' + id,
      };
      const response = await instance(config);
      return response?.data;
      // return {data: {name:"locker group name"}};
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

