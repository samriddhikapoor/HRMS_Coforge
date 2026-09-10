using AutoMapper;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using HRMS.ViewModels.Payroll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS.Controllers
{
    [Authorize(Roles = "HR")]
    public class PayrollController : Controller
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public PayrollController(
            IPayrollRepository payrollRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _payrollRepository = payrollRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        // GET: Payroll
        public async Task<IActionResult> Index()
        {
            try
            {
                var payrolls =
                    await _payrollRepository.GetAllPayrollsAsync();

                var viewModels =
                    _mapper.Map<IEnumerable<PayrollViewModel>>(payrolls);

                return View(viewModels);
            }
            catch (Exception)
            {
                TempData["PayrollMessage"] =
                    "Something went wrong while loading payrolls.";

                return View(new List<PayrollViewModel>());
            }
        }

        // GET: Payroll/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                await LoadEmployeesAsync();

                return View();
            }
            catch (Exception)
            {
                TempData["PayrollMessage"] =
                    "Something went wrong while loading employees.";

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Payroll/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            PayrollViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadEmployeesAsync(model.EmployeeId);
                return View(model);
            }

            try
            {
                // =====================================================
                // BUSINESS RULE 1:
                // EMPLOYEE MUST EXIST
                // =====================================================

                var employee =
                    await _employeeRepository
                        .GetEmployeeByIdAsync(model.EmployeeId);

                if (employee == null)
                {
                    ModelState.AddModelError(
                        "EmployeeId",
                        "Selected employee does not exist.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }


                // =====================================================
                // BUSINESS RULE 2:
                // SALARY VALUES CANNOT BE NEGATIVE
                // =====================================================

                if (model.BasicSalary < 0)
                {
                    ModelState.AddModelError(
                        "BasicSalary",
                        "Basic salary cannot be negative.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }

                if (model.Allowance < 0)
                {
                    ModelState.AddModelError(
                        "Allowance",
                        "Allowance cannot be negative.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }

                if (model.Deduction < 0)
                {
                    ModelState.AddModelError(
                        "Deduction",
                        "Deduction cannot be negative.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }


                // =====================================================
                // BUSINESS RULE 3:
                // SALARY MONTH MUST NOT BE IN THE FUTURE
                // =====================================================

                var salaryMonth =
                    new DateTime(
                        model.SalaryMonth.Year,
                        model.SalaryMonth.Month,
                        1);

                var currentMonth =
                    new DateTime(
                        DateTime.Today.Year,
                        DateTime.Today.Month,
                        1);

                if (salaryMonth > currentMonth)
                {
                    ModelState.AddModelError(
                        "SalaryMonth",
                        "Payroll cannot be created for a future month.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }


                // =====================================================
                // BUSINESS RULE 4:
                // ONE PAYROLL PER EMPLOYEE PER MONTH
                // =====================================================

                var existingPayrolls =
                    await _payrollRepository.GetAllPayrollsAsync();

                var duplicatePayroll =
                    existingPayrolls.Any(p =>
                        p.EmployeeId == model.EmployeeId &&
                        p.SalaryMonth.Year == model.SalaryMonth.Year &&
                        p.SalaryMonth.Month == model.SalaryMonth.Month);

                if (duplicatePayroll)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Payroll for this employee already exists for the selected month.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }


                // =====================================================
                // CREATE PAYROLL
                // =====================================================

                var payroll =
                    _mapper.Map<Payroll>(model);

                payroll.SalaryMonth = salaryMonth;

                // Automatically calculate Net Salary
                payroll.NetSalary =
                    payroll.BasicSalary
                    + payroll.Allowance
                    - payroll.Deduction;

                await _payrollRepository
                    .AddPayrollAsync(payroll);

                TempData["PayrollMessage"] =
                    "Payroll created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong while creating payroll.");

                await LoadEmployeesAsync(model.EmployeeId);

                return View(model);
            }
        }

        // GET: Payroll/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var payroll =
                    await _payrollRepository
                        .GetPayrollByIdAsync(id);

                if (payroll == null)
                {
                    return NotFound();
                }

                var viewModel =
                    _mapper.Map<PayrollViewModel>(payroll);

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["PayrollMessage"] =
                    "Something went wrong while loading payroll details.";

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Payroll/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var payroll =
                    await _payrollRepository
                        .GetPayrollByIdAsync(id);

                if (payroll == null)
                {
                    return NotFound();
                }

                var viewModel =
                    _mapper.Map<PayrollViewModel>(payroll);

                await LoadEmployeesAsync(viewModel.EmployeeId);

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["PayrollMessage"] =
                    "Something went wrong while loading payroll.";

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Payroll/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            PayrollViewModel model)
        {
            if (id != model.PayrollId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadEmployeesAsync(model.EmployeeId);
                return View(model);
            }

            try
            {
                // =====================================================
                // BUSINESS RULE 1:
                // EMPLOYEE MUST EXIST
                // =====================================================

                var employee =
                    await _employeeRepository
                        .GetEmployeeByIdAsync(model.EmployeeId);

                if (employee == null)
                {
                    ModelState.AddModelError(
                        "EmployeeId",
                        "Selected employee does not exist.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }


                // =====================================================
                // BUSINESS RULE 2:
                // SALARY VALUES CANNOT BE NEGATIVE
                // =====================================================

                if (model.BasicSalary < 0)
                {
                    ModelState.AddModelError(
                        "BasicSalary",
                        "Basic salary cannot be negative.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }

                if (model.Allowance < 0)
                {
                    ModelState.AddModelError(
                        "Allowance",
                        "Allowance cannot be negative.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }

                if (model.Deduction < 0)
                {
                    ModelState.AddModelError(
                        "Deduction",
                        "Deduction cannot be negative.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }


                // =====================================================
                // BUSINESS RULE 3:
                // FUTURE MONTH NOT ALLOWED
                // =====================================================

                var salaryMonth =
                    new DateTime(
                        model.SalaryMonth.Year,
                        model.SalaryMonth.Month,
                        1);

                var currentMonth =
                    new DateTime(
                        DateTime.Today.Year,
                        DateTime.Today.Month,
                        1);

                if (salaryMonth > currentMonth)
                {
                    ModelState.AddModelError(
                        "SalaryMonth",
                        "Payroll cannot be created for a future month.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }


                // =====================================================
                // BUSINESS RULE 4:
                // NO DUPLICATE PAYROLL
                // EXCEPT CURRENT PAYROLL BEING EDITED
                // =====================================================

                var existingPayrolls =
                    await _payrollRepository.GetAllPayrollsAsync();

                var duplicatePayroll =
                    existingPayrolls.Any(p =>
                        p.PayrollId != id &&
                        p.EmployeeId == model.EmployeeId &&
                        p.SalaryMonth.Year == model.SalaryMonth.Year &&
                        p.SalaryMonth.Month == model.SalaryMonth.Month);

                if (duplicatePayroll)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Another payroll already exists for this employee for the selected month.");

                    await LoadEmployeesAsync(model.EmployeeId);
                    return View(model);
                }


                // =====================================================
                // UPDATE PAYROLL
                // =====================================================

                var payroll =
                    _mapper.Map<Payroll>(model);

                payroll.SalaryMonth = salaryMonth;

                // Automatically calculate Net Salary
                payroll.NetSalary =
                    payroll.BasicSalary
                    + payroll.Allowance
                    - payroll.Deduction;

                await _payrollRepository
                    .UpdatePayrollAsync(payroll);

                TempData["PayrollMessage"] =
                    "Payroll updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong while updating payroll.");

                await LoadEmployeesAsync(model.EmployeeId);

                return View(model);
            }
        }

        // GET: Payroll/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var payroll =
                    await _payrollRepository
                        .GetPayrollByIdAsync(id);

                if (payroll == null)
                {
                    return NotFound();
                }

                var viewModel =
                    _mapper.Map<PayrollViewModel>(payroll);

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["PayrollMessage"] =
                    "Something went wrong while loading payroll.";

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Payroll/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var payroll =
                    await _payrollRepository
                        .GetPayrollByIdAsync(id);

                if (payroll == null)
                {
                    return NotFound();
                }

                await _payrollRepository
                    .DeletePayrollAsync(id);

                TempData["PayrollMessage"] =
                    "Payroll deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["PayrollMessage"] =
                    "Something went wrong while deleting payroll.";

                return RedirectToAction(nameof(Index));
            }
        }

        // Load employees for dropdown
        private async Task LoadEmployeesAsync(
            int? selectedEmployeeId = null)
        {
            var employees =
                await _employeeRepository
                    .GetEmployeesAsync();

            ViewBag.Employees =
                new SelectList(
                    employees,
                    "EmployeeId",
                    "EmployeeName",
                    selectedEmployeeId);
        }
    }
}