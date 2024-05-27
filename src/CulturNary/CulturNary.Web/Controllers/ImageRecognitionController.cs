using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CulturNary.Web.Services;
using CulturNary.Web.Models;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;

namespace CulturNary.Web.Controllers
{
    public class ImageRecognitionController : Controller
    {
        private readonly IImageRecognitionService _imageRecognitionService;
        private readonly ImageStorageService _imageStorageService;

        public ImageRecognitionController(IImageRecognitionService imageRecognitionService, ImageStorageService imageStorageService)
        {
            _imageRecognitionService = imageRecognitionService;
            _imageStorageService = imageStorageService;
        }

        [Authorize(Roles = "Signed,Admin")]
        public IActionResult ImageRecognition()
        {
            return View();
        }

        [Authorize(Roles = "Signed,Admin")]
        [HttpPost]
        public async Task<IActionResult> ImageRecognition(ImageRecognitionResult model, ImageRecognitionRequest request)
        // public async Task<IActionResult> ImageRecognition(ImageRecognitionResult model, IFormFile file)
        {
            if (request.file != null && request.file.Length > 0)
            {
                try
                {
                    var imageUrl = await _imageStorageService.UploadImageAsync(request.file);
                    var prompt = request.zipCode == null || request.zipCode == "" ? "default" : request.zipCode;

                    string base64Image;
                    using (var ms = new MemoryStream())
                    {
                        await request.file.CopyToAsync(ms);
                        base64Image = Convert.ToBase64String(ms.ToArray());
                    }

                    Console.WriteLine("Calling Image Recognition API");
                    // Call the image recognition service
                    var resultJSON = await _imageRecognitionService.ImageRecognitionAsync(base64Image, prompt);
                    var result = JsonConvert.DeserializeObject<OpenAIResponse>(resultJSON);

                    ImageRecognitionResult resultCompiled = new ImageRecognitionResult(){
                        response = result,
                        imageUrl = imageUrl,
                    };  
                    
                    // Return the deserialized model?
                    return View("ImageRecognition", resultCompiled);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while processing the image: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select a file to upload.");
            }

            return View(model);
        }

        [Authorize(Roles = "Signed,Admin")]
        [HttpPost]
        public async Task<IActionResult> ImageRecognitionCam(ImageRecognitionResult model, ImageRecognitionRequest request)
        // public async Task<IActionResult> ImageRecognitionCam(ImageRecognitionResult model, IFormFile file)
        {
            if (request.file != null && request.file.Length > 0)
            {
                try
                {
                    var imageUrl = await _imageStorageService.UploadImageAsync(request.file);
                    var prompt = request.zipCode == null || request.zipCode == "" ? "default" : request.zipCode;

                    string base64Image;
                    using (var ms = new MemoryStream())
                    {
                        await request.file.CopyToAsync(ms);
                        base64Image = Convert.ToBase64String(ms.ToArray());
                    }

                    Console.WriteLine("Calling Image Recognition API");
                    // Call the image recognition service
                    var resultJSON = await _imageRecognitionService.ImageRecognitionAsync(base64Image, prompt);
                    var result = JsonConvert.DeserializeObject<OpenAIResponse>(resultJSON);

                    ImageRecognitionResult resultCompiled = new ImageRecognitionResult(){
                        response = result,
                        imageUrl = imageUrl,
                    };  
                    
                    // Return the deserialized model?
                    return Ok(resultCompiled);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while processing the image: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select a file to upload.");
            }

            return View(model);
        }

    }
}