import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getAllGroups = createAsyncThunk(
  'group/getAllGroups',
  async (data, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'user/user-groups'
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
          method: "post",
          url: 'user/delete-user-groups',
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
          url: 'user/suspend-user-groups',
          data: { ids: ids }
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
      // const response = await instance(config);
      
      // return response?.data?.data;
      return {
        "name": "Meeting Group",
        "branches" : [ 
          {
            "id": 1,
            "name": "Talin branch",
            "lockers": [10,11, 12]
          },
          {
            "id": 2,
            "name": "Gyumri branch",
            "lockers": [6, 8, 9]
          }
        ]
      }
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const updateUserGroupInfo = createAsyncThunk(
  'group/updateUserGroup',
  async (data, thunkAPI) => {
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
