import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";

const initialState = {
  manage: false,
};

export const systemSlice = createSlice({
  name: "system",
  initialState,
  reducers: {
    setManageEnabled: (state, action) => {
      state.manage = action.payload;
    },
  },

  extraReducers: (builder) => {
  },
});

export const {
  setManageEnabled,
} = systemSlice.actions;

export const getManage = (state) => state.system.manage;

export default systemSlice.reducer;
