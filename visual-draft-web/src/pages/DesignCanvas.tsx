import React, { useEffect, useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import { api } from '../api/axios';
import type { Project, Pin, Comment } from '../types'; // Импорт типов из файла
import PinModal from '../components/PinModal';   // Импорт модалки

export default function DesignCanvas() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    
    const [project, setProject] = useState<Project | null>(null);
    const [pins, setPins] = useState<Pin[]>([]);
    const [connection, setConnection] = useState<HubConnection | null>(null);
    
    // Состояние для выбранного пина (если null — модалка закрыта)
    const [selectedPin, setSelectedPin] = useState<Pin | null>(null);
    
    const imageRef = useRef<HTMLImageElement>(null);

    // 1. Первичная загрузка данных (REST API)
    useEffect(() => {
        if (!id) return;
        api.get<Project>(`/projects/${id}`)
           .then(res => {
               setProject(res.data);
               // Убедимся, что comments инициализированы, даже если пришел null
               const safePins = (res.data.pins || []).map(p => ({
                   ...p, 
                   comments: p.comments || []
               }));
               setPins(safePins);
           })
           .catch(err => console.error("Ошибка загрузки:", err));
    }, [id]);

    // 2. Подключение к Real-time (SignalR)
    useEffect(() => {
        if (!id) return;

        const newConnection = new HubConnectionBuilder()
            .withUrl("http://localhost:5048/hubs/design") // ТВОЙ HTTPS ПОРТ
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);

        newConnection.start()
            .then(() => {
                console.log('🟢 SignalR Connected');
                newConnection.invoke("JoinProject", id);
            })
            .catch(e => console.error('🔴 SignalR Error: ', e));

        // === СОБЫТИЯ ===

        // A. Кто-то создал пин
        newConnection.on("PinCreated", (newPin: Pin) => {
            // Инициализируем массив комментов, на всякий случай
            newPin.comments = []; 
            setPins(prev => [...prev, newPin]);
        });

        // B. Кто-то удалил пин
        newConnection.on("PinDeleted", (deletedPinId: string) => {
            setPins(prev => prev.filter(p => p.id !== deletedPinId));
            // Если этот пин был открыт у нас на экране — закрываем модалку
            setSelectedPin(current => (current?.id === deletedPinId ? null : current));
        });

        // C. Кто-то написал комментарий
        newConnection.on("CommentAdded", (data: { pinId: string, comment: Comment }) => {
            // 1. Обновляем список пинов на холсте (чтобы обновился счетчик)
            setPins(prev => prev.map(p => {
                if (p.id === data.pinId) {
                    return { ...p, comments: [...(p.comments || []), data.comment] };
                }
                return p;
            }));
            
            // 2. Если модалка открыта именно на этом пине — обновляем её содержимое в реальном времени
            setSelectedPin(current => {
                if (current && current.id === data.pinId) {
                    return { ...current, comments: [...(current.comments || []), data.comment] };
                }
                return current;
            });
        });

        return () => {
            newConnection.stop();
        };
    }, [id]);

    // Обработчик клика по картинке (Создание пина)
    const handleCanvasClick = async (e: React.MouseEvent<HTMLDivElement>) => {
        if (!project || !imageRef.current || !id) return;

        const rect = imageRef.current.getBoundingClientRect();
        
        // 1. Получаем координаты в пикселях внутри картинки
        const pixelX = e.clientX - rect.left;
        const pixelY = e.clientY - rect.top;

        // 2. === ВАЖНО: Переводим в проценты (от 0 до 1) ===
        // Делим позицию клика на ТЕКУЩУЮ ширину/высоту картинки
        const percentX = pixelX / rect.width;
        const percentY = pixelY / rect.height;

        const text = prompt("💬 Опишите задачу:");
        if (!text) return;

        try {
            await api.post(`/projects/${id}/pins`, {
                content: text,
                x: percentX, // Отправляем проценты!
                y: percentY
            });
        } catch (error) {
            console.error(error);
            alert("Ошибка создания пина");
        }
    };

    if (!project) return <div className="container">Загрузка...</div>;

    return (
        <div className="container">
            <button 
                onClick={() => navigate('/')}
                style={{ marginBottom: '20px', padding: '10px 20px', cursor: 'pointer' }}
            >
                ← Назад
            </button>
            
            <h2>{project.title}</h2>
            
            <div className="canvas-container" onClick={handleCanvasClick}>
                <img 
                    ref={imageRef}
                    src={project.imageUrl} 
                    alt="Design" 
                    className="canvas-image"
                    style={{ maxWidth: '100%', maxHeight: '80vh' }} 
                />

                {pins.map(pin => (
                    <div 
                        key={pin.id}
                        className="pin"
                        style={{ 
                            // === ИСПРАВЛЕНИЕ ===
                            // Умножаем на 100, чтобы получить CSS проценты
                            left: `${pin.x * 100}%`, 
                            top: `${pin.y * 100}%` 
                        }}
                        data-content={pin.comments?.length > 0 ? `Ответов: ${pin.comments.length}` : pin.content}
                        onClick={(e) => {
                            e.stopPropagation();
                            setSelectedPin(pin);
                        }}
                    >
                        {pin.comments?.length > 0 ? pin.comments.length : '!'}
                    </div>
                ))}
            </div>

            {/* Рендер модального окна, если пин выбран */}
            {selectedPin && (
                <PinModal 
                    pin={selectedPin} 
                    onClose={() => setSelectedPin(null)} 
                />
            )}
        </div>
    );
}