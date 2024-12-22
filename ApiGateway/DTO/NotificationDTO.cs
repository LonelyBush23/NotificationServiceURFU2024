namespace ApiGateway.DTO
{
    public sealed class NotificationDTO
    {
        public required string Receiver { get; set; }
        public required string Message { get; set; }
        public NotificationType Type { get; set; }
    }
}
