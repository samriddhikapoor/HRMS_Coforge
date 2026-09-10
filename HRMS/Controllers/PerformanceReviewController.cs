using AutoMapper;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using HRMS.ViewModels.PerformanceReview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    [Authorize(Roles = "HR")]
    public class PerformanceReviewController : Controller
    {
        private readonly IPerformanceReviewRepository _performanceReviewRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public PerformanceReviewController(
            IPerformanceReviewRepository performanceReviewRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _performanceReviewRepository = performanceReviewRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        // GET: PerformanceReview
        public async Task<IActionResult> Index()
        {
            try
            {
                var reviews =
                    await _performanceReviewRepository
                        .GetAllPerformanceReviewsAsync();

                return View(reviews);
            }
            catch (Exception)
            {
                TempData["PerformanceMessage"] =
                    "Something went wrong while loading performance reviews.";

                return View(new List<PerformanceReview>());
            }
        }

        // GET: PerformanceReview/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Employees =
                    await _employeeRepository.GetEmployeesAsync();

                return View();
            }
            catch (Exception)
            {
                TempData["PerformanceMessage"] =
                    "Something went wrong while loading employees.";

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PerformanceReview/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            PerformanceReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees =
                    await _employeeRepository.GetEmployeesAsync();

                return View(model);
            }

            try
            {
                // Business Rule:
                // Employee must exist

                var employee =
                    await _employeeRepository
                        .GetEmployeeByIdAsync(model.EmployeeId);

                if (employee == null)
                {
                    ModelState.AddModelError(
                        "EmployeeId",
                        "Selected employee does not exist.");

                    ViewBag.Employees =
                        await _employeeRepository.GetEmployeesAsync();

                    return View(model);
                }

                var review =
                    _mapper.Map<PerformanceReview>(model);

                await _performanceReviewRepository
                    .AddPerformanceReviewAsync(review);

                TempData["PerformanceMessage"] =
                    "Performance review created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong while creating performance review.");

                ViewBag.Employees =
                    await _employeeRepository.GetEmployeesAsync();

                return View(model);
            }
        }

        // GET: PerformanceReview/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var review =
                    await _performanceReviewRepository
                        .GetPerformanceReviewByIdAsync(id);

                if (review == null)
                {
                    return NotFound();
                }

                return View(review);
            }
            catch (Exception)
            {
                TempData["PerformanceMessage"] =
                    "Something went wrong while loading performance details.";

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PerformanceReview/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var review =
                    await _performanceReviewRepository
                        .GetPerformanceReviewByIdAsync(id);

                if (review == null)
                {
                    return NotFound();
                }

                ViewBag.Employees =
                    await _employeeRepository.GetEmployeesAsync();

                var model =
                    _mapper.Map<PerformanceReviewViewModel>(review);

                return View(model);
            }
            catch (Exception)
            {
                TempData["PerformanceMessage"] =
                    "Something went wrong while loading performance review.";

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PerformanceReview/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            PerformanceReviewViewModel model)
        {
            if (id != model.PerformanceReviewId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Employees =
                    await _employeeRepository.GetEmployeesAsync();

                return View(model);
            }

            try
            {
                // Business Rule:
                // Employee must exist

                var employee =
                    await _employeeRepository
                        .GetEmployeeByIdAsync(model.EmployeeId);

                if (employee == null)
                {
                    ModelState.AddModelError(
                        "EmployeeId",
                        "Selected employee does not exist.");

                    ViewBag.Employees =
                        await _employeeRepository.GetEmployeesAsync();

                    return View(model);
                }

                var review =
                    await _performanceReviewRepository
                        .GetPerformanceReviewByIdAsync(id);

                if (review == null)
                {
                    return NotFound();
                }

                _mapper.Map(model, review);

                await _performanceReviewRepository
                    .UpdatePerformanceReviewAsync(review);

                TempData["PerformanceMessage"] =
                    "Performance review updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong while updating performance review.");

                ViewBag.Employees =
                    await _employeeRepository.GetEmployeesAsync();

                return View(model);
            }
        }

        // GET: PerformanceReview/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var review =
                    await _performanceReviewRepository
                        .GetPerformanceReviewByIdAsync(id);

                if (review == null)
                {
                    return NotFound();
                }

                return View(review);
            }
            catch (Exception)
            {
                TempData["PerformanceMessage"] =
                    "Something went wrong while loading performance review.";

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PerformanceReview/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var review =
                    await _performanceReviewRepository
                        .GetPerformanceReviewByIdAsync(id);

                if (review == null)
                {
                    return NotFound();
                }

                await _performanceReviewRepository
                    .DeletePerformanceReviewAsync(id);

                TempData["PerformanceMessage"] =
                    "Performance review deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["PerformanceMessage"] =
                    "Something went wrong while deleting performance review.";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}