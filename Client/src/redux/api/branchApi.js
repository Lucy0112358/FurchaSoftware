import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

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

export const getAllBranches = createAsyncThunk(
  'admin/getAllBranches',
  async (params, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'Branch/branches?adminId=8',
          params: {...params}
        };
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

export const createBranch = createAsyncThunk(
  'admin/createBranch',
  async (data, thunkAPI) => {
      try {
        const config = {
          method: "post",
          data: data,
          url: 'Branch/create-branch?adminId=8',
        };
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)
