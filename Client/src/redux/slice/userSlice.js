import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
// import { getUserSites } from "../api/userApi";
// import { APP_BASE_URL } from "../../config";

const initialState = {
  loading: false,
  userSite: {},
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
      // .addCase(getUserSites.fulfilled, (state, action) => {
      //   state.userSite = action.payload;
      // })
    //   .addCase(signin.pending, (state) => {
    //     state.loading = true;
    //     state.loginLoading = true;
    //   })
    //   .addCase(signin.fulfilled, (state) => {
    //     state.loading = false;
    //     state.loginLoading = false;
    //     state.isAuth = true;
    //   })
    //   .addCase(signin.rejected, (state, action) => {
    //     state.errorMessage = action.payload;
    //     state.loginLoading = false;
    //   })
  //   .addCase(getCurrentUser.fulfilled, (state, action) => {
  //     state.authUser = action.payload
  //     state.isAuth = true
  // })
  },
});

export const {
  setLoading,
} = userSlice.actions;

export const getLoadingNow = (state) => state.user.loading;

export default userSlice.reducer;
