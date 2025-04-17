import { useState } from 'react';
import axios from 'axios';

const UploadImage = () => {
  const [file, setFile] = useState(null);
  const [description, setDescription] = useState('');
  const [isUploading, setIsUploading] = useState(false);
  const [objects, setObjects] = useState({});
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!file) return;

    setIsUploading(true);
    const formData = new FormData();
    formData.append('file', file);
    formData.append('description', description);

    try {
      const response = await axios.post(`${import.meta.env.VITE_API_URL}/api/Image/upload`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      // console.log(JSON.stringify(response.data, null, 2))
      setObjects(response.data)
      alert('Image uploaded successfully!');
      setFile(null);
      setDescription('');
    } catch (error) {
      console.error('Upload failed:', error);
      alert('Upload failed');
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <div className="upload-form">
      <h2>Upload Image</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Image File:</label>
          <input
            type="file"
            accept="image/*"
            onChange={(e) => setFile(e.target.files[0])}
            required
          />
        </div>
        
        <div>
          <label>Description:</label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
          />
        </div>

        <button type="submit" disabled={isUploading}>
          {isUploading ? 'Uploading...' : 'Upload Image'}
        </button>
      </form>
        {Object.entries(objects).map(([key, value]) => (
          <li key={key}>{`${key}: ${value}`}</li>
        ))}
        
    </div>

  );
};

export default UploadImage;
