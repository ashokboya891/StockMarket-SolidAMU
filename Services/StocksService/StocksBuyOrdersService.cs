using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts.DTO;
using ServiceContracts.StocksService;
using Services.Helpers;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.StocksService
{
    public class StocksBuyOrdersService : IBuyOrdersService
    {
        private readonly IStocksRepository _stocksRepository;

        /// <summary>
        /// Constructor of StocksBuyOrdersService class that executes when a new object is created.
        /// </summary>
        public StocksBuyOrdersService(IStocksRepository stocksRepository)
        {
            _stocksRepository = stocksRepository;
        }

        /// <summary>
        /// Creates a new buy order.
        /// </summary>
        /// <param name="buyOrderRequest">The buy order request DTO.</param>
        /// <returns>Returns a BuyOrderResponse object.</returns>
        public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest,Guid userId)
        {
            if (buyOrderRequest == null)
                throw new ArgumentNullException(nameof(buyOrderRequest));

            // Model validation
            ValidationHelper.ModelValidation(buyOrderRequest);

            // Business rule validation: Date should not be older than Jan 01, 2000
            if (buyOrderRequest.DateAndTimeOfOrder < new DateTime(2000, 1, 1))
            {
                throw new ArgumentException("Date of the order should not be older than Jan 01, 2000.");
            }

            // Convert DTO to entity
            BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();
            buyOrder.BuyOrderID = Guid.NewGuid(); // Generate unique ID

            // Save to repository
            BuyOrder buyOrderFromRepo = await _stocksRepository.CreateBuyOrder(buyOrder, userId);

            // Convert to response DTO
            return buyOrderFromRepo.ToBuyOrderResponse();
        }

        /// <summary>
        /// Retrieves all buy orders.
        /// </summary>
        /// <returns>Returns a list of BuyOrderResponse DTOs.</returns>
        public async Task<List<BuyOrderResponse>> GetBuyOrders(Guid userId)
        {
            List<BuyOrder> buyOrders = await _stocksRepository.GetBuyOrders(userId);
            return buyOrders.Select(buyOrder => buyOrder.ToBuyOrderResponse()).ToList();
        }
    }
}
