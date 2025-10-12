namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using LabServer.Server.Models;
using LabServer.Server.Models.Uni;
using LabServer.Server.Hubs;

using LabServer.Shared.Models.Uni;
using LabServer.Shared.Models;

public partial class GroupsController : BaseRestController<GroupModel, GroupData>
{
    [HttpGet("{id}/courses")]
    public async Task<ApiRequestResult<IEnumerable<GroupCourseData>>> GetCourses(System.Int64 id, [FromQuery] DataConversionOption include = DataConversionOption.Default)
    {
        var getResult = await _getOneInternal(id);
        if (getResult.Item1 == RestEntityStatus.Ready && getResult.Item2 != null)
        {
            var group = getResult.Item2;
            return ApiRequestResult.Success<IEnumerable<GroupCourseData>>(group.CoursesMapping.ToData<GroupCourseMapping, GroupCourseData>(include));
        }
        return _returnBadResult<IEnumerable<GroupCourseData>>(getResult.Item1);
    }

    private async Task<Tuple<RestEntityStatus, GroupCourseMapping?>> _getOneGroupCourse(System.Int64 groupId, System.Int64 courseId)
    {
        var getGroupResult = await _getOneInternal(groupId);
        if (getGroupResult.Item1 != RestEntityStatus.Ready)
            return new Tuple<RestEntityStatus, GroupCourseMapping?>(getGroupResult.Item1, null);
        var group = getGroupResult.Item2 ?? throw new NotImplementedException("sanity check");

        var groupCourse = group.CoursesMapping.SingleOrDefault(gcm => gcm.CourseId == courseId);
        return new Tuple<RestEntityStatus, GroupCourseMapping?>(groupCourse != null ? RestEntityStatus.Ready : RestEntityStatus.NotFound, groupCourse);
    }

    [HttpGet("{groupId}/courses/{courseId}")]
    public async Task<ApiRequestResult<GroupCourseData>> GetOneGroupCourse(System.Int64 groupId, System.Int64 courseId,
            [FromQuery] DataConversionOption include = DataConversionOption.Default)
    {
        var getGroupCourseResult = await _getOneGroupCourse(groupId, courseId);
        if (getGroupCourseResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<GroupCourseData>(getGroupCourseResult.Item1);
        var groupCourse = getGroupCourseResult.Item2 ?? throw new NotImplementedException("sanitycheck");

        return ApiRequestResult.Success<GroupCourseData>(groupCourse.ToData(include));
    }

    [HttpGet("{id}/courses/sync")]
    public async Task<ApiRequestResult> SyncCourses(System.Int64 id)
    {
        var getGroupResult = await _getOneInternal(id);
        if (getGroupResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult(getGroupResult.Item1);
        var group = getGroupResult.Item2 ?? throw new NotImplementedException("sanity check");

        HashSet<System.String> warrnings = new HashSet<string>();
        foreach (var groupCourse in group.CoursesMapping)
        {
            if (groupCourse.GitLabGroupId == null)
            {
                var syncResult = await groupCourse.SyncWithGitLab();
                if (!syncResult.Ok)
                {
                    warrnings.Add($"Couldn't sync group course '{groupCourse.Id}': {syncResult.Error.Message}");
                    continue;
                }
                await DataHub.SendUpdate<GroupCourseData, GroupCourseMapping>(groupCourse, DataConversionOption.Full);
            }
        }
        await Storage.ApplyChangesAsync();
        return ApiRequestResult.Success(warrnings);
    }

    [HttpGet("{id}/courses/{courseId}/sync")]
    public async Task<ApiRequestResult> SyncCourseAndStudentLabs(System.Int64 id, System.Int64 courseId)
    {
        var getGroupCourseResult = await _getOneGroupCourse(id, courseId);
        if (getGroupCourseResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult(getGroupCourseResult.Item1);
        var groupCourse = getGroupCourseResult.Item2 ?? throw new NotImplementedException("sanitycheck");

        HashSet<System.String> warnings = new HashSet<string>();
        foreach (var groupCourseLab in groupCourse.GroupCourseLabs)
        {
            if (groupCourseLab.GitLabGroupId == null)
            {
                var syncResult = await groupCourseLab.SyncWithGitLab();
                if (!syncResult.Ok)
                {
                    warnings.Add($"Couldn't sync group course lab '{groupCourseLab.CourseLab.Name}': {syncResult.Error.Message}");
                    continue;
                }
                await DataHub.SendUpdate<GroupCourseLabData, GroupCourseLabMapping>(groupCourseLab, DataConversionOption.Full);
            }
            foreach (var studentLab in groupCourseLab.LabsForStudents)
            {
                if (studentLab.GitLabProjectId == null)
                {
                    var syncResult = await studentLab.SyncWithGitLab();
                    if (!syncResult.Ok)
                    {
                        warnings.Add($"Couldn't sync student lab '{studentLab.Id}': {syncResult.Error.Message}");
                        continue;
                    }
                    await DataHub.SendUpdate<StudentLabData, StudentLabModel>(studentLab);
                }
            }
        }
        await Storage.ApplyChangesAsync();
        return ApiRequestResult.Success(warnings);
    }

    [HttpPost("{id}/courses")]
    public async Task<ApiRequestResult<GroupCourseData>> AddCourse(System.Int64 id, [FromBody] GroupCourseData groupCourseData)
    {
        var getGroupResult = await _getOneInternal(id);
        if (getGroupResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<GroupCourseData>(getGroupResult.Item1);
        var group = getGroupResult.Item2 ?? throw new NotImplementedException("sanity check");

        var newGroupCourse = new GroupCourseMapping
        {
            GroupId = group.Id,
            CourseId = groupCourseData.CourseId
        };
        group.CoursesMapping.Add(newGroupCourse);
        try
        {
            await Storage.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiRequestResult.Failure<GroupCourseData>("db update error. constrains failed");
        }

        await DataHub.SendUpdate<GroupCourseData, GroupCourseMapping>(newGroupCourse, DataConversionOption.Full);
        return ApiRequestResult.Success<GroupCourseData>(newGroupCourse.ToData());
    }
}