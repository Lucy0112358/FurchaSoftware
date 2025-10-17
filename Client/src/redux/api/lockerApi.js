import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getLockers = createAsyncThunk(
  'locker/getLockers',
  async (params, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'Locker',
        params: { ...params }
      };

      const response = await instance(config);
      let data = response?.data;
      data.data[0].lockers[10].groupLockers.forEach((door) => {
        door.doorState = 'closed';
      });

      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const getParcelLockers = createAsyncThunk(
  'locker/getParcelLockers',
  async (params, thunkAPI) => {
    const query = { ...params, lockerType: 2 };

    try {
      const config = {
        method: "get",
        url: 'Locker',
        params: query
      };

      const response = await instance(config);

      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const editLockersType = createAsyncThunk(
  'locker/editLockersType',
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        data: data,
        url: 'Locker/edit-lockers',
      };

      const response = await instance(config);
      console.log("response", response);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const openLockers = createAsyncThunk(
  'locker/openLockers',
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        data: data,
        url: 'Locker/open-lockers',
      };

      const response = await instance(config);
      console.log("response", response);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const suspendLockers = createAsyncThunk(
  'locker/suspendLockers',
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        data: data,
        url: 'Locker/suspend-lockers',
      };

      const response = await instance(config);
      console.log("response", response);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const setUser = createAsyncThunk(
  'locker/setUser',
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        data: data,
        url: 'Locker/set-user',
      };

      const response = await instance(config);
      console.log("response", response);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)


export const getLockerTypes = createAsyncThunk(
  'locker/getLockerTypes',
  async (_, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'auth/getLockerTypes',
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)