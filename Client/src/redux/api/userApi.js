import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getAllUsers = createAsyncThunk(
  'user/getAllUsers',
  async (_, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'User/company-users',
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const setUserInfo = createAsyncThunk(
  'menu/setUserInfo',
  async (data, thunkAPI) => {
    data.adminId = 8;
      try {
        const config = {
          method: "post",
          url: 'User/add-user',
          data: data
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const filterUserWithOutPaginte = createAsyncThunk(
  'user/filterUserWithOutPaginte',
  async (params, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'User/filtered-users/',
          params: { ...params },
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const logout = createAsyncThunk(
  'user/logout',
  async (_, thunkAPI) => {
      try {
        localStorage.removeItem("token");
        window.location.href = '/login';
        return;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const deleteUsers = createAsyncThunk(
  'user/deleteUsers',
  async (ids, thunkAPI) => {
      try {
        const config = {
          method: "delete",
          url: 'User/users',
          data: { ids: ids }
        };
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const changeUserState = createAsyncThunk(
  'user/changeUserState',
  async ({ids, state}, thunkAPI) => {
      try {
        const config = {
          method: "patch",
          url: 'User/change-state',
          data: { ids: ids, state: state }
        };
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const changeUserGroup = createAsyncThunk(
  'user/changeUserGroup',
  async ({ids, groupId}, thunkAPI) => {
    console.log(groupId, ids);
    
      try {
        const config = {
          method: "patch",
          url: 'User/change-group',
          data: { ids: ids, groupId: groupId }
        };
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)