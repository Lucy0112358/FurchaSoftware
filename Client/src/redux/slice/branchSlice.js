import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";

const initialState = {
  loading: false,
  allBranches: [],
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
      // .addCase(getAllAdmins.pending, (state) => {
      //   state.loading = true;
      // })
      // .addCase(getAllAdmins.fulfilled, (state, action) => {
      //   state.loading = false;
      //   adminSlice.caseReducers.setUser(state, action);
      // })
      // .addCase(getAllAdmins.rejected, (state, action) => {
      //   state.errorMessage = action.payload;
      //   state.loading = false;
      // })
  },
});

export const {
  setLoading,
} = branchSlice.actions;

export const getLoadingNow = (state) => state.branch.loading;
export const getAllBranchesData = (state) => state.user.allBranches;

export default branchSlice.reducer;
