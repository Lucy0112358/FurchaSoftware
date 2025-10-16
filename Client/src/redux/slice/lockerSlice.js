import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getLockerGroupsByBranchId } from "../api/branchApi";
import { getLockers, getLockerTypes } from "../api/lockerApi";
import { LockerTypes } from "../../enums/Locker/Types";
// import { getAllGroups } from "../api/groupApi";

const initialState = {
  loading: false,
  allLockers: [],
  lockerFilters: {
    isOpen: null,
  },
  filteredLockerGroups: [],
  selectedLockerIds: [],
  lockerTypesData: {},
  lockerFilterTypesData: []
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
     updateLockerDoorState: (state, action) => {
      const { lockerId, doorState } = action.payload;
      state.allLockers = state.allLockers.map((office) => ({
        ...office,
        lockers: office.lockers.map((lockerGroup) => ({
          ...lockerGroup,
          groupLockers: lockerGroup.groupLockers.map((locker) =>
            locker.id === lockerId
              ? { ...locker, doorState }
              : locker
          ),
        })),
      }));
    },
    setLockerType: (state, action) => {
      state.lockerTypesData = action.payload.data;
    },
    setLockerFilterType: (state, action) => {
      const types = LockerTypes
      const payloadData = action.payload.data || []
      const merged = types.map((item) => {
        const type = payloadData.find((t) => t.type === item.type);
        return {
          ...item,
          id: type.id,
        };
      });
      
      state.lockerFilterTypesData = merged;
    },

    setSelectedLockerIds: (state, action) => {
      state.selectedLockerIds = action.payload;
    },
    clearFilteredLockerGroups: (state) => {
      state.filteredLockerGroups = [];
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
      .addCase(getLockerTypes.fulfilled, (state, action) => {
        lockerSlice.caseReducers.setLockerType(state, action);
        lockerSlice.caseReducers.setLockerFilterType(state, action);

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
  setSelectedLockerIds,
  clearFilteredLockerGroups,
  updateLockerDoorState,
} = lockerSlice.actions;

export const getLoadingNow = (state) => state.locker.loading;
export const getLockerFilter = (state) => state.locker.lockerFilters
export const getAllLockersData = (state) => state.locker.allLockers;
export const getFilteredLockerGroups = (state) => state.locker.filteredLockerGroups;
export const getSelectedLockerIds = (state) => state.locker.selectedLockerIds;
export const getLockerTypesData = (state) => state.locker.lockerTypesData;
export const getFilterLockerTypesData = (state) => state.locker.lockerFilterTypesData;

export default lockerSlice.reducer;
