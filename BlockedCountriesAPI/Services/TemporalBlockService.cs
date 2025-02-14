using System.Collections.Concurrent;

namespace BlockedCountriesAPI.Services
{
    public class TemporalBlockService :BackgroundService
    {
        private readonly ConcurrentDictionary<string, DateTime> _temporalBlocks = new();

        public void AddTemporaryBlock(string countryCode, int durationMinutes)
        {
            var expirationTime = DateTime.UtcNow.AddMinutes(durationMinutes);
            _temporalBlocks[countryCode] = expirationTime;
        }

        public bool IsTemporarilyBlocked(string countryCode)
        {
            return _temporalBlocks.TryGetValue(countryCode, out var expirationTime) && expirationTime > DateTime.UtcNow;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var expiredBlocks = _temporalBlocks.Where(kv => kv.Value <= now).Select(kv => kv.Key).ToList();
                foreach (var countryCode in expiredBlocks)
                {
                    _temporalBlocks.TryRemove(countryCode, out _);
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
