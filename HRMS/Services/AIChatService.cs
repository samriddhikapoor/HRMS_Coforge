using HRMS.Services.Interfaces;

namespace HRMS.Services
{
    public class AIChatService : IAIChatService
    {
        public string GetAnswer(string category, string question)
        {
            if (category == "Leave")
            {
                if (question == "How can I apply for leave?")
                {
                    return "Go to the Leave section, select Apply Leave, enter the start date, end date and reason, then submit your request.";
                }

                if (question == "What is the leave approval process?")
                {
                    return "After submitting a leave request, it remains Pending until it is reviewed by the authorized HR, Manager or Admin.";
                }

                if (question == "Can I cancel an approved leave?")
                {
                    return "Please contact the authorized HR or Manager regarding cancellation of an approved leave.";
                }
            }

            if (category == "Attendance")
            {
                if (question == "How do I check in?")
                {
                    return "Open the Attendance section and click the Check In button. Your check-in time will be recorded.";
                }

                if (question == "How do I check out?")
                {
                    return "After checking in, open Attendance and click Check Out. Your check-out time will be recorded.";
                }
            }

            if (category == "Payroll")
            {
                if (question == "Where can I view my payroll?")
                {
                    return "Open the Payroll section from your employee dashboard to view your available payroll records.";
                }

                if (question == "How is net salary calculated?")
                {
                    return "Net Salary is calculated as Basic Salary + Allowance - Deduction.";
                }
            }

            if (category == "Performance")
            {
                if (question == "Where can I see my performance review?")
                {
                    return "Open the Performance section from your employee dashboard to view your performance reviews.";
                }
            }

            return "Please select a valid HRMS question.";
        }
    }
}