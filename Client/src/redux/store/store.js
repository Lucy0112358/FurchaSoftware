import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../slice/authSlice';
import userReducer from '../slice/userSlice';
import menuReducer from '../slice/menuSlice';
import groupReducer from '../slice/groupSlice';
import lockerReducer from '../slice/lockerSlice';
import lockerGroupReducer from '../slice/lockerGroupSlice';
import moduleReducer from '../slice/moduleSlice';
import branchReducer from '../slice/branchSlice';
import adminReducer from '../slice/adminSlice';
import systemReducer from '../slice/systemSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    user: userReducer,
    menu: menuReducer,
    group: groupReducer,
    locker: lockerReducer,
    modules: moduleReducer,
    branch: branchReducer,
    admin: adminReducer,
    system: systemReducer,
    lockerGroup: lockerGroupReducer,
  },
});
