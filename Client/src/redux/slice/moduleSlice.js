import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getBranches, getLockerGroupsData} from "../api/menuApi";
import { userSlice } from "./userSlice";
import { getLockerGroupRange, getModules, getNewBrains} from "../api/moduleApi";

const initialState = {
  modalBranches: {},
  allModules: {},
  modalGroups: {},
  modalGroupsCopy: {},
  newBrains: {},
  lockerGroupMinMax: {
    min: 1,
    max: 256,
  },
};

export const moduleSlice = createSlice({
  name: "modules",
  initialState,
  reducers: {
    setBranches: (state, action) => {
      state.modalBranches = action;
    },
    setLockerGroup: (state, action) => {
      state.modalGroups = action;
      state.modalGroupsCopy = action;
    },
    filterGroupByBranch: (state, action) => {
      state.modalGroups = action.payload
        ? state.modalGroupsCopy.filter(group => group.branchId === action.payload)
        : [...state.modalGroupsCopy];
    }
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
      .addCase(getNewBrains.fulfilled, (state, action) => {
        state.newBrains = action.payload.data;
      })
      .addCase(getLockerGroupRange.fulfilled, (state, action) => {
        state.lockerGroupMinMax = {
          min: action.payload.data?.lastLocker?? 1,
          max: 256,
        };
      });
  },
});

export const {
  filterGroupByBranch
} = moduleSlice.actions;

export const getModuleModalGroupes = (state) => state.modules.modalGroups;
export const getModuleModalBranches = (state) => state.modules.modalBranches;
export const getModulesData = (state) => state.modules.allModules;
export const getNewBrainsData = (state) => state.modules.newBrains;
export const getLockerGroupMinMax = (state) => state.modules.lockerGroupMinMax;


export default moduleSlice.reducer;
