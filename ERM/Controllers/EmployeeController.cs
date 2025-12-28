using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Organization.Data;
using Organization.DataModels;
using System.Text.Json;

namespace Organization.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly OrganizationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        // ----------------- CONSTRUCTOR -----------------
        public EmployeeController(OrganizationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ----------------- HELPER METHODS -----------------

        // Load country codes from JSON file
        private void LoadCountryCodes()
        {
            var jsonPath = Path.Combine(_environment.WebRootPath, "data/countryCodes.json");

            if (!System.IO.File.Exists(jsonPath))
            {
                ViewBag.CountryCodes = new List<SelectListItem>();
                return;
            }

            var jsonData = System.IO.File.ReadAllText(jsonPath);
            var countries = JsonSerializer.Deserialize<List<CountryCode>>(jsonData,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.CountryCodes = countries
                .Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = $"{c.Country} ({c.Code})"
                })
                .ToList();
        }

        public class CountryCode
        {
            public string Country { get; set; }
            public string Code { get; set; }
            public string Iso { get; set; }
        }

        // ----------------- INDEX -----------------
        public async Task<IActionResult> Index()
        {
            return View(await _context.Emploees.ToListAsync());
        }

        // ----------------- DETAILS -----------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Emploees.FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // ----------------- CREATE (GET) -----------------
        public IActionResult Create()
        {
            LoadCountryCodes();
            return View();
        }

        // ----------------- CREATE (POST) -----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee, IFormFile? ProfilePicture, List<IFormFile>? Documents)
        {
            // ----------------- DATE VALIDATIONS -----------------
            var today = DateTime.Today;
            var minDOB = today.AddYears(-18);

            // DOB cannot be in future
            if (employee.DOB > today)
            {
                ModelState.AddModelError("DOB", "Date of Birth cannot be in the future.");
            }

            // DOB must be at least 18 years old
            if (employee.DOB > minDOB)  // this allows someone who turns 18 today
            {
                ModelState.AddModelError("DOB", "Employee must be at least 18 years old.");
            }

            // DOJ must be after DOB (can be today)
            if (employee.DOJ < employee.DOB)
            {
                ModelState.AddModelError("DOJ", "Date of Joining cannot be before Date of Birth.");
            }

            // DOE must be after DOJ (optional)
            if (employee.DOE.HasValue && employee.DOE <= employee.DOJ)
            {
                ModelState.AddModelError("DOE", "Date of Exit must be after Date of Joining.");
            }


            // ----------------- PROFILE PICTURE -----------------
            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                await HandleProfilePictureUpload(employee, ProfilePicture);
            }

            // ----------------- STOP IF INVALID -----------------
            if (!ModelState.IsValid)
                return View(employee);

            // ----------------- DOCUMENTS -----------------
            if (Documents != null && Documents.Count > 0)
            {
                await HandleDocumentUploads(employee, Documents);
            }

            // ----------------- SAVE EMPLOYEE -----------------
            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ----------------- EDIT (GET) -----------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Emploees.FindAsync(id);
            if (employee == null) return NotFound();

            LoadCountryCodes();
            return View(employee);
        }

        // ----------------- EDIT (POST) -----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee, IFormFile? ProfilePicture, List<IFormFile>? Documents)
        {
            if (id != employee.Id) return NotFound();

            var existingEmployee = await _context.Emploees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (existingEmployee == null) return NotFound();

            // ----------------- PROFILE PICTURE -----------------
            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                await HandleProfilePictureUpload(employee, ProfilePicture, existingEmployee);
            }
            else
            {
                employee.ProfilePicture = existingEmployee.ProfilePicture;
                employee.TempProfilePicture = existingEmployee.TempProfilePicture;
            }

            // ----------------- DOCUMENTS -----------------
            if (Documents != null && Documents.Count > 0)
            {
                await HandleDocumentUploads(employee, Documents);
            }
            else
            {
                employee.Document1 = existingEmployee.Document1;
                employee.Document2 = existingEmployee.Document2;
                employee.Document3 = existingEmployee.Document3;
                employee.Document4 = existingEmployee.Document4;
            }

            _context.Update(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ----------------- DELETE (GET) -----------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Emploees.FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // ----------------- DELETE (POST) -----------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Emploees.FindAsync(id);
            if (employee != null)
            {
                _context.Emploees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ----------------- PRIVATE HELPER METHODS -----------------

        private bool employeeExists(int id)
        {
            return _context.Emploees.Any(e => e.Id == id);
        }

        private async Task HandleProfilePictureUpload(Employee employee, IFormFile ProfilePicture, Employee? existingEmployee = null)
        {
            const long MaxFileSize = 150 * 1024; // 150 KB
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(ProfilePicture.FileName).ToLowerInvariant();

            if (ProfilePicture.Length > MaxFileSize)
            {
                ModelState.AddModelError("ProfilePicture", "Profile picture must be less than 150 KB.");
                return;
            }

            if (!permittedExtensions.Contains(ext))
            {
                ModelState.AddModelError("ProfilePicture", "Only JPG or PNG images are allowed.");
                return;
            }

            // Safe folder name
            string safeFolderName = string.Join("_", employee.FirstName.Split(Path.GetInvalidFileNameChars()))
                                    .Replace(" ", "_").Trim('_');

            string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "profile", safeFolderName);
            Directory.CreateDirectory(uploadFolder);

            // Delete old picture if exists
            if (existingEmployee != null && !string.IsNullOrEmpty(existingEmployee.ProfilePicture))
            {
                string oldFilePath = Path.Combine(_environment.WebRootPath,
                    existingEmployee.ProfilePicture.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            // Save new picture
            string uniqueFileName = "profile_" + Guid.NewGuid() + ext;
            string filePath = Path.Combine(uploadFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await ProfilePicture.CopyToAsync(stream);

            employee.ProfilePicture = $"/uploads/profile/{safeFolderName}/{uniqueFileName}";
            var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            employee.TempProfilePicture = $"data:image/{ext.TrimStart('.')};base64,{Convert.ToBase64String(imageBytes)}";
        }

        private async Task HandleDocumentUploads(Employee employee, List<IFormFile> Documents)
        {
            string safeEmployeeName = string.Join("_", employee.FirstName.Split(Path.GetInvalidFileNameChars()))
                                      .Replace(" ", "_").Trim('_');

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "documents", safeEmployeeName);
            Directory.CreateDirectory(uploadsFolder);

            var docPaths = new List<string>();
            foreach (var doc in Documents.Take(4))
            {
                string uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(doc.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await doc.CopyToAsync(stream);

                docPaths.Add($"/uploads/documents/{safeEmployeeName}/{uniqueFileName}");
            }

            employee.Document1 = docPaths.ElementAtOrDefault(0);
            employee.Document2 = docPaths.ElementAtOrDefault(1);
            employee.Document3 = docPaths.ElementAtOrDefault(2);
            employee.Document4 = docPaths.ElementAtOrDefault(3);
        }
    }
}
