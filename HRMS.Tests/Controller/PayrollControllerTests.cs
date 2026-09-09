using HRMS.Controllers;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using HRMS.ViewModels.Payroll;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HRMS.Tests.Controller
{
    public class PayrollControllerTests
    {
        // =========================================================
        // INDEX - RETURNS VIEW
        // =========================================================

        [Fact]
        public async Task Index_ReturnsView()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            var payrolls =
                new List<Payroll>();

            payrollRepository
                .Setup(x => x.GetAllPayrollsAsync())
                .ReturnsAsync(payrolls);

            mapper
                .Setup(x => x.Map<IEnumerable<PayrollViewModel>>(payrolls))
                .Returns(new List<PayrollViewModel>());

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.Index();

            // Assert

            Assert.IsType<ViewResult>(result);
        }


        // =========================================================
        // CREATE - GET
        // =========================================================

        [Fact]
        public async Task Create_Get_ReturnsView()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            var employees =
                new List<Employee>();

            employeeRepository
                .Setup(x => x.GetEmployeesAsync())
                .ReturnsAsync(employees);

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.Create();

            // Assert

            Assert.IsType<ViewResult>(result);

            employeeRepository.Verify(
                x => x.GetEmployeesAsync(),
                Times.Once);
        }


        // =========================================================
        // CREATE - POST - INVALID MODEL
        // =========================================================

        [Fact]
        public async Task Create_InvalidModel_ReturnsView()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            var employees =
                new List<Employee>();

            employeeRepository
                .Setup(x => x.GetEmployeesAsync())
                .ReturnsAsync(employees);

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            controller.ModelState.AddModelError(
                "EmployeeId",
                "Employee is required");

            var model =
                new PayrollViewModel
                {
                    EmployeeId = 1
                };

            // Act

            var result =
                await controller.Create(model);

            // Assert

            Assert.IsType<ViewResult>(result);

            payrollRepository.Verify(
                x => x.AddPayrollAsync(It.IsAny<Payroll>()),
                Times.Never);
        }


        // =========================================================
        // CREATE - POST - VALID MODEL
        // =========================================================

        [Fact]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            var model =
                new PayrollViewModel
                {
                    EmployeeId = 1,
                    BasicSalary = 50000,
                    Allowance = 5000,
                    Deduction = 2000
                };

            var payroll =
                new Payroll
                {
                    EmployeeId = 1,
                    BasicSalary = 50000,
                    Allowance = 5000,
                    Deduction = 2000
                };

            mapper
                .Setup(x => x.Map<Payroll>(model))
                .Returns(payroll);

            payrollRepository
                .Setup(x => x.AddPayrollAsync(It.IsAny<Payroll>()))
                .Returns(Task.CompletedTask);

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.Create(model);

            // Assert

            var redirectResult =
                Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectResult.ActionName);

            Assert.Equal(
                53000,
                payroll.NetSalary);

            payrollRepository.Verify(
                x => x.AddPayrollAsync(payroll),
                Times.Once);
        }


        // =========================================================
        // DETAILS - PAYROLL NOT FOUND
        // =========================================================

        [Fact]
        public async Task Details_PayrollNotFound_ReturnsNotFound()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            payrollRepository
                .Setup(x => x.GetPayrollByIdAsync(1))
                .ReturnsAsync((Payroll)null);

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.Details(1);

            // Assert

            Assert.IsType<NotFoundResult>(result);
        }


        // =========================================================
        // DETAILS - PAYROLL FOUND
        // =========================================================

        [Fact]
        public async Task Details_PayrollFound_ReturnsView()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            var payroll =
                new Payroll();

            payrollRepository
                .Setup(x => x.GetPayrollByIdAsync(1))
                .ReturnsAsync(payroll);

            mapper
                .Setup(x => x.Map<PayrollViewModel>(payroll))
                .Returns(new PayrollViewModel());

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.Details(1);

            // Assert

            Assert.IsType<ViewResult>(result);
        }


        // =========================================================
        // EDIT - GET - PAYROLL NOT FOUND
        // =========================================================

        [Fact]
        public async Task Edit_Get_PayrollNotFound_ReturnsNotFound()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            payrollRepository
                .Setup(x => x.GetPayrollByIdAsync(1))
                .ReturnsAsync((Payroll)null);

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.Edit(1);

            // Assert

            Assert.IsType<NotFoundResult>(result);
        }


        // =========================================================
        // EDIT - GET - PAYROLL FOUND
        // =========================================================

        [Fact]
        public async Task Edit_Get_PayrollFound_ReturnsView()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            var payroll =
                new Payroll
                {
                    EmployeeId = 1
                };

            var viewModel =
                new PayrollViewModel
                {
                    EmployeeId = 1
                };

            payrollRepository
                .Setup(x => x.GetPayrollByIdAsync(1))
                .ReturnsAsync(payroll);

            mapper
                .Setup(x => x.Map<PayrollViewModel>(payroll))
                .Returns(viewModel);

            employeeRepository
                .Setup(x => x.GetEmployeesAsync())
                .ReturnsAsync(new List<Employee>());

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.Edit(1);

            // Assert

            Assert.IsType<ViewResult>(result);

            employeeRepository.Verify(
                x => x.GetEmployeesAsync(),
                Times.Once);
        }


        // =========================================================
        // EDIT - POST - ID DOES NOT MATCH
        // =========================================================

        [Fact]
        public async Task Edit_IdDoesNotMatch_ReturnsBadRequest()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            var model =
                new PayrollViewModel
                {
                    PayrollId = 2
                };

            // Act

            var result =
                await controller.Edit(1, model);

            // Assert

            Assert.IsType<BadRequestResult>(result);
        }


        // =========================================================
        // EDIT - POST - INVALID MODEL
        // =========================================================

        [Fact]
        public async Task Edit_InvalidModel_ReturnsView()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            controller.ModelState.AddModelError(
                "BasicSalary",
                "Basic salary is required");

            var model =
                new PayrollViewModel
                {
                    PayrollId = 1,
                    EmployeeId = 1
                };

            // Act

            var result =
                await controller.Edit(1, model);

            // Assert

            Assert.IsType<ViewResult>(result);

            payrollRepository.Verify(
                x => x.UpdatePayrollAsync(It.IsAny<Payroll>()),
                Times.Never);
        }


        // =========================================================
        // EDIT - POST - VALID MODEL
        // =========================================================

        [Fact]
        public async Task Edit_ValidModel_RedirectsToIndex()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            var model =
                new PayrollViewModel
                {
                    PayrollId = 1,
                    EmployeeId = 1,
                    BasicSalary = 50000,
                    Allowance = 5000,
                    Deduction = 2000
                };

            var payroll =
                new Payroll
                {
                    PayrollId = 1,
                    EmployeeId = 1,
                    BasicSalary = 50000,
                    Allowance = 5000,
                    Deduction = 2000
                };

            mapper
                .Setup(x => x.Map<Payroll>(model))
                .Returns(payroll);

            payrollRepository
                .Setup(x => x.UpdatePayrollAsync(It.IsAny<Payroll>()))
                .Returns(Task.CompletedTask);

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.Edit(1, model);

            // Assert

            var redirectResult =
                Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectResult.ActionName);

            Assert.Equal(
                53000,
                payroll.NetSalary);

            payrollRepository.Verify(
                x => x.UpdatePayrollAsync(payroll),
                Times.Once);
        }


        // =========================================================
        // DELETE - GET - PAYROLL NOT FOUND
        // =========================================================

        [Fact]
        public async Task Delete_PayrollNotFound_ReturnsNotFound()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            payrollRepository
                .Setup(x => x.GetPayrollByIdAsync(1))
                .ReturnsAsync((Payroll)null);

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.Delete(1);

            // Assert

            Assert.IsType<NotFoundResult>(result);
        }


        // =========================================================
        // DELETE - GET - PAYROLL FOUND
        // =========================================================

        [Fact]
        public async Task Delete_PayrollFound_ReturnsView()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            var payroll =
                new Payroll();

            payrollRepository
                .Setup(x => x.GetPayrollByIdAsync(1))
                .ReturnsAsync(payroll);

            mapper
                .Setup(x => x.Map<PayrollViewModel>(payroll))
                .Returns(new PayrollViewModel());

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.Delete(1);

            // Assert

            Assert.IsType<ViewResult>(result);
        }


        // =========================================================
        // DELETE - POST
        // =========================================================

        [Fact]
        public async Task DeleteConfirmed_RedirectsToIndex()
        {
            // Arrange

            var payrollRepository =
                new Mock<IPayrollRepository>();

            var employeeRepository =
                new Mock<IEmployeeRepository>();

            var mapper =
                new Mock<AutoMapper.IMapper>();

            payrollRepository
                .Setup(x => x.DeletePayrollAsync(1))
                .Returns(Task.CompletedTask);

            var controller =
                new PayrollController(
                    payrollRepository.Object,
                    employeeRepository.Object,
                    mapper.Object);

            // Act

            var result =
                await controller.DeleteConfirmed(1);

            // Assert

            var redirectResult =
                Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectResult.ActionName);

            payrollRepository.Verify(
                x => x.DeletePayrollAsync(1),
                Times.Once);
        }
    }
}
