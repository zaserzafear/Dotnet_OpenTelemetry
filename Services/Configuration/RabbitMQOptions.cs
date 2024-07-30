namespace Services.Configuration
{
    internal class RabbitMQOptions
    {
        public required string Host { get; init; }
        public required string User { get; init; }
        public required string Pass { get; init; }
        public required RabbitMQConsumeProperties AuthRequest { get; init; }
    }

    internal class RabbitMQConsumeProperties
    {
        public required int PrefetchCount { get; init; }
        public required int ConcurrentMessageLimit { get; init; }
    }
}
