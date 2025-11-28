using Bogus;

namespace CortexFilterTestAPI.Data;

public class EmployeesRepository
{
    private static IReadOnlyList<Employee>? _employees;

    public Task<IEnumerable<Employee>> GetAllAsync()
    {
        if (_employees is null)
        {
            _employees = CreateMockEmployees();
        }
        return Task.FromResult<IEnumerable<Employee>>(_employees);
    }

    private List<Employee> CreateMockEmployees()
    {
        DateTime minDateOfBirth = DateTime.Today.AddYears(-18);
        DateTime maxDateOfBirth = DateTime.Today.AddYears(-60);
        int id = 0;
        var faker = new Faker<Employee>()
            .RuleFor(x => x.Id, f => ++id)
            .RuleFor(x => x.Firstname, f => f.Name.FirstName())
            .RuleFor(x => x.Lastname, f => f.Name.LastName())
            .RuleFor(x => x.Status, f => f.PickRandom<EmployeeStatus>())
            .RuleFor(x => x.DateOfBirth, f => f.Date.Between(minDateOfBirth, maxDateOfBirth).Date)
            .UseSeed(420)
        ;
        return faker.Generate(50);
    }
}

public class Employee
{
    public int Id { get; init; }
    public string Firstname { get; init; } = null!;
    public string Lastname { get; init; } = null!;
    public EmployeeStatus Status { get; init; }
    public DateTime DateOfBirth { get; init; }
}

public enum EmployeeStatus
{
    Unknown,
    Draft,
    Active,
    OnVacation,
    Inactive,
}
