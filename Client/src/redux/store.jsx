import { configureStore } from '@reduxjs/toolkit';
import {imageReducer} from './reducers/images';

const store = configureStore({
  reducer: {
    images: imageReducer
  }
});

export default store;