//using NotificationQueue.Domain.Entities;
//using NotificationQueue.Domain.Repositories;
//using NotificationQueue.Infrastructure.Repositories;
//using System.Text.Json;

//namespace NotificationQueue
//{
//    public class NotificationService
//    {
//        private readonly IMessageQueueService _queueService;
//        private readonly INotificationRepository _notificationRepository;

//        public NotificationService(IMessageQueueService queueService, INotificationRepository notificationRepository)
//        {
//            _queueService = queueService;
//            _notificationRepository = notificationRepository;
//        }

//        public async Task ProcessNotificationAsync(Notification notification)
//        {
//            try
//            {
//                // Сохраняем в базу как "Pending"
//                notification.State = NotificationState.Pending;
//                _notificationRepository.AddAsync(notification);
//                await _notificationRepository.SaveChangesAsync();

//                // Отправляем в нужную очередь
//                var queueName = notification.Channel == NotificationChannel.SMS ? "sms_notifications" : "telegram_notifications";
//                var message = JsonSerializer.Serialize(notification);

//                await _queueService.PublishAsync(queueName, message);

//                // Обновляем статус как "Sent"
//                notification.State = NotificationState.Sent;
//                notification.SentAt = DateTime.UtcNow;
//            }
//            catch (Exception ex)
//            {
//                // Логируем ошибку и обновляем статус
//                notification.State = NotificationState.Failed;
//                notification.ErrorMessage = ex.Message;
//            }
//            finally
//            {
//                await _notificationRepository.SaveChangesAsync();
//            }
//        }

//        public async Task RetryFailedNotificationsAsync()
//        {
//            var failedNotifications = _notificationRepository.Notifications
//                .Where(n => n.State == NotificationState.Failed)
//                .ToList();

//            foreach (var notification in failedNotifications)
//            {
//                notification.State = NotificationState.Retrying;
//                await ProcessNotificationAsync(notification);
//            }
//        }
//    }
//}
