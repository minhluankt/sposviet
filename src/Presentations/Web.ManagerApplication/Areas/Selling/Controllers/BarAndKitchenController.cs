using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Abstractions;

namespace Web.ManagerApplication.Areas.Selling.Controllers
{
    [Area("Selling")]
    public class BarAndKitchenController : BaseController<BarAndKitchenController>
    {
        // GET: BarAndKitchenController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BarAndKitchenController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BarAndKitchenController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BarAndKitchenController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BarAndKitchenController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BarAndKitchenController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BarAndKitchenController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BarAndKitchenController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
