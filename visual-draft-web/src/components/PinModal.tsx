import React, { useState } from 'react';
import type { Pin } from '../types';
import { api } from '../api/axios';

interface Props {
    pin: Pin;
    onClose: () => void;
}

export default function PinModal({ pin, onClose }: Props) {
    const [replyText, setReplyText] = useState("");

    // Удаление всего пина
    const handleDeletePin = async () => {
        if (!confirm("Удалить этот пин и все переписку?")) return;
        try {
            await api.delete(`/pins/${pin.id}`);
            onClose(); // Закрываем окно, SignalR сам удалит кружок с экрана
        } catch (e) {
            console.error(e);
            alert("Ошибка удаления");
        }
    };

    // Отправка ответа
    const handleSendReply = async () => {
        if (!replyText.trim()) return;
        try {
            // Отправляем запрос на добавление комментария
            // Обрати внимание: параметры передаются через query string (?text=...)
            // так как в бэкенде мы принимали (Guid pinId, string text)
            await api.post(`/pins/${pin.id}/comments?text=${encodeURIComponent(replyText)}`);
            setReplyText("");
        } catch (e) {
            console.error(e);
            alert("Не удалось отправить сообщение");
        }
    };

    return (
        <div style={styles.overlay} onClick={onClose}>
            <div style={styles.modal} onClick={e => e.stopPropagation()}>
                {/* Шапка */}
                <div style={styles.header}>
                    <h3 style={{margin: 0}}>Обсуждение</h3>
                    <button onClick={handleDeletePin} style={styles.deleteBtn}>
                        🗑 Удалить Пин
                    </button>
                </div>
                
                {/* История сообщений */}
                <div style={styles.chatArea}>
                    {/* Самый первый комментарий (сам пин) */}
                    <div style={styles.messageRow}>
                        <div style={styles.avatar}>A</div>
                        <div style={styles.bubbleMain}>
                            <strong>Автор:</strong> {pin.content}
                        </div>
                    </div>

                    {/* Ответы */}
                    {pin.comments?.map(c => (
                        <div key={c.id} style={styles.messageRow}>
                            <div style={{...styles.avatar, background: '#74b9ff'}}>R</div>
                            <div style={styles.bubbleReply}>
                                {c.text}
                            </div>
                        </div>
                    ))}
                </div>

                {/* Поле ввода */}
                <div style={styles.inputArea}>
                    <input 
                        value={replyText}
                        onChange={e => setReplyText(e.target.value)}
                        placeholder="Написать ответ..."
                        style={styles.input}
                        onKeyDown={e => e.key === 'Enter' && handleSendReply()}
                        autoFocus
                    />
                    <button onClick={handleSendReply} style={styles.sendBtn}>➤</button>
                </div>
            </div>
        </div>
    );
}

// Простые inline-стили для скорости
const styles: Record<string, React.CSSProperties> = {
    overlay: {
        position: 'fixed', top: 0, left: 0, right: 0, bottom: 0,
        backgroundColor: 'rgba(0,0,0,0.6)', zIndex: 2000,
        display: 'flex', justifyContent: 'center', alignItems: 'center'
    },
    modal: {
        background: '#fff', width: '400px', height: '500px',
        borderRadius: '16px', display: 'flex', flexDirection: 'column',
        boxShadow: '0 20px 60px rgba(0,0,0,0.2)', overflow: 'hidden'
    },
    header: {
        padding: '16px', borderBottom: '1px solid #eee',
        display: 'flex', justifyContent: 'space-between', alignItems: 'center',
        background: '#f8f9fa'
    },
    deleteBtn: {
        background: '#ff7675', color: 'white', border: 'none',
        padding: '6px 12px', borderRadius: '6px', cursor: 'pointer', fontSize: '12px'
    },
    chatArea: {
        flex: 1, padding: '16px', overflowY: 'auto',
        display: 'flex', flexDirection: 'column', gap: '12px',
        background: '#f1f2f6'
    },
    messageRow: { display: 'flex', gap: '10px' },
    avatar: {
        width: '32px', height: '32px', borderRadius: '50%',
        background: '#a29bfe', color: 'white', display: 'flex',
        alignItems: 'center', justifyContent: 'center', fontSize: '12px', fontWeight: 'bold'
    },
    bubbleMain: {
        background: 'white', padding: '10px 14px', borderRadius: '0 12px 12px 12px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.05)', maxWidth: '80%'
    },
    bubbleReply: {
        background: '#fff', padding: '8px 12px', borderRadius: '0 12px 12px 12px',
        border: '1px solid #dfe6e9', maxWidth: '80%'
    },
    inputArea: {
        padding: '12px', borderTop: '1px solid #eee', display: 'flex', gap: '8px', background: 'white'
    },
    input: {
        flex: 1, padding: '10px', borderRadius: '8px', border: '1px solid #ddd', outline: 'none'
    },
    sendBtn: {
        background: '#0984e3', color: 'white', border: 'none',
        width: '40px', borderRadius: '8px', cursor: 'pointer', fontSize: '18px'
    }
};