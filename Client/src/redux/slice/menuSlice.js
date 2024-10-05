import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getUserBranches, getUserGroups } from "../api/menuApi";
// import { APP_BASE_URL } from "../../config";

const initialState = {
  loading: false,
  userBranches: {},
  userGroups: {},
  selectGroups: {}
};

export const menuSlice = createSlice({
  name: "menu",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
    setUserBranches: (state, action) => {
      state.userBranches = [{ id: 0, name: "All" }, ...action];
    },
    setUserGroups: (state, action) => {
      state.userGroups = [{ id: 0, name: "All" }, ...action];
    },
  },

  extraReducers: (builder) => {
    builder
      .addCase(getUserBranches.fulfilled, (state, action) => {
        menuSlice.caseReducers.setUserBranches(state, action.payload.data);
        // state.userBranch = action.payload;
      })
      .addCase(getUserGroups.fulfilled, (state, action) => {
        menuSlice.caseReducers.setUserGroups(state, action.payload.data);
        // state.userBranch = action.payload;
      })
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
} = menuSlice.actions;

export const getLoadingNow = (state) => state.menu.loading;
export const getUserBranchesData = (state) => state.menu.userBranches;
export const getUserGroupsData = (state) => state.menu.userGroups;
export const getSelectGroups = (state) => state.menu.selectGroups;



export default menuSlice.reducer;
