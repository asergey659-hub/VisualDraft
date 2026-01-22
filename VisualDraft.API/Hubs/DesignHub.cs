using Microsoft.AspNetCore.SignalR;

namespace VisualDraft.Api.Hubs
{
    /// <summary>
    /// Хаб SignalR для управления обменом сообщениями в реальном времени.
    /// Клиенты подключаются сюда через WebSocket.
    /// </summary>
    public class DesignHub : Hub
    {
        /// <summary>
        /// Подключает пользователя к группе конкретного проекта.
        /// Это нужно, чтобы события (новые пины) приходили только тем, кто смотрит этот проект,
        /// а не всем пользователям сайта вообще.
        /// </summary>
        /// <param name="projectId">ID проекта (Guid в виде строки).</param>
        public async Task JoinProject(string projectId)
        {
            // Добавляем текущее соединение (ConnectionId) в именованную группу
            await Groups.AddToGroupAsync(Context.ConnectionId, projectId);
        }

        /// <summary>
        /// Отключает пользователя от обновлений проекта (например, когда он закрыл вкладку).
        /// </summary>
        public async Task LeaveProject(string projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId);
        }
    }
}