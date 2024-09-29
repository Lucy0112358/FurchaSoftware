import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../slice/authSlice';
import userReducer from '../slice/userSlice';
import menuReducer from '../slice/menuSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    user: userReducer,
    menu: menuReducer,
  },
});
