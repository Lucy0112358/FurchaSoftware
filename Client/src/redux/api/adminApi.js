import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

export const getAllAdmins = createAsyncThunk(
  'admin/getAllAdmins',
  async (_, thunkAPI) => {
      try {
        const config = {
          method: "get",
          url: 'User/company-users?adminId=8',
        };
        const response = await instance(config);
        return response?.data;
      } catch (error) {
        return thunkAPI.rejectWithValue(error.response.data.error.both);
      }
    }
)

// export const setUserInfo = createAsyncThunk(
//   'menu/setUserInfo',
//   async (data, thunkAPI) => {
//     console.log(data, 7444444444)
//     data.adminId = 8;
//       try {
//         const config = {
//           method: "post",
//           url: 'User/add-user',
//           data: data
//         };
        
//         const response = await instance(config);
//         return response?.data;
//       } catch (error) {
//         return thunkAPI.rejectWithValue(error.response.data.error.both);
//       }
//     }
// )
