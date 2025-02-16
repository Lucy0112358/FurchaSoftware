import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getAuthUser, signin } from "../api/authApi";
// import { APP_BASE_URL } from "../../config";

const initialState = {
  loading: false,
  isAuth: false,
  authUser: {},
};

export const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
  },

  extraReducers: (builder) => {
    builder
      .addCase(signin.pending, (state) => {
        state.loading = true;
      })
      .addCase(signin.fulfilled, (state) => {
        state.loading = false;
        state.isAuth = true;
      })
      .addCase(signin.rejected, (state, action) => {
        state.loading = false;
      })
      .addCase(getAuthUser.fulfilled, (state, action) => {
        state.authUser = action.payload
        state.isAuth = true
      })
  },
});

export const {
  setLoading,
} = authSlice.actions;

export const getLoading = (state) => state.auth.loading;
export const getIsAuth = (state) => state.auth.isAuth;

export default authSlice.reducer;
