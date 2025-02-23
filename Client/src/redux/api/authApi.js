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
      localStorage.setItem("token", response.data.access_token);
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
