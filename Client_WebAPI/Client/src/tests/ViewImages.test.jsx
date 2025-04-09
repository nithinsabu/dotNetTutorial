import { describe, it, vi, beforeEach, expect } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { Provider } from 'react-redux';
import configureStore from 'redux-mock-store';
import axios from 'axios';
import ViewImages from '../ViewImages';
import { setImages, deleteImage } from '../redux/actions/images';
// import '@testing-library/jest-dom';
// import  * as matchers from '@testing-library/jest-dom/matchers';
// expect.extend(matchers);
import '@testing-library/jest-dom/vitest'
import React from 'react';
vi.mock('axios');

const mockStore = configureStore([]);

describe('ViewImages Component', () => {
  const store = mockStore({
      images: { imagesList: [{ id: '1', name: 'Image 1', description: 'Test image' }] }
    });
    store.dispatch = vi.fn();

  it('renders images from store', () => {
    render(
      <Provider store={store}>
        <ViewImages />
      </Provider>
    );

    expect(screen.getByText('Images')).toBeInTheDocument();
    expect(screen.getByText('Image 1')).toBeInTheDocument();
    expect(screen.getByText('Test image')).toBeInTheDocument();
  });

  it('fetches images on mount', async () => {
    axios.get.mockResolvedValue({ data: [{ id: '2', name: 'Image 2', description: 'Another test image' }] });

    render(
      <Provider store={store}>
        <ViewImages />
      </Provider>
    );

    await waitFor(() => {
      expect(store.dispatch).toHaveBeenCalledWith(setImages([{ id: '2', name: 'Image 2', description: 'Another test image' }]));
    });
  });

  it('deletes an image when delete button is clicked', async () => {
    axios.delete.mockResolvedValue({});
    const store = mockStore({
      images: { imagesList: [{ id: '1', name: 'Image 1', description: 'Test image' }] }
    });
    store.dispatch = vi.fn();
    render(
      <Provider store={store}>
        <ViewImages />
      </Provider>
    );

    const deleteButton = screen.getByText('Delete');
    fireEvent.click(deleteButton);

    await waitFor(() => {
      expect(store.dispatch).toHaveBeenCalledWith(deleteImage('1'));
    });
  });
});
