import { useEffect, useState } from 'react';
import axios from 'axios';

const ViewImages = () => {
  const [images, setImages] = useState([]);
  const deleteImage = async (imageID) => {
    try{ 
        const response = await axios.delete(`http://localhost:5253/api/image/${imageID}`)
        const temp = [...images];
        setImages(temp.filter((img) => img.id!==imageID))
    }catch (error) {
        console.error('Error fetching images:', error);
      }
  }
  useEffect(() => {
    const fetchImages = async () => {
      try {
        const response = await axios.get('http://localhost:5253/api/image/');
        console.log(response.data)
        setImages(response.data);
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
          <button onClick={() => deleteImage(image.id)}>Delete</button>
        </div>
      ))}
    </div>
  );
};

export default ViewImages;
