import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getAllGroups, userGroupShow } from "../api/groupApi";

const initialState = {
  loading: false,
  allGroup: [],
  getUserGroup: {},
};

export const groupSlice = createSlice({
  name: "group",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
  },

  extraReducers: (builder) => {
    builder
      .addCase(getAllGroups.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllGroups.fulfilled, (state, action) => {
        state.loading = false;
        state.allGroup = action.payload.data;
      })
      .addCase(getAllGroups.rejected, (state, action) => {
        state.errorMessage = action.payload;
        state.loading = false;
      })
      .addCase(userGroupShow.fulfilled, (state, action) => {
        state.getUserGroup = action.payload;
      })
  },
});

export const {
  setLoading,
} = groupSlice.actions;

export const getLoadingNow = (state) => state.group.loading;
export const getUserGroupData = (state) => state.user.getUserGroup;
export const getAllGroupsData = (state) => state.group.allGroup;

export default groupSlice.reducer;
