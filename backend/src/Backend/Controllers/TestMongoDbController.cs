//using System;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using Microsoft.AspNetCore.Mvc;
//using MongoDB.Driver;
//using Citolab.QConstruction.Model;

//namespace Citolab.QConstruction.Backend.Controllers
//{
//    /// <summary>
//    /// Controller to test the MongoDB connection
//    /// </summary>
//    [Route("tests/[controller]")]
//    public class TestMongoDbController : Controller
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        [Route("index")]
//        public IActionResult Index()
//        {
//            //var response = new HttpResponseMessage();

//            try
//            {

//                var settings = new MongoClientSettings
//                {
//                    //Credentials = new[] { credential },
//                    Server = MongoServerAddress.Parse("10.1.0.5:27017")
//                };
//                var client = new MongoClient(settings);

//                var database = client.GetDatabase("QConstruction");
//                var collection = database.GetCollection<User>(typeof(User).Name);
//                var thing = collection.FindAsync(u => u.Name != "").Result.FirstOrDefault();
//                return Ok("Success while connecting to 10.1.0.5:27017.");

//                // response.Content = new StringContent("<html><body>Success while connecting to 10.5.0.1:27017.</body></html>");
//            }
//            catch (AggregateException aggregateException)
//            {
//                var exceptionMessages = string.Empty;
//                foreach (var innerException in aggregateException.InnerExceptions)
//                {
//                    exceptionMessages += innerException.Message + Environment.NewLine;
//                }
//                return Content($"<html><body>Error while connecting to 10.1.0.5:27017. Exception: {aggregateException.Message}. {exceptionMessages}</body></html>", new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
//            }
//            catch (Exception exception)
//            {
//                return Content($"<html><body>Error while connecting to 10.1.0.5:27017. Exception: {exception.Message}</body></html>", new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
//                //response.Content = new StringContent($"<html><body>Error while connecting to 10.1.0.5:27017. Exception: {exception.Message}</body></html>");
//                //                throw;
//            }

//           // response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

//           // return response;
//        }
//    }
//}
