using Bogus;

namespace CortexFilterTestAPI.Data;

public class AbsencesRepository
{
    private static IReadOnlyList<Absence>? _absences;
    private readonly EmployeesRepository _employeesRepository;
    public AbsencesRepository(EmployeesRepository employeesRepository)
    {
        _employeesRepository = employeesRepository;
    }

    public async Task<IEnumerable<Absence>> GetAllAsync()
    {
        if (_absences is null)
        {
            var employees = await _employeesRepository.GetAllAsync();
            _absences = CreateMockAbsences(employees).OrderByDescending(x => x.Date).ToList();
        }
        return _absences;
    }

    private List<Absence> CreateMockAbsences(IEnumerable<Employee> employees)
    {
        var employeeIds = employees.Select(x => x.Id).ToArray();
        var minDate = new DateTime(DateTime.Now.Year, 1, 1);
        var maxDate = DateTime.Today.AddDays(-1);
        int id = 0;
        var faker = new Faker<Absence>()
            .RuleFor(x => x.Id, f => ++id)
            .RuleFor(x => x.Date, f => f.Date.Between(minDate, maxDate).Date)
            .RuleFor(x => x.EmployeeId, f => f.PickRandom(employeeIds))
            .RuleFor(x => x.Type, f => f.PickRandom<AbsenceType>())
            .UseSeed(69)
        ;
        var result = faker.Generate(100);
        foreach (var employee in employees.Where(x => x.Status == EmployeeStatus.OnVacation))
        {
            result.Add(new Absence()
            {
                Id = ++id,
                Date = DateTime.Today,
                EmployeeId = employee.Id,
                Type = AbsenceType.Vacation
            });
        }
        return result;
    }
}

public class Absence
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public int EmployeeId { get; init; }
    public AbsenceType Type { get; init; }
}


public enum AbsenceType
{
    Vacation,
    SickDay,
    ParentalLeave,
    HomeOffice,
    BusinessJourney,
}