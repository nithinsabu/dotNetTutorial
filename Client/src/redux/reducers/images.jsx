import { ADD_IMAGES, DELETE_IMAGE } from "../actions/images";
const initialState = {imagesList: []};

export const imageReducer = (state = initialState, action) =>{
    // console.log(action, state)
    switch(action.type){
        
        case ADD_IMAGES:
            // console.log({...state, images: [...state.images, ...action.payload]})
            return {...state, imagesList: [...state.imagesList, ...action.payload]};
        case DELETE_IMAGE:
            return {...state, imagesList: state.imagesList.filter(image => image.id!==action.payload)}
        default:
            return state;
    }
}



