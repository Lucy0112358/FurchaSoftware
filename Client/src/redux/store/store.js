import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../slice/authSlice';
import userReducer from '../slice/userSlice';
import menuReducer from '../slice/menuSlice';
import groupReducer from '../slice/groupSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    user: userReducer,
    menu: menuReducer,
    group: groupReducer,
  },
});
