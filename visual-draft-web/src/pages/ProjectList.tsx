import { useEffect, useState } from 'react';
import { api } from '../api/axios';
import { useNavigate } from 'react-router-dom';
import type { Project } from '../types'
// ================================

export default function ProjectList() {
    const [projects, setProjects] = useState<Project[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        api.get<Project[]>('/projects')
           .then(response => setProjects(response.data))
           .catch(error => console.error("Ошибка API:", error));
    }, []);

    return (
        <div className="container">
            <h1>📂 Мои проекты</h1>
            <div className="grid">
                {projects.map(project => (
                    <div 
                        key={project.id} 
                        className="project-card"
                        onClick={() => navigate(`/project/${project.id}`)}
                    >
                        <h3>{project.title}</h3>
                        <p style={{ color: '#666' }}>ID: {project.id.slice(0, 8)}...</p>
                    </div>
                ))}
            </div>
        </div>
    );
}