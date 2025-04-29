using Dapr;
using Dapr.Client;
using DaprTutorial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using ProcessFramework;
using System.Diagnostics;

namespace DaprTutorial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private Kernel _kernel;
        private readonly IConfiguration _config;
        private readonly DaprClient _client;
        private static List<object> _messages = new List<object>();

        public HomeController(ILogger<HomeController> logger, Kernel kernel, IConfiguration config, DaprClient client)
        {
            _config = config;
            _kernel = kernel;
            _logger = logger;
            _client = client;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Process(string processid)
        {

            return View();
        }

        [HttpGet, Route("api/runprocess/{processid}")]
        public async Task<IActionResult> RunProcessAsync(string processid,string connectionid)
        {
            DocumentationProcess docprocess = new DocumentationProcess(_config);
            KernelProcess process = docprocess.BuildSimpleProcess();

            UserInputs inputs = new UserInputs()
            {
                ConnectionID = connectionid,
                ProcessId = processid,
                ProductName = "ProductName" + processid,
            };

            DaprKernelProcessContext context = await process.StartAsync(new() { Data = inputs, Id = "start" }, processid);

            return Ok("Process Completeds");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Topic("pubsub", "stepsdata")]
        [HttpPost("stepsdata")]
        public async Task<IActionResult> GetMessages([FromBody] object data)
        {
            _messages.Add(data);
            return Ok();

        }

        [HttpGet("api/messages")]
        public IActionResult AllStepsData()
        {
            return Ok(_messages);
        }

        private async Task SendSSE(object data)
        {
            await Response.WriteAsync($"data: Event {data}\n\n");
            await Response.Body.FlushAsync();
        }
    }
}
