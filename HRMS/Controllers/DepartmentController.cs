

using AutoMapper;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using HRMS.ViewModels.Department;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public DepartmentController(
            IDepartmentRepository departmentRepository,
            IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            var departments =
                await _departmentRepository.GetAllDepartmentsAsync();

            var departmentViewModels =
                _mapper.Map<IEnumerable<DepartmentViewModel>>(departments);

            return View(departmentViewModels);
        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var department =
                await _departmentRepository.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            var departmentViewModel =
                _mapper.Map<DepartmentViewModel>(department);

            return View(departmentViewModel);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            DepartmentViewModel departmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(departmentViewModel);
            }

            try
            {
                var department =
                    _mapper.Map<Department>(departmentViewModel);

                await _departmentRepository
                    .AddDepartmentAsync(department);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong while creating department.");

                return View(departmentViewModel);
            }
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var department =
                await _departmentRepository.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            var departmentViewModel =
                _mapper.Map<DepartmentViewModel>(department);

            return View(departmentViewModel);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            DepartmentViewModel departmentViewModel)
        {
            if (id != departmentViewModel.DepartmentId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(departmentViewModel);
            }

            try
            {
                var department =
                    _mapper.Map<Department>(departmentViewModel);

                await _departmentRepository
                    .UpdateDepartmentAsync(department);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong while updating department.");

                return View(departmentViewModel);
            }
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var department =
                await _departmentRepository.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            var departmentViewModel =
                _mapper.Map<DepartmentViewModel>(department);

            return View(departmentViewModel);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _departmentRepository
                    .DeleteDepartmentAsync(id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] =
                    "Something went wrong while deleting department.";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}


