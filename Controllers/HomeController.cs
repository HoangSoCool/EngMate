using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TiengAnh.Models;
using TiengAnh.Repositories;
using System.Linq;
using TiengAnh.Services;
using System.Collections.Generic;

namespace TiengAnh.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITopicRepository _topicRepository;
        private readonly VocabularyRepository _vocabularyRepository;

        public HomeController(MongoDbService mongoDbService, ILogger<HomeController> logger, 
            ITopicRepository topicRepository, VocabularyRepository vocabularyRepository) 
            : base(mongoDbService)
        {
            _logger = logger;
            _topicRepository = topicRepository;
            _vocabularyRepository = vocabularyRepository;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Tải dữ liệu chủ đề cho trang chủ");
            var topics = await _topicRepository.GetAllAsync();
            
            // Lấy danh sách tất cả từ vựng để đếm
            var allVocabularies = await _vocabularyRepository.GetAllAsync();

            // Cập nhật số lượng từ cho mỗi chủ đề
            foreach (var topic in topics)
            {
                int wordCount = allVocabularies.Count(v => v.ID_CD == topic.ID_CD);
                topic.WordCount = wordCount;
                topic.TotalItems = wordCount;
            }
            
            // Chỉ hiển thị 6 chủ đề đầu tiên trên trang chủ
            var featuredTopics = topics.Take(6).ToList();
            
            ViewBag.Topics = featuredTopics;
            
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

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                // Kiểm tra nếu có lỗi được lưu trong TempData
                if (TempData["ErrorMessage"] != null)
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
                    _logger.LogError("Lỗi từ TempData: {ErrorMessage}", (string)ViewBag.ErrorMessage);
                }
                else
                {
                    // Kiểm tra các loại lỗi phổ biến
                    var exceptionFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                    if (exceptionFeature != null)
                    {
                        var exception = exceptionFeature.Error;
                        ViewBag.ErrorMessage = exception.Message;
                        
                        if (exception is TimeoutException)
                        {
                            ViewBag.ErrorMessage = "Không thể kết nối tới cơ sở dữ liệu MongoDB. Vui lòng kiểm tra cấu hình kết nối.";
                        }
                        else if (exception is System.Net.Sockets.SocketException)
                        {
                            ViewBag.ErrorMessage = "Lỗi kết nối mạng. Vui lòng kiểm tra kết nối internet và cấu hình MongoDB.";
                        }
                        
                        _logger.LogError(exception, "Chi tiết lỗi: {ErrorMessage}", exception.Message);
                    }
                }
                
                // Log lỗi để debug
                _logger.LogError("Đã xảy ra lỗi. RequestId: {RequestId}", 
                    Activity.Current?.Id ?? HttpContext.TraceIdentifier);
                
                // Hiển thị thông tin lỗi
                return View(new ErrorViewModel { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
                });
            }
            catch (Exception ex)
            {
                // Xử lý khi error handler gặp lỗi
                _logger.LogError(ex, "Lỗi nghiêm trọng trong trang Error: {Message}", ex.Message);
                return Content($"Đã xảy ra lỗi nghiêm trọng: {ex.Message}. Vui lòng kiểm tra logs để biết thêm chi tiết.");
            }
        }
    }
}
