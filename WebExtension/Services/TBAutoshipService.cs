using System;
using System.Threading.Tasks;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using Microsoft.Extensions.Logging;
using WebExtension.Models.Distributors;

namespace Tori.AutoShips
{
    public interface ITBAutoshipService
    {
        Task<RetryRule> ShouldRetryAutoShipOrder(DateTime retryDate, Order order, Autoship autoShipInfo);
    }

    internal class TBAutoshipService : ITBAutoshipService
    {
        private readonly IOrderService _orderService;
        private readonly IAssociateService _associateService;
        private readonly ILogger<TBAutoshipService> _logger;
        private static readonly string ClassName = typeof(TBAutoshipService).FullName;

        public TBAutoshipService(
            IOrderService orderService,
            IAssociateService associateService,
            ILogger<TBAutoshipService> logger
            )
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// This method determines whether the Autoship ORDER should be retried.
        /// Use this if the client requires any AutoShip order retry rules.
        /// </summary>
        public async Task<RetryRule> ShouldRetryAutoShipOrder(DateTime retryDate, Order order, Autoship autoShipInfo)
        {
            if (order.Void)
            {
                _logger.LogInformation($"{ClassName}.Log Issue", $"Error with Autoship (ID {autoShipInfo.AutoshipId})");
                return new RetryRule
                {
                    Charge = false
                };
            }

            if (order.OrderDate.AddDays(1).Date == retryDate.Date)
            {
                return new RetryRule
                {
                    Charge = true
                };
            }

            if (order.OrderDate.AddDays(2).Date <= retryDate.Date)
            {

                await _orderService.Log(order.OrderNumber, "Order Failed to process multiple times TORI AutoShipService Canceling order to stop processing.");
                await _orderService.CancelOrder(order.OrderNumber);

                var associate = await _associateService.GetAssociate(order.AssociateId);
                var updatedStatus = AssociateStatus.Hold;
                if (associate.StatusId == 2) { updatedStatus = AssociateStatus.Suspended; }
                if (associate.StatusId == 4) { updatedStatus = AssociateStatus.Cancelled; }

                await _associateService.SetAssociateStatus(associate.AssociateId, (int)updatedStatus);

                return new RetryRule
                {
                    Charge = false,
                    Cancel = true,
                    AssociateStatusId = (int)updatedStatus
                };
            }

            return new RetryRule { Charge = false };
        }
    }
}


