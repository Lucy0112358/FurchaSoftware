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
      // let data = response?.data;
      // data?.data[0]?.lockers[10].groupLockers.forEach((door) => {
      //   door.doorState = 'closed';
      // });

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
      return thunkAPI.rejectWithValue(error.response.data.errorMessage);
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

export const changeMode = createAsyncThunk(
  'locker/changeMode',
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        data: data,
        url: 'Locker/change-mode',
      };

      const response = await instance(config);

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

export const getParcelMessages = createAsyncThunk(
  'locker/getParcelMessages',
  async (_, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'Locker/getParcelMessages',
      };
      return {
        data: [
          { id: 1, title: "Parcel Delivered", content: "Your parcel has been delivered.", parcelType: 1 },
          { id: 2, title: "Parcel Pickup", content: "Please collect your parcel within 3 days.", parcelType: 1 },
          { id: 3, title: "Parcel Pickup2", content: "Your parcel is ready for pickup.", parcelType: 2 }
        ]
      }

      // const response = await instance(config);
      // return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const updateMessage = createAsyncThunk(
  'locker/updateMessage',
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        data: data,
        url: 'Locker/updateMessage',
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const createMessage = createAsyncThunk(
  'locker/createMessage',
  async (data, thunkAPI) => {
    console.log(data);
    alert(1)
    try {
      const config = {
        method: "post",
        data: data,
        url: 'Locker/createMessage',
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)