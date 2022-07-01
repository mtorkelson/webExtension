using System;
using System.Threading.Tasks;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Autoships;
using Tori.AutoShips;

namespace tori.Hooks.Autoships
{
    public class ShouldRetryAutoshipOrderHook : IHook<ShouldRetryAutoshipOrderHookRequest, ShouldRetryAutoshipOrderHookResponse>
    {
        private readonly ITBAutoshipService _tbAutoshipService;
        public ShouldRetryAutoshipOrderHook(ITBAutoshipService tbAutoshipService)
        {
            _tbAutoshipService = tbAutoshipService;
        }

        public async Task<ShouldRetryAutoshipOrderHookResponse> Invoke(ShouldRetryAutoshipOrderHookRequest request, Func<ShouldRetryAutoshipOrderHookRequest, Task<ShouldRetryAutoshipOrderHookResponse>> func)
        {
            RetryRule RetryRule = await _tbAutoshipService.ShouldRetryAutoShipOrder(request.RetryDate, request.Order, request.Autoship);

            ShouldRetryAutoshipOrderHookResponse shouldRetryAutoship = new ShouldRetryAutoshipOrderHookResponse
            {
                RetryRule = RetryRule
            };

            return shouldRetryAutoship;
        }
    }
}