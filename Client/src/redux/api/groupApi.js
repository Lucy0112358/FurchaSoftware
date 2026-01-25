import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getAllGroups = createAsyncThunk(
  'group/getAllGroups',
  async (data, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'User/user-groups'
        };
       
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const deleteUserGroups = createAsyncThunk(
  'group/deleteUserGroups',
  async (ids, thunkAPI) => {
      try {
        const config = {
          method: "delete",
          url: 'User/deleteUserGroups',
          data: { ids: ids }
        };
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const suspendUserGroups = createAsyncThunk(
  'group/suspendUserGroups',
  async (ids, thunkAPI) => {
      try {
        const config = {
          method: "post",
          url: 'User/suspendUserGroups',
          data: { ids: ids }
        };
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

