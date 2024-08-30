using Microsoft.AspNetCore.Mvc;
using SemesterTwo.Models;
using SemesterTwo.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SemesterTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableService _tableService;
        private readonly QueueService _queueService;
        private readonly FileService _fileService;

        public HomeController(BlobService blobService, TableService tableService, QueueService queueService, FileService fileService)
        {
            _blobService = blobService;
            _tableService = tableService;
            _queueService = queueService;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                await _blobService.UploadBlobAsync("product-images", file.FileName, stream);
                return Json(new { success = true, message = "Image uploaded successfully!" });
            }
            return Json(new { success = false, message = "Please select a file to upload." });
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                await _tableService.AddEntityAsync(profile);
                return Json(new { success = true, message = "Customer profile added successfully!" });
            }
            return Json(new { success = false, message = "Invalid profile data. Please check the fields and try again." });
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string orderId)
        {
            if (!string.IsNullOrEmpty(orderId))
            {
                await _queueService.SendMessageAsync("order-processing", $"Processing order {orderId}");
                return Json(new { success = true, message = $"Order {orderId} is being processed." });
            }
            return Json(new { success = false, message = "Order ID cannot be empty." });
        }

        [HttpPost]
        public async Task<IActionResult> UploadContract(IFormFile file)
        {
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                await _fileService.UploadFileAsync("contracts-logs", file.FileName, stream);
                return Json(new { success = true, message = "Contract uploaded successfully!" });
            }
            return Json(new { success = false, message = "Please select a file to upload." });
        }
    }
}
