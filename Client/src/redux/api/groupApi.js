import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getAllGroups = createAsyncThunk(
  'group/getAllGroups',
  async (params, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'user/user-groups',
        params: { ...params },
      };

      const response = await instance(config);
      
      return response?.data || [];
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
        method: "post",
        url: 'user/delete-user-groups',
        data: { ids: ids }
      };
      const response = await instance(config);
      await thunkAPI.dispatch(getAllGroups());

      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const changeGroupsState = createAsyncThunk(
  'group/changeGroupsState',
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        url: 'user/change-groups-state',
        data: data 
      };
      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const userGroupShow = createAsyncThunk(
  'group/show',
  async ({ id }, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'user/user-groups/' + id,
      };
      const response = await instance(config);

      return response?.data?.data;
      // return {
      //   "name": "Meeting Group",
      //   "branches": [
      //     {
      //       "id": 1,
      //       "name": "Talin branch",
      //       "lockers": [132, 133, 135]
      //     },
      //   ]
      // }
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const setUserGroup = createAsyncThunk(
  'menu/setUserGroup',
  async (data, thunkAPI) => {
      try {
        const config = {
          method: "post",
          url: 'User/add-user-group',
          data: data
        };
        
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const updateUserGroupInfo = createAsyncThunk(
  'group/updateUserGroup',
  async (data, thunkAPI) => {
    console.log(data, 8888888888);
    
    try {
      const config = {
        method: "post",
        url: 'user/edit-user-group',
        data: data
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)
