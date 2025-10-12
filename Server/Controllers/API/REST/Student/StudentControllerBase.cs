namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;

using LabServer.Server.Hubs;
using LabServer.Server.Models;
using LabServer.Server.Models.Uni;
using LabServer.Server.Service;

using LabServer.Shared.Models.Uni;
using LabServer.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

public class StudentsController : BaseRestController<StudentModel, StudentData>
{
    public StudentsController(IDBStorage<StudentModel> storage, UserManager<UserModel> userManager, IHubContext<DataHub> dataHub) : base(storage, userManager, dataHub)
    {
    }

    [HttpGet("{studentId}/labs/{labId}")]
    public async Task<ApiRequestResult<StudentLabData>> GetStudentLab(System.Int64 studentId, System.Int64 labId, [FromQuery] DataConversionOption include = DataConversionOption.Default)
    {
        var getStudentResult = await _getOneInternal(studentId);
        if (getStudentResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<StudentLabData>(getStudentResult.Item1);
        var student = getStudentResult.Item2 ?? throw new NotImplementedException("sanity check");

        var studentLab = student.Labs.SingleOrDefault(sl => sl.Id == labId);
        if (studentLab == null)
            return ApiRequestResult.Failure<StudentLabData>("not found");

        return ApiRequestResult.Success<StudentLabData>(studentLab.ToData(include));
    }

    [HttpPut("{studentId}/labs/{labId}")]
    public async Task<ApiRequestResult<StudentLabData>> UpdateStudentLab(System.Int64 studentId, System.Int64 labId, 
            [FromBody] StudentLabData studentLabData)
    {
        var getStudentResult = await _getOneInternal(studentId);
        if (getStudentResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<StudentLabData>(getStudentResult.Item1);
        var student = getStudentResult.Item2 ?? throw new NotImplementedException("sanity check");

        var studentLab = student.Labs.SingleOrDefault(sl => sl.Id == labId);
        if (studentLab == null)
            return ApiRequestResult.Failure<StudentLabData>("not found");

        studentLab.Status = studentLabData.Status;
        studentLab.Notes = studentLabData.Notes;

        try
        {
            await Storage.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiRequestResult.Failure<StudentLabData>("db update error. constraints failed");
        }

        return ApiRequestResult.Success<StudentLabData>(studentLab.ToData());
    }
}