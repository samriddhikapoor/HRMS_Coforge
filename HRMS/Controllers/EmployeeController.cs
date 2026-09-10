
using AutoMapper;
using HRMS.Data;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using HRMS.ViewModels.Employee;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _userManager = userManager;
        }


        // =========================================================
        // EMPLOYEE INDEX
        // =========================================================

        public async Task<IActionResult> Index()
        {
            var employees =
                await _employeeRepository.GetEmployeesAsync();

            var employeeViewModels =
                _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);

            return View(employeeViewModels);
        }


        // =========================================================
        // EMPLOYEE DETAILS
        // =========================================================

        public async Task<IActionResult> Details(int id)
        {
            var employee =
                await _employeeRepository.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeViewModel =
                _mapper.Map<EmployeeViewModel>(employee);

            return View(employeeViewModel);
        }


        // =========================================================
        // CREATE EMPLOYEE - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsAsync();

            return View();
        }


        // =========================================================
        // CREATE EMPLOYEE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            EmployeeViewModel employeeViewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(
                    employeeViewModel.DepartmentId);

                return View(employeeViewModel);
            }

            try
            {
                // =====================================================
                // CREATE IDENTITY USER
                // =====================================================

                var user = new ApplicationUser
                {
                    UserName = employeeViewModel.Email,
                    Email = employeeViewModel.Email,
                    EmailConfirmed = true
                };

                var userResult =
                    await _userManager.CreateAsync(
                        user,
                        employeeViewModel.Password);

                if (!userResult.Succeeded)
                {
                    foreach (var error in userResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    await LoadDepartmentsAsync(
                        employeeViewModel.DepartmentId);

                    return View(employeeViewModel);
                }


                // =====================================================
                // ASSIGN EMPLOYEE ROLE
                // =====================================================

                var roleResult =
                    await _userManager.AddToRoleAsync(
                        user,
                        "Employee");

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);

                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    await LoadDepartmentsAsync(
                        employeeViewModel.DepartmentId);

                    return View(employeeViewModel);
                }


                // =====================================================
                // CREATE EMPLOYEE PROFILE
                // =====================================================

                var employee =
                    _mapper.Map<Employee>(
                        employeeViewModel);

                employee.UserId = user.Id;

                await _employeeRepository
                    .AddEmployeeAsync(employee);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong while creating employee.");

                await LoadDepartmentsAsync(
                    employeeViewModel.DepartmentId);

                return View(employeeViewModel);
            }
        }


        // =========================================================
        // EDIT EMPLOYEE - GET
        // =========================================================

        public async Task<IActionResult> Edit(int id)
        {
            var employee =
                await _employeeRepository
                    .GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeViewModel =
                _mapper.Map<EmployeeViewModel>(
                    employee);

            await LoadDepartmentsAsync(
                employee.DepartmentId);

            return View(employeeViewModel);
        }


        // =========================================================
        // EDIT EMPLOYEE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            EmployeeViewModel employeeViewModel)
        {
            if (id != employeeViewModel.EmployeeId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(
                    employeeViewModel.DepartmentId);

                return View(employeeViewModel);
            }

            try
            {
                var employee =
                    await _employeeRepository
                        .GetEmployeeByIdAsync(id);

                if (employee == null)
                {
                    return NotFound();
                }

                employee.EmployeeName =
                    employeeViewModel.EmployeeName;

                employee.Email =
                    employeeViewModel.Email;

                employee.PhoneNumber =
                    employeeViewModel.PhoneNumber;

                employee.Designation =
                    employeeViewModel.Designation;

                employee.JoiningDate =
                    employeeViewModel.JoiningDate;

                employee.DepartmentId =
                    employeeViewModel.DepartmentId;

                await _employeeRepository
                    .UpdateEmployeeAsync(employee);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong while updating employee.");

                await LoadDepartmentsAsync(
                    employeeViewModel.DepartmentId);

                return View(employeeViewModel);
            }
        }


        // =========================================================
        // DELETE EMPLOYEE - GET
        // =========================================================

        public async Task<IActionResult> Delete(int id)
        {
            var employee =
                await _employeeRepository
                    .GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeViewModel =
                _mapper.Map<EmployeeViewModel>(
                    employee);

            return View(employeeViewModel);
        }


        // =========================================================
        // DELETE EMPLOYEE - POST
        // =========================================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            try
            {
                await _employeeRepository
                    .DeleteEmployeeAsync(id);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] =
                    "Something went wrong while deleting employee.";

                return RedirectToAction("Index");
            }
        }


        // =========================================================
        // LOAD DEPARTMENTS
        // =========================================================

        private async Task LoadDepartmentsAsync(
            int? selectedDepartmentId = null)
        {
            var departments =
                await _departmentRepository
                    .GetAllDepartmentsAsync();

            ViewBag.Departments =
                new SelectList(
                    departments,
                    "DepartmentId",
                    "DepartmentName",
                    selectedDepartmentId);
        }
    }
}