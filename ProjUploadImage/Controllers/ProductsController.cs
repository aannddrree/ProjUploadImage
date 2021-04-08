using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjUploadImage.Data;
using ProjUploadImage.Models;

namespace ProjUploadImage.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProjUploadImageContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(ProjUploadImageContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,ImageFile")] Product product)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                string extension = Path.GetExtension(product.ImageFile.FileName);
                product.Image = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/image", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,ImageFile")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Verificar se o nome da imagem mudou:
                    var productCompare = _context.Product.Find(product.Id);

                    product.Image = (product.ImageFile == null) ? "": product.ImageFile.FileName;

                    if (!CompareFileName(productCompare.Image, product.Image))
                    {
                        //Remover Imagem anterior
                        var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", productCompare.Image);
                        if (System.IO.File.Exists(imagePath))
                            System.IO.File.Delete(imagePath);

                        //Incluir nova
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                        string extension = Path.GetExtension(product.ImageFile.FileName);
                        product.Image = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/image", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    productCompare.Description = product.Description;
                    productCompare.Image = string.IsNullOrEmpty(product.Image) ? productCompare.Image : product.Image;

                    _context.Update(productCompare);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        private bool CompareFileName(string name, string newName)
        {
            //Se não foi selecionada uma imagem nova fica a antiga. 
            if (string.IsNullOrEmpty(newName))
                return true;

            if (string.IsNullOrEmpty(name))
                return false;

            //extensão do arquivo
            var validateName = name.Split('.');
            var validateNewName = newName.Split('.');

            if (validateName[1] != validateNewName[1])
                return false;            

            //Remover a data gerada
            string nameOld = validateName[0].Replace(validateName[0]
                                            .Substring(validateName[0].Length - 9, 9), "");

            if (newName == nameOld)
                return true;

            return false;
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);

            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", product.Image);

            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
