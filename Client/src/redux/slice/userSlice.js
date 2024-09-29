import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getAllUsers } from "../api/userApi";
// import { getUserSites } from "../api/userApi";
// import { APP_BASE_URL } from "../../config";

const initialState = {
  loading: false,
  allUsers: [],
};

export const userSlice = createSlice({
  name: "user",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
  },

  extraReducers: (builder) => {
    builder
      .addCase(getAllUsers.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllUsers.fulfilled, (state, action) => {
        state.loading = false;
        state.allUsers = action.payload.data;
      })
      .addCase(getAllUsers.rejected, (state, action) => {
        state.errorMessage = action.payload;
        state.loading = false;
      })
  },
});

export const {
  setLoading,
} = userSlice.actions;

export const getLoadingNow = (state) => state.user.loading;

export const getAllUsersData = (state) => state.user.allUsers;

export default userSlice.reducer;
