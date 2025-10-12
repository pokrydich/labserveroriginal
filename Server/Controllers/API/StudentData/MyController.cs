namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using LabServer.Shared.Models;
using LabServer.Server.Data;
using LabServer.Shared.Models.Uni;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class MyController : Controller
{
    private readonly LabsContext _context;

    public MyController(LabsContext context)
    {
        _context = context;
    }

    [HttpGet("{token}")]
    public async Task<ApiRequestResult<StudentData>> GetStudentData(System.String token)
    {
        var students = await _context.Students.ToListAsync();
        var student = students.SingleOrDefault(s => s.DashboardToken == token);
        if (student == null)
        {
            return ApiRequestResult.Failure<StudentData>("unauthorized");
        }
        return ApiRequestResult.Success<StudentData>(student.ToData(DataConversionOption.Children));
    }
}