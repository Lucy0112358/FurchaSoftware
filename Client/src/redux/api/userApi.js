import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getUsers = createAsyncThunk(
  'user/getUsers',
  async (params, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'user/users',
        params: { ...params },
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

export const updateUserInfo = createAsyncThunk(
  'user/updateUser',
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        url: 'User/edit-user',
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
        url: 'User/filtered-users',
        params: { ...params },
      };

      const response = await instance(config);
      return response?.data?.data || [];
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
        method: "post",
        url: 'User/delete-users',
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
  async ({ ids, state }, thunkAPI) => {
    try {
      const config = {
        method: "post",
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
  async ({ ids, groupId }, thunkAPI) => {
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

export const userShow = createAsyncThunk(
  'user/show',
  async ({ id }, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'user/' + id,
      };
      const response = await instance(config);
      return response?.data?.data;
      // return {
      //   "isPinRequired": true,
      //   "name": "Name",
      //   "email": "email@mail.ru",
      //   "phone": "Phone",
      //   "surname": "LastName",
      //   "activeFrom": "2025-08-14",
      //   "activeTo": "2025-08-28",
      //   "userGroups": [
      //     1,
      //     2
      //   ],
      //   "cards": [
      //     "xzcxzvcxvcxv",
      //     "cxvxc"
      //   ],
      //   "branches" : [ 
      //     {
      //       "id": 1,
      //       "name": "Talin branch",
      //       "lockers": [10,11, 12]
      //     },
      //     {
      //       "id": 2,
      //       "name": "Gyumri branch",
      //       "lockers": [6, 8, 9]
      //     }
      //   ]
      // }
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)