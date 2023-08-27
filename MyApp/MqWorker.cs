using ServiceStack.Messaging;

namespace MyApp;

public class MqWorker : BackgroundService
{
    private const int MqStatsDescriptionDurationMs = 10000;

    private readonly ILogger<MqWorker> logger;

    private readonly IMessageService mqServer;

    public MqWorker(ILogger<MqWorker> logger, IMessageService mqServer)
    {
        this.logger = logger;
        this.mqServer = mqServer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.mqServer.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("MQ Worker running at: {Stats}", this.mqServer.GetStatsDescription());
            await Task.Delay(MqStatsDescriptionDurationMs, stoppingToken);
        }

        this.mqServer.Stop();
    }
    
}