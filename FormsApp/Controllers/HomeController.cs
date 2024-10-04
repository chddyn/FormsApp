using FormsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FormsApp.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {

        }

        public IActionResult Index(string searchString, string category)
        {
            var products = Repository.Products;

            if (!String.IsNullOrEmpty(searchString))
            {
                ViewBag.Search = searchString;
                products = products.Where(x => x.Name!.ToLower().Contains(searchString)).ToList();
            }


            if (!String.IsNullOrEmpty(category) && category != "0")
            {
                ViewBag.Search = category;
                products = products.Where(x => x.CategoryId == int.Parse(category)).ToList();
            }


            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name", category);

            //var model = new ProductViewModel
            //{
            //    Products = products,
            //    Categories=Repository.Categories,
            //    SelectedCategory=category
            //};

            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            //  ViewBag.Categories= Repository.Categories;

            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product model, IFormFile imageFile)
        {

            var extenstion = "";
            if (imageFile != null)
            {
                extenstion = Path.GetExtension(imageFile.FileName);
                var allowedExtension = new[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtension.Contains(extenstion))
                {
                    ModelState.AddModelError("", "Geçerli bir format seçiniz");
                }

            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {


                    var random = string.Format($"{Guid.NewGuid().ToString()}{extenstion}");
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", random);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    model.Image = random;
                    model.ProductId = Repository.Products.Count() + 1;
                    Repository.Products.Add(model);
                    return RedirectToAction("Index");
                }

            }
            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");

            return View(model);
        }


        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }
            var entity = Repository.Products.FirstOrDefault(x => x.ProductId == id);
            if (entity == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");

            return View(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product model, IFormFile? imageFile)
        {


            if (id != model.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var extenstion = Path.GetExtension(imageFile.FileName);
                    var random = string.Format($"{Guid.NewGuid().ToString()}{extenstion}");
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", random);



                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    model.Image = random;
                }
                Repository.EditProduct(model);
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");

            return View(model);
        }



        public ActionResult Delete(int? id)
        {
            if (id == null) { return NotFound(); }

            var entity = Repository.Products.FirstOrDefault(x => x.ProductId == id);

            if (entity == null) { return NotFound(); }

            // Repository.Delete(entity);

            return View("DeleteConfirm", entity);
        }

        [HttpPost]
        public ActionResult Delete(int? id, int productId)
        {
            if (id != productId)
            {
                return NotFound();
            }

            var entity = Repository.Products.FirstOrDefault(x => x.ProductId == productId);
            if (entity == null)
            {
                return NotFound();
            }

            Repository.Delete(entity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditProducts(List<Product> products)
        {
            foreach (var product in products)
            {

                Repository.EditProductIsActive(product);
            }

            return RedirectToAction("Index");
        }
    }
}
