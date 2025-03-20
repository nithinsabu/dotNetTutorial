import { useEffect } from 'react';
import axios from 'axios';
import {useDispatch, useSelector} from 'react-redux';
import { setImages, deleteImage } from './redux/actions/images';
const ViewImages = () => {
  const dispatch = useDispatch();
  const images = useSelector(state => state.images.imagesList); // what is passed to configureStore = obj. this is obj.reducer.images
  // console.log(images)
  useEffect(()=> {
    console.log(images)
  }, [images])
  // const [images, setImages] = useState([]);
  const deleteImage_ = async (imageID) => {
    try{ 
        await axios.delete(`http://localhost:5253/api/image/${imageID}`)
        // const temp = [...images];
        dispatch(deleteImage(imageID))
    }catch (error) {
        console.error('Error fetching images:', error);
      }
  }
  useEffect(() => {
    const fetchImages = async () => {
      try {
        const response = await axios.get('http://localhost:5253/api/image/');
        dispatch(setImages(response.data))
      } catch (error) {
        console.error('Error fetching images:', error);
      }
    };
    fetchImages();
  }, []);

  return (
    <div className="image-list">
      <h2>Images</h2>
      {images.map((image) => (
        <div key={image.id} className="image-item">
          <h3>{image.name}</h3>
          <p>{image.description}</p>
          <img 
            src={`http://localhost:5253/api/image/${image.id}`} 
            alt={image.name} 
            style={{ maxWidth: '300px', height: 'auto' }}
          />
          <button onClick={() => deleteImage_(image.id)}>Delete</button>
        </div>
      ))}
    </div>
  );
};

export default ViewImages;
