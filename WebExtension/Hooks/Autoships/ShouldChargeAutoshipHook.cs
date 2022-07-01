using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Autoships;
using System;
using System.Threading.Tasks;

namespace WebExtension.Hooks.Autoships
{
    public class ShouldChargeAutoshipHook : IHook<ShouldChargeAutoshipHookRequest, ShouldChargeAutoshipHookResponse>
    {
        public async Task<ShouldChargeAutoshipHookResponse> Invoke(ShouldChargeAutoshipHookRequest request, Func<ShouldChargeAutoshipHookRequest, Task<ShouldChargeAutoshipHookResponse>> func)
        {
            var response = new ShouldChargeAutoshipHookResponse
            {
                RetryRule = new RetryRule { Charge = request.Autoship.NextProcessDate.Date <= request.Now.Date }
            };
            return response;
        }
    }
}
