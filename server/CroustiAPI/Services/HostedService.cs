public class HostedService : IHostedService
{
    private DuckDnsService duckDnsService;
    private Timer timer = null;

    public HostedService(DuckDnsService duckDnsService)
    {
        this.duckDnsService = duckDnsService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.timer = new Timer(UpdateIp, null, TimeSpan.Zero, TimeSpan.FromMinutes(60));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.timer.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    private void UpdateIp(object state)
    {
        Task.WaitAll(new [] { this.duckDnsService.UpdateDns() });
    }
}