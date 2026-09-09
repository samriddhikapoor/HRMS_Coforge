using HRMS.Controllers;
using HRMS.Data;
using HRMS.Repositories.Interfaces;
using HRMS.ViewModels.Employee;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HRMS.Tests.Controller
{
    public class EmployeeControllerTests
    {
        // =========================================================
        // DETAILS - EMPLOYEE NOT FOUND
        // =========================================================

        [Fact]
        public async Task Details_EmployeeNotFound_ReturnsNotFound()
        {
            // Arrange

            var employeeRepository = new Mock<IEmployeeRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var mapper = new Mock<AutoMapper.IMapper>();

            employeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(1))
                .ReturnsAsync((HRMS.Models.Employee)null);

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var controller = new EmployeeController(
                employeeRepository.Object,
                departmentRepository.Object,
                mapper.Object,
                userManager.Object);

            // Act

            var result = await controller.Details(1);

            // Assert

            Assert.IsType<NotFoundResult>(result);
        }


        // =========================================================
        // DETAILS - EMPLOYEE FOUND
        // =========================================================

        [Fact]
        public async Task Details_EmployeeFound_ReturnsView()
        {
            // Arrange

            var employeeRepository = new Mock<IEmployeeRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var mapper = new Mock<AutoMapper.IMapper>();

            var employee = new HRMS.Models.Employee();

            employeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(1))
                .ReturnsAsync(employee);

            mapper
                .Setup(x => x.Map<EmployeeViewModel>(employee))
                .Returns(new EmployeeViewModel());

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var controller = new EmployeeController(
                employeeRepository.Object,
                departmentRepository.Object,
                mapper.Object,
                userManager.Object);

            // Act

            var result = await controller.Details(1);

            // Assert

            Assert.IsType<ViewResult>(result);
        }


        // =========================================================
        // INDEX - RETURNS VIEW
        // =========================================================

        [Fact]
        public async Task Index_ReturnsView()
        {
            // Arrange

            var employeeRepository = new Mock<IEmployeeRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var mapper = new Mock<AutoMapper.IMapper>();

            var employees = new List<HRMS.Models.Employee>();

            employeeRepository
                .Setup(x => x.GetEmployeesAsync())
                .ReturnsAsync(employees);

            mapper
                .Setup(x => x.Map<IEnumerable<EmployeeViewModel>>(employees))
                .Returns(new List<EmployeeViewModel>());

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var controller = new EmployeeController(
                employeeRepository.Object,
                departmentRepository.Object,
                mapper.Object,
                userManager.Object);

            // Act

            var result = await controller.Index();

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

            var employeeRepository = new Mock<IEmployeeRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var mapper = new Mock<AutoMapper.IMapper>();

            var departments = new List<HRMS.Models.Department>();

            departmentRepository
                .Setup(x => x.GetAllDepartmentsAsync())
                .ReturnsAsync(departments);

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var controller = new EmployeeController(
                employeeRepository.Object,
                departmentRepository.Object,
                mapper.Object,
                userManager.Object);

            // Act

            var result = await controller.Create();

            // Assert

            Assert.IsType<ViewResult>(result);

            departmentRepository.Verify(
                x => x.GetAllDepartmentsAsync(),
                Times.Once);
        }


        // =========================================================
        // EDIT - EMPLOYEE NOT FOUND
        // =========================================================

        [Fact]
        public async Task Edit_EmployeeNotFound_ReturnsNotFound()
        {
            // Arrange

            var employeeRepository = new Mock<IEmployeeRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var mapper = new Mock<AutoMapper.IMapper>();

            employeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(1))
                .ReturnsAsync((HRMS.Models.Employee)null);

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var controller = new EmployeeController(
                employeeRepository.Object,
                departmentRepository.Object,
                mapper.Object,
                userManager.Object);

            // Act

            var result = await controller.Edit(1);

            // Assert

            Assert.IsType<NotFoundResult>(result);
        }


        // =========================================================
        // EDIT - EMPLOYEE FOUND
        // =========================================================

        [Fact]
        public async Task Edit_EmployeeFound_ReturnsView()
        {
            // Arrange

            var employeeRepository = new Mock<IEmployeeRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var mapper = new Mock<AutoMapper.IMapper>();

            var employee = new HRMS.Models.Employee
            {
                EmployeeId = 1,
                DepartmentId = 1
            };

            employeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(1))
                .ReturnsAsync(employee);

            mapper
                .Setup(x => x.Map<EmployeeViewModel>(employee))
                .Returns(new EmployeeViewModel());

            var departments = new List<HRMS.Models.Department>();

            departmentRepository
                .Setup(x => x.GetAllDepartmentsAsync())
                .ReturnsAsync(departments);

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var controller = new EmployeeController(
                employeeRepository.Object,
                departmentRepository.Object,
                mapper.Object,
                userManager.Object);

            // Act

            var result = await controller.Edit(1);

            // Assert

            Assert.IsType<ViewResult>(result);
        }


        // =========================================================
        // DELETE - EMPLOYEE NOT FOUND
        // =========================================================

        [Fact]
        public async Task Delete_EmployeeNotFound_ReturnsNotFound()
        {
            // Arrange

            var employeeRepository = new Mock<IEmployeeRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var mapper = new Mock<AutoMapper.IMapper>();

            employeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(1))
                .ReturnsAsync((HRMS.Models.Employee)null);

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var controller = new EmployeeController(
                employeeRepository.Object,
                departmentRepository.Object,
                mapper.Object,
                userManager.Object);

            // Act

            var result = await controller.Delete(1);

            // Assert

            Assert.IsType<NotFoundResult>(result);
        }


        // =========================================================
        // DELETE - EMPLOYEE FOUND
        // =========================================================

        [Fact]
        public async Task Delete_EmployeeFound_ReturnsView()
        {
            // Arrange

            var employeeRepository = new Mock<IEmployeeRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var mapper = new Mock<AutoMapper.IMapper>();

            var employee = new HRMS.Models.Employee();

            employeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(1))
                .ReturnsAsync(employee);

            mapper
                .Setup(x => x.Map<EmployeeViewModel>(employee))
                .Returns(new EmployeeViewModel());

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var controller = new EmployeeController(
                employeeRepository.Object,
                departmentRepository.Object,
                mapper.Object,
                userManager.Object);

            // Act

            var result = await controller.Delete(1);

            // Assert

            Assert.IsType<ViewResult>(result);
        }
    }
}

