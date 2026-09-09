using HRMS.Controllers;
using HRMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HRMS.Tests.Controller
{
    public class AttendanceControllerTests
    {
        // =========================================================
        // INDEX - RETURNS VIEW
        // =========================================================

        [Fact]
        public async Task Index_ReturnsView()
        {
            // Arrange

            var attendanceRepository =
                new Mock<IAttendanceRepository>();

            var attendance =
                new List<HRMS.Models.Attendance>();

            attendanceRepository
                .Setup(x => x.GetAllAttendancesAsync())
                .ReturnsAsync(attendance);

            var controller =
                new AttendanceController(
                    attendanceRepository.Object);

            // Act

            var result =
                await controller.Index();

            // Assert

            Assert.IsType<ViewResult>(result);
        }
    }
}

