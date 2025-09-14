import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { adminShow, getAllAdmins } from "../api/adminApi";

const initialState = {
  loading: false,
  allAdmins: [],
  adminInfo: {},
  getAdmin: {},
};

export const adminSlice = createSlice({
  name: "admin",
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload.loading;
    },
    setAddAdminInfo: (state, action) => {
      state.adminInfo = {
        ...state.adminInfo,
        ...action.payload,
      };
    },
    setAdmin: (state, action) => {
      state.allAdmins = action.payload.data;
    },
  },

  extraReducers: (builder) => {
    builder
      .addCase(getAllAdmins.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllAdmins.fulfilled, (state, action) => {
        state.loading = false;
        adminSlice.caseReducers.setAdmin(state, action);
      })
      .addCase(getAllAdmins.rejected, (state, action) => {
        state.errorMessage = action.payload;
        state.loading = false;
      })
      .addCase(adminShow.fulfilled, (state, action) => {
        state.getAdmin = action.payload.data;
      })
  },
});

export const {
  setLoading,
  setAddAdminInfo,
} = adminSlice.actions;

export const getLoadingNow = (state) => state.admin.loading;
export const getAdminData = (state) => state.admin.getAdmin;
export const getAllAdminsData = (state) => state.admin.allAdmins;
export const getAddAdminInfo = (state) => state.admin.adminInfo;

export default adminSlice.reducer;
