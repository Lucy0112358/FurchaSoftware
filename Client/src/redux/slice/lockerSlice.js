import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getLockerGroupsByBranchId } from "../api/branchApi";
import { getLockers } from "../api/lockerApi";
// import { getAllGroups } from "../api/groupApi";

const initialState = {
  loading: false,
  allLockers: [],
  lockerFilters: {
    lockerStatus: null,
  },
  filteredLockerGroups: [],
  selectedLockerIds: [],
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
    setLocker: (state, action) => {
      state.allLockers = action.payload.data;
    },
    setSelectedLockerIds: (state, action) => {
      state.selectedLockerIds = action.payload;
    },
  },

  extraReducers: (builder) => {
    builder
      .addCase(getLockerGroupsByBranchId.fulfilled, (state, action) => {
        state.filteredLockerGroups = action.payload.data;
      })
      .addCase(getLockers.pending, (state) => {
        state.loading = true;
      })
      .addCase(getLockers.fulfilled, (state, action) => {
        state.loading = false;
        lockerSlice.caseReducers.setLocker(state, action);
      })
      .addCase(getLockers.rejected, (state, action) => {

      })
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
  setLockerFilter,
  setSelectedLockerIds
} = lockerSlice.actions;

export const getLoadingNow = (state) => state.locker.loading;
export const getLockerFilter = (state) => state.locker.lockerFilters
export const getAllLockersData = (state) => state.locker.allLockers;
export const getFilteredLockerGroups = (state) => state.locker.filteredLockerGroups;
export const getSelectedLockerIds = (state) => state.locker.selectedLockerIds;

export default lockerSlice.reducer;
