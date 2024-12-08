import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";

const initialState = {
  loading: false,
};

export const branchSlice = createSlice({
  name: "branch",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
  },

  extraReducers: (builder) => {
    builder
  },
});

export const {
  setLoading,
} = branchSlice.actions;

export const getLoadingNow = (state) => state.branch.loading;

export default branchSlice.reducer;
