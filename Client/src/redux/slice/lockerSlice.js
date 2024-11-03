import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
// import { getAllGroups } from "../api/groupApi";

const initialState = {
  loading: false,
  allLockers: [],
  lockerFilters: {
    lockerStatus: 'all',
  },
};

export const lockerSlice = createSlice({
  name: "locker",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
    setLockerFilter: (state, action) => {
      // state.lockerFilters = {
      //   ...state.lockerFilters,
      //   ...action.payload,}
      state.lockerFilters = action.payload
    },
  },

  extraReducers: (builder) => {
    builder
      // .addCase(getAllGroups.pending, (state) => {
      //   state.loading = true;
      // })
      // .addCase(getAllGroups.fulfilled, (state, action) => {
      //   state.loading = false;
      //   state.allGroup = action.payload.data;
      // })
      // .addCase(getAllGroups.rejected, (state, action) => {
      //   state.errorMessage = action.payload;
      //   state.loading = false;
      // })
  },
});

export const {
  setLoading,
  setLockerFilter
} = lockerSlice.actions;

export const getLoadingNow = (state) => state.locker.loading;

export const getLockerFilter = (state) => state.locker.lockerFilters

export const getAllLockersData = (state) => state.locker.allLockers;

export default lockerSlice.reducer;
