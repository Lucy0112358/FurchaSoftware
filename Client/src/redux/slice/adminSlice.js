import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getAllUsers, setUserInfo } from "../api/userApi";
import { filterUserByName, userFilter } from "../api/menuApi";
import { getAllAdmins } from "../api/adminApi";
// import { getUserSites } from "../api/userApi";
// import { APP_BASE_URL } from "../../config";

const initialState = {
  loading: false,
  allAdmins: [],
};

export const adminSlice = createSlice({
  name: "admin",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
    // setAddUserInfo: (state, action) => {
    //   state.userInfo = {
    //     ...state.userInfo,
    //     ...action.payload,
    //   };
    // },
    // setAdmin: (state, action) => {
    //   state.allAdmins = action.payload.data;
    // },
  },

  extraReducers: (builder) => {
    builder
      .addCase(getAllAdmins.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllAdmins.fulfilled, (state, action) => {
        state.loading = false;
        adminSlice.caseReducers.setUser(state, action);
      })
      .addCase(getAllAdmins.rejected, (state, action) => {
        state.errorMessage = action.payload;
        state.loading = false;
      })
      // .addCase(setUserInfo.pending, (state) => {
      //   state.loading = true;
      // })
      // .addCase(setUserInfo.fulfilled, (state, action) => {
      //   state.loading = false;
      //   state.allUsers.push(action.payload.data);
      //   console.log(action.payload.data);
      //   // adminSlice.caseReducers.setUser(state, action);
      // })
      // .addCase(setUserInfo.rejected, (state, action) => {
      //   state.errorMessage = action.payload;
      //   state.loading = false;
      // })
      // .addCase(userFilter.fulfilled, (state, action) => {
      //   adminSlice.caseReducers.setUser(state, action);
      // })
      // .addCase(filterUserByName.fulfilled, (state, action) => {
      //   adminSlice.caseReducers.setUser(state, action);
      // })
  },
});

export const {
  setLoading,
  // setAddUserInfo,
} = adminSlice.actions;

export const getLoadingNow = (state) => state.user.loading;

export const getAllAdminsData = (state) => state.user.allAdmins;

// export const getAddUserInfo = (state) => state.user.userInfo;

export default adminSlice.reducer;
