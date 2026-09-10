namespace HRMS.Services.Interfaces
{
    public interface IAIChatService
    {
        string GetAnswer(string category, string question);
    }
}