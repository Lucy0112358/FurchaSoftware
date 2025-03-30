import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getBranches, getLockerGroupsData, getUserGroups, setUserGroup, userFilter } from "../api/menuApi";
import { userSlice } from "./userSlice";
// import { APP_BASE_URL } from "../../config";

const initialState = {
  loading: false,
  branches: {},
  userGroups: {},
  selectGroups: {},
  userGroupSelect: false,
  lockerStatusSelect: false,
  lockerGroups: {},
  menuFilter: {},
};

export const menuSlice = createSlice({
  name: "menu",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
    setUserBranches: (state, action) => {
      state.branches = action;
    },
    setLockerGroup: (state, action) => {
      state.lockerGroups = [{ id: 0, name: "All" }, ...action];
    },
    setUserGroups: (state, action) => {
      state.userGroups = [{ id: 0, name: "All" }, ...action];
    },
    setUserGroupSelect: (state) => {
      state.userGroupSelect = !state.userGroupSelect
    },
    setLockerStatusSelect: (state) => {
      state.lockerStatusSelect = !state.lockerStatusSelect
    },
    setMenuFilter: (state, action) => {
      state.menuFilter = action.payload;
    }
  },

  extraReducers: (builder) => {
    builder
      .addCase(getBranches.fulfilled, (state, action) => {
        menuSlice.caseReducers.setUserBranches(state, action.payload.data);
        // state.userBranch = action.payload;
      })
      .addCase(getLockerGroupsData.fulfilled, (state, action) => {
        menuSlice.caseReducers.setLockerGroup(state, action.payload.data);
        // state.userBranch = action.payload;
      })
      .addCase(getUserGroups.fulfilled, (state, action) => {
        menuSlice.caseReducers.setUserGroups(state, action.payload.data);
        // state.userBranch = action.payload;
      })
      .addCase(setUserGroup.fulfilled, (state, action) => {
        console.log(action.payload.data)
      })
      .addCase(setUserGroup.rejected, (state, action) => {
       
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
  setUserGroupSelect,
  setLockerStatusSelect,
  setMenuFilter
} = menuSlice.actions;

export const getLoadingNow = (state) => state.menu.loading;
export const getBranchesData = (state) => state.menu.branches;
export const getUserGroupsData = (state) => state.menu.userGroups;
export const getSelectGroups = (state) => state.menu.selectGroups;
export const getSelectGroupSelect = (state) => state.menu.userGroupSelect;
export const getLockerStatusSelect = (state) => state.menu.lockerStatusSelect;
export const getMenuFilter = (state) => state.menu.menuFilter;
export const getLockerGroups = (state) => state.menu.lockerGroups;


export default menuSlice.reducer;
