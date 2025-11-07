namespace Analytics.Shared.Settings
{
    public class RabbitMQSettings
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string Exchange { get; set; } = string.Empty;
        public string Queue { get; set; } = string.Empty;
        public string DeadLetterQueue { get; set; } = string.Empty;

        

    }
}
