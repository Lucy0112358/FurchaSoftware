import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getAllBranches, getBranch } from "../api/branchApi";

const initialState = {
  loading: false,
  allBranches: [],
  branchData: {},
};

export const branchSlice = createSlice({
  name: "branch",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
    setBranch: (state, action) => {
      state.allBranches = action.payload.data;
    },
  },

  extraReducers: (builder) => {
    builder
      .addCase(getAllBranches.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllBranches.fulfilled, (state, action) => {
        state.loading = false;
        branchSlice.caseReducers.setBranch(state, action);
      })
      .addCase(getAllBranches.rejected, (state, action) => {
        state.errorMessage = action.payload;
        state.loading = false;
      })
      .addCase(getBranch.fulfilled, (state, action) => {
        state.branch = action.payload;
      })
  },
});

export const {
  setLoading,
} = branchSlice.actions;

export const getLoadingNow = (state) => state.branch.loading;
export const getAllBranchesData = (state) => state.branch.allBranches;
export const getBranchData = (state) => state.branch.branchData;

export default branchSlice.reducer;
