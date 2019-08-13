using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UnivApp.Models;

namespace UnivApp.Pages.Students
{
    public class EditModel : PageModel
    {
        private readonly UnivApp.Models.SchoolContext _context;
				private IHostingEnvironment _environment;

				[BindProperty, Display(Name="File")]
				public IFormFile UploadedFile { get; set; }

        public EditModel(UnivApp.Models.SchoolContext context, IHostingEnvironment environment)
        {
            _context = context;
						_environment = environment;
        }

		public ActionResult OnPostFile() {

			IFormFile Upload = Request.Form.Files[0];
			var path = Path.Combine(_environment.ContentRootPath, "uploads", Upload.FileName);
			using (var fileStream = new FileStream(path, FileMode.Create)) {
				Upload.CopyToAsync(fileStream);
				return Page();
			}
		}

		[BindProperty]
        public Student Student { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Student = await _context.Student.FirstOrDefaultAsync(m => m.ID == id);

            if (Student == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(Student.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.ID == id);
        }
    }

	
}
