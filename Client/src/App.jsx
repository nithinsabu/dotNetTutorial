import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import ViewImages from './ViewImages';
import UploadImage from './UploadImage';
import './App.css'
function App() {
  return (
    <Router>
      <nav>
        <ul>
          <li><Link to="/">View Images</Link></li>
          <li><Link to="/upload">Upload Image</Link></li>
        </ul>
      </nav>

      <Routes>
        <Route path="/" element={<ViewImages />} />
        <Route path="/upload" element={<UploadImage />} />
      </Routes>
    </Router>
  );
}

export default App;
