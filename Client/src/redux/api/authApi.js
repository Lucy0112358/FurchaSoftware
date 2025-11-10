import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const signin = createAsyncThunk(
  "auth/signin",
  async (data, thunkAPI) => {
    try {
      const signInData = {
        email: data.email,
        password: data.password,
      };

      const config = {
        method: "post",
        url: "auth/login",
        data: signInData,
      };

      const response = await instance(config);
      localStorage.setItem("token", response.data.token);
      await thunkAPI.dispatch(getAuthUser());
      return true;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
);

export const editAuthUser = createAsyncThunk(
  "auth/edit",
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        url: "auth/edid-profile",
        data: data,
      };

      const response = await instance(config);
     
      await thunkAPI.dispatch(getAuthUser());
      return true;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
);

export const changePassword = createAsyncThunk(
  "auth/changePassword",
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        url: "auth/change-password",
        data: data,
      };

      const response = await instance(config);
     
      await thunkAPI.dispatch(getAuthUser());
      return true;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
);

export const getAuthUser = createAsyncThunk(
  'auth/getAuthUser',
  async (_, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'auth/getAuthUser',
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const getRoles = createAsyncThunk(
  'auth/getRoles',
  async (_, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'auth/roles',
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const getPermissions = createAsyncThunk(
  'auth/getPermissions',
  async (roleId, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'auth/role-permissions',
        params: { roleId: roleId },
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)
