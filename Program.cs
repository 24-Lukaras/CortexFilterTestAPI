using CortexFilterTestAPI.Ai;
using CortexFilterTestAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection(nameof(OpenAiSettings)));
builder.Services.AddScoped<AzureChatClientProvider>();
builder.Services.AddScoped<EmployeesRepository>();
builder.Services.AddScoped<AbsencesRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var absenceEndpoints = app.MapGroup("/absences")
    .WithTags("Absences")
    .WithOpenApi();
absenceEndpoints.MapGet("/all", async (AbsencesRepository repository, EmployeesRepository employeesRepository) =>
    {
        var absences = await repository.GetAllAsync();
        var employees = await employeesRepository.GetAllAsync();
        var employeesDictionary = employees.ToDictionary(x => x.Id);
        var response = absences.Select(x => AbsenceItem.FromAbsence(x, employeesDictionary)).ToArray();
        return response;
    })
    .WithName("GetAllAbsences")
    .WithOpenApi();
absenceEndpoints.MapGet("/filter", async (string query) =>
    {
        return "ok";
    })
    .WithName("FilterAbsences")
    .WithOpenApi();

var employeesEndpoints = app.MapGroup("/employees")
    .WithTags("Employees")
    .WithOpenApi();
employeesEndpoints.MapGet("/all", async (EmployeesRepository repository) =>
    {
        var employees = await repository.GetAllAsync();
        var response = employees.Select(x => EmployeeItem.FromEmployee(x)).ToArray();
        return response;
    })
    .WithName("GetAllEmployees")
    .WithOpenApi();
employeesEndpoints.MapGet("/filter", async (string query) =>
    {
        return "ok";
    })
    .WithName("FilterEmployees")
    .WithOpenApi();

app.Run();

record AbsenceItem(int Id,
    DateOnly Date,
    string? Employee,
    string Type)
{
    public static AbsenceItem FromAbsence(Absence absence, IReadOnlyDictionary<int, Employee> employees)
    {
        var employee = employees.GetValueOrDefault(absence.EmployeeId);
        return new AbsenceItem(
            absence.Id,
            DateOnly.FromDateTime(absence.Date),
            employee is null ? null : $"{employee.Firstname} {employee.Lastname}",
            absence.Type switch
            {
                AbsenceType.BusinessJourney => "pracovní cesta",
                AbsenceType.HomeOffice => "home office",
                AbsenceType.ParentalLeave => "rodièovská dovolená",
                AbsenceType.SickDay => "sick day",
                AbsenceType.Vacation => "dovolená",
                _ => "nespecifikováno"
            }
        );
    }
}
record EmployeeItem(int Id,
    string Name,
    string Status,
    DateOnly DateOfBirth,
    int Age)
{
    public static EmployeeItem FromEmployee(Employee employee) =>
        new EmployeeItem(
            employee.Id,
            $"{employee.Firstname} {employee.Lastname}",
            employee.Status switch {
                EmployeeStatus.Draft => "potenciální",
                EmployeeStatus.Active => "aktivní",
                EmployeeStatus.OnVacation => "nedostupný",
                EmployeeStatus.Inactive => "neaktivní",
                _ => "neznámý"
            },
            DateOnly.FromDateTime(employee.DateOfBirth),
            DateTime.Today.Year - employee.DateOfBirth.Year -
                ((DateTime.Today.Month < employee.DateOfBirth.Month || (DateTime.Today.Month == employee.DateOfBirth.Month && DateTime.Today.Day <= employee.DateOfBirth.Day))
                    ? 1 : 0)
        );
}