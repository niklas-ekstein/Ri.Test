namespace Ri.Interview.Validations;

public class ValidationError
{
    public string Name { get; set; }
    public List<ValidationErrorDetail> Details { get; set; }
}