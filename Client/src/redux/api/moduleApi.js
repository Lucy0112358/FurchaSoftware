import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getModules = createAsyncThunk(
    'modules/getModules',
    async (_, thunkAPI) => {
        try {
          const config = {
            method: "get",
            url: 'Locker/GetModules',
          };
          
          const response = await instance(config);
          return response?.data;
        } catch (error) {
          return thunkAPI.rejectWithValue(error.response.data.error.both);
        }
      }
  )

  export const addModuleFunc = createAsyncThunk(
    'modules/addModuleFunc',
    async (data, thunkAPI) => {
        try {
          const config = {
            method: "post",
            url: 'Locker/CreateModule',
            data: data
          };
          
          const response = await instance(config);
          thunkAPI.dispatch(getModules());
          return response?.data;
        } catch (error) {
          return thunkAPI.rejectWithValue(error.response.data.error.both);
        }
      }
  )

  export const getNewBrains = createAsyncThunk(
    'modules/getNewBrains',
    async (_, thunkAPI) => {
        try {
          const config = {
            method: "get",
            url: 'Locker/get-new-brains',
          };
          
          const response = await instance(config);
          return response?.data;
        } catch (error) {
          return thunkAPI.rejectWithValue(error.response.data.error.both);
        }
      }
  )

  export const getModul = createAsyncThunk(
    'modules/getModul',
    async ({id}, thunkAPI) => {
        try {
          const config = {
            method: "get",
            url: 'Module/' + id,
          };
          
          const response = await instance(config);
          return response?.data;
        } catch (error) {
          return thunkAPI.rejectWithValue(error.response.data.error.both);
        }
      }
  )

  export const getLockerGroupRange = createAsyncThunk(
    'modules/getLockerGroupRange',
    async ({brainId, groupId}, thunkAPI) => {
        try {
          console.log(brainId, groupId);
          
          const config = {
            method: "get",
            url: 'Locker/lockers-range/?groupId=' + groupId + '&brainId=' + brainId,
          };
          
          const response = await instance(config);
          // const response = {
          //   success: true,
          //   data: {
          //     min: 1,
          //     max: 10
          //   }
          // }
          console.log(response, 'response');
          
          return response?.data;
        } catch (error) {
          return thunkAPI.rejectWithValue(error.response.data.error.both);
        }
      }
  )