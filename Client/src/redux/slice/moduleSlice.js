import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getBranches, getLockerGroupsData} from "../api/menuApi";
import { userSlice } from "./userSlice";
import { getModules } from "../api/moduleApi";
// import { APP_BASE_URL } from "../../config";

const initialState = {
  modalBranches: {},
  allModules: {},
  modalGroups: {},

};

export const moduleSlice = createSlice({
  name: "modules",
  initialState,
  reducers: {
    setBranches: (state, action) => {
      state.modalBranches = [{ id: 0, name: "All" }, ...action];
    },
    setLockerGroup: (state, action) => {
      state.modalGroups = [{ id: 0, name: "All" }, ...action];
    },
    filterGroupByBranch: (state, action) => {
      state.modalGroups = state.modalGroups.filter((group) => {
        return group.branchId === action.payload;
      });
    },
  },

  extraReducers: (builder) => {
    builder
      .addCase(getBranches.fulfilled, (state, action) => {
        moduleSlice.caseReducers.setBranches(state, action.payload.data);
      })
      .addCase(getLockerGroupsData.fulfilled, (state, action) => {
        moduleSlice.caseReducers.setLockerGroup(state, action.payload.data);
      })
      .addCase(getModules.fulfilled, (state, action) => {
        state.allModules =  action.payload.data;
      })

      
  },
});

export const {
  filterGroupByBranch
} = moduleSlice.actions;

export const getModuleModalGroupes = (state) => state.modules.modalGroups;
export const getModuleModalBranches = (state) => state.modules.modalBranches;
export const getModulesData = (state) => state.modules.allModules;


export default moduleSlice.reducer;
