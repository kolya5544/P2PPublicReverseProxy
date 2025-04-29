using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PPublicReverseProxy.Utility
{
    public class PerUserRateLimiter
    {
        private readonly ConcurrentDictionary<long, RollingRateLimiter> _userLimiters = new();
        private readonly int _maxRequests;
        private readonly TimeSpan _window;

        public PerUserRateLimiter(int maxRequests, TimeSpan window)
        {
            _maxRequests = maxRequests;
            _window = window;
        }

        public bool TryRequest(long userId)
        {
            var limiter = _userLimiters.GetOrAdd(userId, _ => new RollingRateLimiter(_maxRequests, _window));
            return limiter.TryRequest();
        }
    }

}
