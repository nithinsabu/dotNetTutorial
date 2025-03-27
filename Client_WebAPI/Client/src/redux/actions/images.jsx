export const SET_IMAGES = 'SET_IMAGES';
export const DELETE_IMAGE = 'DELETE_IMAGE';
export const ADD_IMAGE = 'ADD_IMAGE'
export const setImages = (images) => ({ type: SET_IMAGES, payload: images });
export const deleteImage = (Id) => ({ type: DELETE_IMAGE, payload: Id });

