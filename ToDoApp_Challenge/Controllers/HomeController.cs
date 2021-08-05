using Couchbase;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ToDoApp_Challenge.Models;

namespace ToDoApp_Challenge.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICouchbaseCollection _collection;

        public HomeController(ILogger<HomeController> logger, IBucketProvider bucketProvider)
        {
            _logger = logger;
            _collection = bucketProvider.GetBucketAsync("Main").Result.Scope("_default").Collection("_default");
        }

        public IActionResult Index()
        {
            List<TodoModel> data;

            try
            {
                data = _collection.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result.ContentAs<UserToDoListModel>().ToDoList;
            } catch (AggregateException)
            {
                data = new List<TodoModel>();
            }

            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TodoModel theNewModel)
        {
            if (theNewModel != null)
            {
                if (!String.IsNullOrEmpty(theNewModel.Content))
                {
                    theNewModel.Id = Guid.NewGuid().ToString();

                    UserToDoListModel data;

                    try
                    {
                        data = _collection.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result.ContentAs<UserToDoListModel>();
                    }
                    catch (AggregateException)
                    {
                        data = new UserToDoListModel();
                    }

                    data.ToDoList.Add(theNewModel);
                    _ = _collection.UpsertAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), data).Result;
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            var data = _collection.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result.ContentAs<UserToDoListModel>().ToDoList.Find(item => item.Id == id);

            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var data = _collection.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result.ContentAs<UserToDoListModel>();
            data.ToDoList.RemoveAll(item => item.Id == id);
            _ = _collection.UpsertAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), data).Result;

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
