import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getModules = createAsyncThunk(
    'modules/getModules',
    async (_, thunkAPI) => {
        try {
          const config = {
            method: "get",
            url: 'modules',
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

  //change name editModuleEdit
    export const editModuleLocker = createAsyncThunk(
    'modules/editModuleLocker',
    async ({data, id}, thunkAPI) => {
        try {
          const config = {
            method: "post",
            url: 'modules/' + id,
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

  export const getModuleLocker = createAsyncThunk(
    'modules/getModuleLocker',
    async ({id}, thunkAPI) => {
        try {
          const config = {
            method: "get",
            url: 'modules/' + id,
          };
          
          const response = await instance(config);
          return response?.data;
        } catch (error) {
          return thunkAPI.rejectWithValue(error.response.data.error.both);
        }
      }
  )

  export const deleteModule = createAsyncThunk(
    'modules/deleteModule',
    async (id, thunkAPI) => {
        try {
          const config = {
            method: "post",
            url: 'modules/delete/' + id,
          };
          const response = await instance(config);
          return response?.data;
        } catch (error) {
          return thunkAPI.rejectWithValue(error.response.data.error.both);
        }
      }
  )

  export const moduleShow = createAsyncThunk(
  'modules/show',
  async ({ id }, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'modules/getModule/' + id,
      };
      const response = await instance(config);
      return response?.data?.data;
      // return {
      //   "brainId": 'uransd-5sda5zcxz-sdacmdsd-sdcksdc',
      //   "branchId": 8,
      // }
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const updateModule = createAsyncThunk(
  'modules/updateModule',
  async ({id, data}, thunkAPI) => {
    try {
      const config = {
        method: "post",
        url: 'modules/editModule/' + id,
        data: data
      };
console.log(config, data);

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

  // export const getLockerGroupRange = createAsyncThunk(
  //   'modules/getLockerGroupRange',
  //   async ({brainId, groupId}, thunkAPI) => {
  //       try {
  //         console.log(brainId, groupId);
          
  //         const config = {
  //           method: "get",
  //           url: 'Locker/lockers-range/?groupId=' + groupId + '&brainId=' + brainId,
  //         };
          
  //         const response = await instance(config);
  //         // const response = {
  //         //   success: true,
  //         //   data: {
  //         //     min: 1,
  //         //     max: 10
  //         //   }
  //         // }
  //         console.log(response, 'response');
          
  //         return response?.data;
  //       } catch (error) {
  //         return thunkAPI.rejectWithValue(error.response.data.error.both);
  //       }
  //     }
  // )