export const ADD_IMAGES = 'ADD_IMAGES';
export const DELETE_IMAGE = 'DELETE_IMAGE';
export const addImages = (images) => ({ type: ADD_IMAGES, payload: images });
export const deleteImage = (Id) => ({ type: DELETE_IMAGE, payload: Id });

