import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getAllUsers, setUserInfo } from "../api/userApi";
import { filterUserByName, userFilter } from "../api/menuApi";
// import { getUserSites } from "../api/userApi";
// import { APP_BASE_URL } from "../../config";

const initialState = {
  loading: false,
  allUsers: [],
  userInfo: {},
};

export const userSlice = createSlice({
  name: "user",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
    setAddUserInfo: (state, action) => {
      state.userInfo = {
        ...state.userInfo,
        ...action.payload,
      };
    },
    setUser: (state, action) => {
      state.allUsers = action.payload.data;
    },
  },

  extraReducers: (builder) => {
    builder
      .addCase(getAllUsers.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllUsers.fulfilled, (state, action) => {
        state.loading = false;
        userSlice.caseReducers.setUser(state, action);
      })
      .addCase(getAllUsers.rejected, (state, action) => {
        state.errorMessage = action.payload;
        state.loading = false;
      })
      .addCase(setUserInfo.pending, (state) => {
        state.loading = true;
      })
      .addCase(setUserInfo.fulfilled, (state, action) => {
        state.loading = false;
        state.allUsers.push(action.payload.data);
        console.log(action.payload.data);
        // userSlice.caseReducers.setUser(state, action);
      })
      .addCase(setUserInfo.rejected, (state, action) => {
        state.errorMessage = action.payload;
        state.loading = false;
      })
      .addCase(userFilter.fulfilled, (state, action) => {
        userSlice.caseReducers.setUser(state, action);
      })
      .addCase(filterUserByName.fulfilled, (state, action) => {
        userSlice.caseReducers.setUser(state, action);
      })
      
  },
});

export const {
  setLoading,
  setAddUserInfo,
} = userSlice.actions;

export const getLoadingNow = (state) => state.user.loading;

export const getAllUsersData = (state) => state.user.allUsers;

export const getAddUserInfo = (state) => state.user.userInfo;

export default userSlice.reducer;
