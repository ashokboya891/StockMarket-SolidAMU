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
using System.Security.Claims;

namespace Services.StocksService
{
    public class StocksSellOrdersService : ISellOrdersService
    {
        private readonly IStocksRepository _stocksRepository;

        public StocksSellOrdersService(IStocksRepository stocksRepository)
        {
            _stocksRepository = stocksRepository;
        }

        public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest, Guid userId)
        {
            if (sellOrderRequest == null)
                throw new ArgumentNullException(nameof(sellOrderRequest));

            ValidationHelper.ModelValidation(sellOrderRequest);

            SellOrder sellOrder = sellOrderRequest.ToSellOrder();
            sellOrder.SellOrderID = Guid.NewGuid();
            sellOrder.UserID = userId; // Assign the user ID

            SellOrder SellOrderFromRepo = await _stocksRepository.CreateSellOrder(sellOrder, userId);
            return sellOrder.ToSellOrderResponse();
        }

        public async Task<List<SellOrderResponse>> GetSellOrders(Guid userId)
        {
            List<SellOrder> sellOrders = await _stocksRepository.GetSellOrders(userId);
            return sellOrders.Select(temp => temp.ToSellOrderResponse()).ToList();
        }
    }
}
