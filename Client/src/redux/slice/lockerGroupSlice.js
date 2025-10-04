import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getLockerGroup } from "../api/lockerGroupApi";
// import { getAllGroups } from "../api/groupApi";

const initialState = {
  loading: false,
  lockerGroupData: {},
};

export const lockerGroupSlice = createSlice({
  name: "lockerGroup",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
  },

  extraReducers: (builder) => {
    builder
    .addCase(getLockerGroup.pending, (state) => {
      state.loading = true;
    })
    .addCase(getLockerGroup.fulfilled, (state, action) => {
      state.loading = false;
      state.lockerGroupData = action.payload.data;
    })
    .addCase(getLockerGroup.rejected, (state, action) => {
      state.loading = false;
    })
  },
});

export const {
  setLoading,
} = lockerGroupSlice.actions;

export const getLoading = (state) => state.lockerGroup.loading;
export const getLockerGroupData = (state) => state.lockerGroup.lockerGroupData;

export default lockerGroupSlice.reducer;
