import { BrowserRouter, Routes, Route } from 'react-router-dom';
import ProjectList from './pages/ProjectList';
import DesignCanvas from './pages/DesignCanvas';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<ProjectList />} />
        <Route path="/project/:id" element={<DesignCanvas />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;