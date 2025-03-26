using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using ServiceContracts.DTO;
using ServiceContracts.FinnhubService;
using ServiceContracts.StocksService;
using StockMarketSolution.Models;
using System.Text.Json;
using System.Security.Claims;

namespace StockMarketSolution.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class TradeController : Controller
    {
        private readonly TradingOptions _tradingOptions;
        private readonly IBuyOrdersService _stocksBuyOrdersService;
        private readonly ISellOrdersService _stocksSellOrdersService;
        private readonly IFinnhubSearchStocksService _finnhubSeachStocksService;
        private readonly IFinnhubCompanyProfileService _finnhubCompanyProfileService;
        private readonly IFinnhubStockPriceQuoteService _finnhubStockPriceQuoteService;
        private readonly IConfiguration _configuration;

        public TradeController(
            IOptions<TradingOptions> tradingOptions,
            IBuyOrdersService stocksBuyOrdersService,
            ISellOrdersService stocksSellOrdersService,
            IFinnhubSearchStocksService finnhubSearchStocksService,
            IFinnhubCompanyProfileService finnhubCompanyProfileService,
            IFinnhubStockPriceQuoteService finnhubStockPriceQuoteService,
            IConfiguration configuration)
        {
            _tradingOptions = tradingOptions.Value;
            _stocksBuyOrdersService = stocksBuyOrdersService;
            _stocksSellOrdersService = stocksSellOrdersService;
            _finnhubSeachStocksService = finnhubSearchStocksService;
            _finnhubCompanyProfileService = finnhubCompanyProfileService;
            _finnhubStockPriceQuoteService = finnhubStockPriceQuoteService;
            _configuration = configuration;
        }
        [Route("[action]/{stockSymbol}")]
        [Route("~/[controller]/{stockSymbol}")]
        public async Task<IActionResult> Index(string stockSymbol)
        {
            //reset stock symbol if not exists
            if (string.IsNullOrEmpty(stockSymbol))
                stockSymbol = "MSFT";


            //get company profile from API server
            Dictionary<string, object>? companyProfileDictionary = await _finnhubCompanyProfileService.GetCompanyProfile(stockSymbol);

            //get stock price quotes from API server
            Dictionary<string, object>? stockQuoteDictionary = await _finnhubStockPriceQuoteService.GetStockPriceQuote(stockSymbol);


            //create model object
            StockTrade stockTrade = new StockTrade() { StockSymbol = stockSymbol };

            //load data from finnHubService into model object
            if (companyProfileDictionary != null && stockQuoteDictionary != null)
            {
                stockTrade = new StockTrade() { StockSymbol = companyProfileDictionary["ticker"].ToString(), StockName = companyProfileDictionary["name"].ToString(), Quantity = _tradingOptions.DefaultOrderQuantity ?? 0, Price = Convert.ToDouble(stockQuoteDictionary["c"].ToString()) };
            }

            //Send Finnhub token to view
            ViewBag.FinnhubToken = _configuration["FinnhubToken"];

            return View(stockTrade);
        }



        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> BuyOrder(BuyOrderRequest buyOrderRequest)
        {
            buyOrderRequest.DateAndTimeOfOrder = DateTime.Now;

            ModelState.Clear();
            TryValidateModel(buyOrderRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View("Index", new StockTrade
                {
                    StockName = buyOrderRequest.StockName,
                    Quantity = buyOrderRequest.Quantity,
                    StockSymbol = buyOrderRequest.StockSymbol
                });
            }

            BuyOrderResponse buyOrderResponse = await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest, GetUserId());
            return RedirectToAction(nameof(Orders));
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> SellOrder(SellOrderRequest sellOrderRequest)
        {
            sellOrderRequest.DateAndTimeOfOrder = DateTime.Now;

            ModelState.Clear();
            TryValidateModel(sellOrderRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View("Index", new StockTrade
                {
                    StockName = sellOrderRequest.StockName,
                    Quantity = sellOrderRequest.Quantity,
                    StockSymbol = sellOrderRequest.StockSymbol
                });
            }

            SellOrderResponse sellOrderResponse = await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest, GetUserId());
            return RedirectToAction(nameof(Orders));
        }

        [Route("[action]")]
        public async Task<IActionResult> Orders()
        {
            Guid userId = GetUserId();
            List<BuyOrderResponse> buyOrderResponses = await _stocksBuyOrdersService.GetBuyOrders(userId);
            List<SellOrderResponse> sellOrderResponses = await _stocksSellOrdersService.GetSellOrders(userId);

            Orders orders = new Orders() { BuyOrders = buyOrderResponses, SellOrders = sellOrderResponses };
            ViewBag.TradingOptions = _tradingOptions;
            return View(orders);
        }

        [Route("OrdersPDF")]
        public async Task<IActionResult> OrdersPDF()
        {
            Guid userId = GetUserId();
            List<IOrderResponse> orders = new List<IOrderResponse>();
            orders.AddRange(await _stocksBuyOrdersService.GetBuyOrders(userId));
            orders.AddRange(await _stocksSellOrdersService.GetSellOrders(userId));
            orders = orders.OrderByDescending(temp => temp.DateAndTimeOfOrder).ToList();

            ViewBag.TradingOptions = _tradingOptions;

            return new ViewAsPdf("OrdersPDF", orders, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }
    }
}
