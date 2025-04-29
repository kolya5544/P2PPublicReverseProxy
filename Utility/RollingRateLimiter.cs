using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PPublicReverseProxy.Utility
{
    public class RollingRateLimiter
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _window;
        private readonly Queue<DateTime> _requestTimestamps = new();
        private readonly object _lock = new();

        public RollingRateLimiter(int maxRequests, TimeSpan window)
        {
            _maxRequests = maxRequests;
            _window = window;
        }

        public bool TryRequest()
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;

                // Remove expired timestamps
                while (_requestTimestamps.Count > 0 && now - _requestTimestamps.Peek() > _window)
                    _requestTimestamps.Dequeue();

                if (_requestTimestamps.Count >= _maxRequests)
                    return false; // rate limit exceeded

                _requestTimestamps.Enqueue(now);
                return true;
            }
        }
    }
}
