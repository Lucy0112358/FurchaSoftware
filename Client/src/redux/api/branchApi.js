import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";
import { getNewBrains } from "./moduleApi";

export const getLockerGroupsByBranchId = createAsyncThunk(
  'branch/getLockerGroupsByBranchId',
  async (id, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: `Locker/GetGroupsWithLockers?branchId=${id}`,
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const getBranches = createAsyncThunk(
  'branch/getBranches',
  async (params, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'Branch/branches',
        params: { ...params }
      };
      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const createBranch = createAsyncThunk(
  'branch/createBranch',
  async (data, thunkAPI) => {
    try {
      const config = {
        method: "post",
        data: data,
        url: 'Branch/create-branch',
      };
      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const deleteBranch = createAsyncThunk(
  'branch/deleteBranch',
  async (id, thunkAPI) => {
    try {
      const config = {
        method: "post",
        url: 'branch/delete-branch/' + id,
      };
      const response = await instance(config);
      await thunkAPI.dispatch(getBranches());
      return response?.data;

    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const getBranch = createAsyncThunk(
  'branch/getBranch',
  async (id, thunkAPI) => {
    try {
      const config = {
        method: "get",
        url: 'Branch/branches/' + id,
      };
      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)

export const updateBranch = createAsyncThunk(
  'branch/updateBranch',
  async (data, thunkAPI) => {
    try {
      const { id, ...payload } = data;
      const config = {
        method: "post",
        url: `branch/edit-branch?id=${id}`,
        data: payload,
      };

      const response = await instance(config);
      return response?.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error.response?.data?.error?.both || "Unknown error");
    }
  }
);

export const deleteBrainId = createAsyncThunk(
  'branch/deleteBrainId',
  async (uuid, thunkAPI) => {
    console.log(uuid);
    
    try {
      const config = {
        method: "post",
        url: 'branch/delete-brain-id/' + uuid,
      };
      const response = await instance(config);
      await thunkAPI.dispatch(getNewBrains());
      return response?.data;

    } catch (error) {
      return thunkAPI.rejectWithValue(error.response.data.error.both);
    }
  }
)