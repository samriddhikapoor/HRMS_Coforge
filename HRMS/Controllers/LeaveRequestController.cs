

using HRMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    [Authorize(Roles = "HR,Admin,Manager")]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public LeaveRequestController(
            ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        // GET: LeaveRequest
        public async Task<IActionResult> Index()
        {
            try
            {
                var leaveRequests =
                    await _leaveRequestRepository
                        .GetAllLeaveRequestsAsync();

                return View(leaveRequests);
            }
            catch (Exception)
            {
                TempData["LeaveMessage"] =
                    "Something went wrong while loading leave requests.";

                return View(new List<HRMS.Models.LeaveRequest>());
            }
        }


        // GET: LeaveRequest/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var leaveRequest =
                    await _leaveRequestRepository
                        .GetLeaveRequestByIdAsync(id);

                if (leaveRequest == null)
                {
                    return NotFound();
                }

                return View(leaveRequest);
            }
            catch (Exception)
            {
                TempData["LeaveMessage"] =
                    "Something went wrong while loading leave details.";

                return RedirectToAction(nameof(Index));
            }
        }


        // POST: LeaveRequest/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var leaveRequest =
                    await _leaveRequestRepository
                        .GetLeaveRequestByIdAsync(id);

                // Business Rule:
                // Leave request must exist
                if (leaveRequest == null)
                {
                    return NotFound();
                }

                // Business Rule:
                // Only Pending requests can be approved
                if (leaveRequest.Status != "Pending")
                {
                    TempData["LeaveMessage"] =
                        $"This leave request is already {leaveRequest.Status.ToLower()}.";

                    return RedirectToAction(nameof(Index));
                }

                leaveRequest.Status = "Approved";

                await _leaveRequestRepository
                    .UpdateLeaveRequestAsync(leaveRequest);

                TempData["LeaveMessage"] =
                    "Leave request approved successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["LeaveMessage"] =
                    "Something went wrong while approving the leave request.";

                return RedirectToAction(nameof(Index));
            }
        }


        // POST: LeaveRequest/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var leaveRequest =
                    await _leaveRequestRepository
                        .GetLeaveRequestByIdAsync(id);

                // Business Rule:
                // Leave request must exist
                if (leaveRequest == null)
                {
                    return NotFound();
                }

                // Business Rule:
                // Only Pending requests can be rejected
                if (leaveRequest.Status != "Pending")
                {
                    TempData["LeaveMessage"] =
                        $"This leave request is already {leaveRequest.Status.ToLower()}.";

                    return RedirectToAction(nameof(Index));
                }

                leaveRequest.Status = "Rejected";

                await _leaveRequestRepository
                    .UpdateLeaveRequestAsync(leaveRequest);

                TempData["LeaveMessage"] =
                    "Leave request rejected successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["LeaveMessage"] =
                    "Something went wrong while rejecting the leave request.";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}