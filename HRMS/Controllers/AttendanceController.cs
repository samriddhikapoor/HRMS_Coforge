

using HRMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    [Authorize(Roles = "HR")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceController(
            IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        // GET: Attendance
        public async Task<IActionResult> Index()
        {
            try
            {
                var attendance =
                    await _attendanceRepository
                        .GetAllAttendancesAsync();

                return View(attendance);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] =
                    "Something went wrong while loading attendance.";

                return View(new List<HRMS.Models.Attendance>());
            }
        }
    }
}