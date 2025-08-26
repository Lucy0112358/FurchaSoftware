import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getAuthUser, getPermissions, getRoles, signin } from "../api/authApi";
// import { APP_BASE_URL } from "../../config";

const initialState = {
  loading: false,
  isAuth: false,
  authUser: {},
  roles: [],
  permissions: [],
  authUserPermissions: [],
};

export const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
    setPermissions: (state, action) => {
      state.permissions = action.payload;
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
      .addCase(getAuthUser.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAuthUser.fulfilled, (state, action) => {
        // state.authUser = action.payload
        state.isAuth = true
        state.authUserPermissions = {
          // 'role': state.authUser.role,
          // 'permissions': state.authUser.permissions
          // 'role': 'LVL3_Admin',
          // 'permissions': ['Open_Locker'],
          'role': 'LVL5_MasterAdmin',
          'permissions': ['Open_Locker'],
      }
        state.loading = false;
      })
      .addCase(getAuthUser.rejected, (state, action) => {
        state.loading = false;
      })
      .addCase(getRoles.fulfilled, (state, action) => {
        state.roles = action.payload
      })
      .addCase(getPermissions.fulfilled, (state, action) => {
        state.permissions = action.payload
      })
  },
});

export const {
  setLoading,
  setPermissions
} = authSlice.actions;

export const getLoading = (state) => state.auth.loading;
export const getIsAuth = (state) => state.auth.isAuth;
export const getRolesData = (state) => state.auth.roles;
export const getPermissionsData = (state) => state.auth.permissions;
export const getAuthUserData = (state) => state.auth.authUser;
export const getAuthUserPermissionsData = (state) => state.auth.authUserPermissions;

export default authSlice.reducer;
