namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using LabServer.Server.Models.Uni;
using LabServer.Server.Hubs;

using LabServer.Shared.Models.Uni;
using LabServer.Shared.Models;

public partial class GroupsController : BaseRestController<GroupModel, GroupData>
{
    private async Task<Tuple<RestEntityStatus, GroupCourseLabMapping?>> _getOneGroupCourseLab(System.Int64 groupId, System.Int64 courseId, System.Int64 courseLabId)
    {
        var getGroupCourseResult = await _getOneGroupCourse(groupId, courseId);
        if (getGroupCourseResult.Item1 != RestEntityStatus.Ready)
            return new Tuple<RestEntityStatus, GroupCourseLabMapping?>(getGroupCourseResult.Item1, null);
        var groupCourse = getGroupCourseResult.Item2 ?? throw new NotImplementedException("sanitycheck");

        var groupCourseLab = groupCourse.GroupCourseLabs.SingleOrDefault(gcl => gcl.CourseLabId == courseLabId);
        return new Tuple<RestEntityStatus, GroupCourseLabMapping?>(groupCourseLab != null ? RestEntityStatus.Ready : RestEntityStatus.NotFound, groupCourseLab);
    }

    [HttpGet("{groupId}/courses/{courseId}/labs/{courseLabId}")]
    public async Task<ApiRequestResult<GroupCourseLabData>> GetOneGroupCourseLab(System.Int64 groupId, System.Int64 courseId, System.Int64 courseLabId)
    {
        var getGroupCourseLabResult = await _getOneGroupCourseLab(groupId, courseId, courseLabId);
        if (getGroupCourseLabResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<GroupCourseLabData>(getGroupCourseLabResult.Item1);
        var groupCourseLab = getGroupCourseLabResult.Item2 ?? throw new NotImplementedException("sanitycheck");

        return ApiRequestResult.Success<GroupCourseLabData>(groupCourseLab.ToData());
    }

    [HttpPut("{groupId}/courses/{courseId}/labs/{courseLabId}")]
    public async Task<ApiRequestResult<GroupCourseLabData>> UpdateGroupCourseLab(System.Int64 groupId, System.Int64 courseId, System.Int64 courseLabId,
        [FromBody] GroupCourseLabData groupCourseLabData)
    {
        var getGroupCourseLabResult = await _getOneGroupCourseLab(groupId, courseId, courseLabId);
        if (getGroupCourseLabResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<GroupCourseLabData>(getGroupCourseLabResult.Item1);
        var groupCourseLab = getGroupCourseLabResult.Item2 ?? throw new NotImplementedException("sanitycheck");

        groupCourseLab.DeadlineDate = groupCourseLabData.DeadlineDate;

        try
        {
            await Storage.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiRequestResult.Failure<GroupCourseLabData>("db update error. constraints failed");
        }
        return ApiRequestResult.Success<GroupCourseLabData>(groupCourseLab.ToData());
    }

    [HttpPost("{groupId}/courses/{courseId}/labs")] // AKA open lab to course
    public async Task<ApiRequestResult<GroupCourseLabData>> OpenGroupCouseLab(System.Int64 groupId, System.Int64 courseId,
            [FromBody] GroupCourseLabData groupCourseLabData)
    {
        var getGroupCourseResult = await _getOneGroupCourse(groupId, courseId);
        if (getGroupCourseResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<GroupCourseLabData>(getGroupCourseResult.Item1);
        var groupCourse = getGroupCourseResult.Item2 ?? throw new NotImplementedException("sanitycheck");

        var newGroupCourseLab = new GroupCourseLabMapping
        {
            GroupCourseId = groupCourse.Id,
            CourseLabId = groupCourseLabData.CourseLabId,
            StartDate = DateTime.UtcNow
        };

        groupCourse.GroupCourseLabs.Add(newGroupCourseLab);
        try
        {
            await Storage.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new NotImplementedException("db update failed. constrains failed");
        }

        await DataHub.SendUpdate<GroupCourseLabData, GroupCourseLabMapping>(newGroupCourseLab, DataConversionOption.Full);
        return ApiRequestResult.Success<GroupCourseLabData>(newGroupCourseLab.ToData());
    }
}