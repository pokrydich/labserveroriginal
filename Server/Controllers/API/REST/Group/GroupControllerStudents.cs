namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Mvc;

using LabServer.Server.Models;
using LabServer.Server.Models.Uni;
using LabServer.Shared.Models.Uni;
using LabServer.Shared.Models;
using LabServer.Server.Helpers;
using LabServer.Server.Hubs;
using Microsoft.EntityFrameworkCore;

public partial class GroupsController : BaseRestController<GroupModel, GroupData>
{
    [HttpGet("{id}/students")]
    public async Task<ApiRequestResult<IEnumerable<StudentData>>> GetStudents(System.Int64 id)
    {
        var getResult = await _getOneInternal(id);
        if (getResult.Item1 == RestEntityStatus.Ready && getResult.Item2 != null)
        {
            var group = getResult.Item2;
            return ApiRequestResult.Success<IEnumerable<StudentData>>(group.Students.ToData<StudentModel, StudentData>());
        }
        return _returnBadResult<IEnumerable<StudentData>>(getResult.Item1);
    }

    [HttpGet("{id}/students/sync")]
    public async Task<ApiRequestResult> SyncStudents(System.Int64 id)
    {
        var getGroupResult = await _getOneInternal(id);
        if (getGroupResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<StudentData>(getGroupResult.Item1);
        var group = getGroupResult.Item2 ?? throw new NotImplementedException("sanity check");

        HashSet<System.String> warrnings = new HashSet<string>();
        foreach (var student in group.Students)
        {
            if (student.GitLabUserId == null)
            {
                var syncResult = await student.SyncWithGitLab(makeExternal: true, skipConfirmation: true);
                if (!syncResult.Ok)
                {
                    warrnings.Add($"Couldn't sync student '{student.Id}': {syncResult.Error.Message}");
                    continue;
                }
                await DataHub.SendUpdate<StudentData, StudentModel>(student);
            }
        }
        await Storage.ApplyChangesAsync();
        return ApiRequestResult.Success(warrnings=warrnings);
    }

    [HttpGet("{id}/students/sendcredentials")]
    public async Task<ApiRequestResult> SendGitPassword(System.Int64 id)
    {
        var getGroupResult = await _getOneInternal(id);
        if (getGroupResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult(getGroupResult.Item1);
        var group = getGroupResult.Item2 ?? throw new NotImplementedException("sanity check");

        HashSet<System.String> warnings = new HashSet<string>();
        foreach (var student in group.Students.Where(s => s.GitLabUserId != null))
        {
            if (!_emailSender.Send(student.Email, "OOP GitLab credentials", $"MyLabs URL: https://my.ooplabs.ru/mylabs/{student.DashboardToken}\nUsername: {student.Username}\nPassword: {student.InitialPassword}\n"))
                warnings.Add($"couldn't sent email to student '{student.Name}' ({student.Email})");
        }
        return ApiRequestResult.Success(warnings=warnings);
    }

    [HttpPost("{id}/students")]
    public async Task<ApiRequestResult<StudentData>> CreateStudent(System.Int64 id, [FromBody] StudentData studentData)
    {
        var getGroupResult = await _getOneInternal(id);
        if (getGroupResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<StudentData>(getGroupResult.Item1);
        var group = getGroupResult.Item2 ?? throw new NotImplementedException("sanity check");

        var newStudnet = new StudentModel
        {
            Name = studentData.Name,
            Username = GitLabNameTransformer.UseranmeFromName(studentData.Name),
            Email = studentData.Email,
            GroupId = group.Id
        };

        await _studentsStore.Add(newStudnet);

        try
        {
            await _studentsStore.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiRequestResult.Failure<StudentData>("db insert failed. constrints failed");
        }
        
        await DataHub.SendUpdate<StudentData, StudentModel>(newStudnet);
        return ApiRequestResult.Success<StudentData>(newStudnet.ToData());
    }

    [HttpPost("{id}/students/import")]
    public async Task<ApiRequestResult> ImportStudents(System.Int64 id, [FromBody] TextDataModel textData)
    {
        var getGroupResult = await _getOneInternal(id);
        if (getGroupResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<StudentData>(getGroupResult.Item1);
        var group = getGroupResult.Item2 ?? throw new NotImplementedException("sanity check");

        // var studentRecords = StudentsCsvParser.Parse<StudentCsvRecord>(textData.Text);
        var studentRecords = StudentsCsvParser.Parse(textData.Text);
        HashSet<System.String> warnings = new HashSet<string>();
        foreach (var record in studentRecords)
        {
            var newStudent = new StudentModel
            {
                Name = record.Name,
                Username = GitLabNameTransformer.UseranmeFromName(record.Name),
                Email = record.Email
            };
            group.Students.Add(newStudent);

            try
            {
                await Storage.ApplyChangesAsync();
                await DataHub.SendUpdate<StudentData, StudentModel>(newStudent);
            }
            catch (DbUpdateException ex)
            {
                warnings.Add("couldn't add student into the group (it's most likely that unique constraint has failed)");
            }
        }
        return ApiRequestResult.Success(warnings);
    }

    private async Task<Tuple<RestEntityStatus, StudentModel?>> _getOneStudent(System.Int64 groupId, System.Int64 studentId)
    {
        var getGroupResult = await _getOneInternal(groupId);
        if (getGroupResult.Item1 != RestEntityStatus.Ready)
            return new Tuple<RestEntityStatus, StudentModel?>(getGroupResult.Item1, null);
        var group = getGroupResult.Item2 ?? throw new NotImplementedException("sanity check");
        var student = group.Students.SingleOrDefault(s => s.Id == studentId);
        if (student == null)
            return new Tuple<RestEntityStatus, StudentModel?>(RestEntityStatus.NotFound, null);
        return new Tuple<RestEntityStatus, StudentModel?>(RestEntityStatus.Ready, student);
    }

    [HttpGet("{id}/students/{studentId}")]
    public async Task<ApiRequestResult<StudentData>> GetOneStudent(System.Int64 id, System.Int64 studentId)
    {
        var getStudentResult = await _getOneStudent(id, studentId);
        if (getStudentResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<StudentData>(getStudentResult.Item1);
        var student = getStudentResult.Item2 ?? throw new NotImplementedException("sanity check");
        return ApiRequestResult.Success<StudentData>(student.ToData());
    }

    [HttpGet("{id}/students/{studentId}/sendcredentials")]
    public async Task<ApiRequestResult> SendCredentialsToOneStudent(System.Int64 id, System.Int64 studentId)
    {
        var getStudentResult = await _getOneStudent(id, studentId);
        if (getStudentResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult(getStudentResult.Item1);
        var student = getStudentResult.Item2 ?? throw new NotImplementedException("sanity check");

        if (!_emailSender.Send(student.Email, "OOP GitLab credentials", $"MyLabs URL: https://my.ooplabs.ru/mylabs/{student.DashboardToken}\nUsername: {student.Username}\nPassword: {student.InitialPassword}"))
            return ApiRequestResult.Failure($"couldn't sent email to student '{student.Name}' ({student.Email})");
        return ApiRequestResult.Success();
    }

    [HttpPost("{groupId}/students/{studentId}/labs")] // AKA open lab for student
    public async Task<ApiRequestResult<StudentLabData>> OpenStudentLab(System.Int64 groupId, System.Int64 studentId,
                        [FromBody] StudentLabData studentLab)
    {
        var getStudentResult = await _getOneStudent(groupId, studentId);
        if (getStudentResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<StudentLabData>(getStudentResult.Item1);
        var student = getStudentResult.Item2 ?? throw new NotImplementedException("sanity check");

        var newStudentLab = new StudentLabModel
        {
            GroupCourseLabId = studentLab.GroupCourseLabId,
            OpenedDate = DateTime.UtcNow,
            Status = StudentLabStatus.cInProgress
        };

        student.Labs.Add(newStudentLab);
        try
        {
            await Storage.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new NotImplementedException("db update failed. constraints failed");
        }

        await DataHub.SendUpdate<StudentLabData, StudentLabModel>(newStudentLab);
        return ApiRequestResult.Success<StudentLabData>(newStudentLab.ToData());
    }

    [HttpPut("{id}/students/{studentId}")]
    public async Task<ApiRequestResult<StudentData>> UpdateStudent(System.Int64 id, System.Int64 studentId, [FromBody] StudentData studentData)
    {
        var getStudentResult = await _getOneStudent(id, studentId);
        if (getStudentResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<StudentData>(getStudentResult.Item1);
        var student = getStudentResult.Item2 ?? throw new NotImplementedException("sanity check");

        student.Update(studentData);
        try
        {
            await _studentsStore.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiRequestResult.Failure<StudentData>("db error");
        }

        await DataHub.SendUpdate<StudentData, StudentModel>(student);
        return ApiRequestResult.Success<StudentData>(student.ToData());
    }
}