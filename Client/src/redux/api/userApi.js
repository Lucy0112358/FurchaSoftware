import { createAsyncThunk } from "@reduxjs/toolkit";
import instance from "../../config/axios/axiosConfig";

// export const getUserSites = createAsyncThunk(
//   'user/getUserSites',
//   async (_, thunkAPI) => {
//       try {
//         const config = {
//           method: "get",
//           url: 'auth/me',
//         };
        
//         const response = await instance(config);
//         return response?.data;
//       } catch (error) {
//         return thunkAPI.rejectWithValue(error.response.data.error.both);
//       }
//     }
// )
