namespace Toolbox.Pages.Models;
public class QuestionAndAnswer
{
    public string Question { get; set; }
    public string Answer { get; set; }

    public QuestionAndAnswer(string question, string answer)
    {
        Question = question;
        Answer = answer;
    }
}