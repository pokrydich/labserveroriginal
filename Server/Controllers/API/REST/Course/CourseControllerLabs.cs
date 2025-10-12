namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using LabServer.Server.Models.Uni;
using LabServer.Server.Models;
using LabServer.Server.Hubs;

using LabServer.Shared.Models;
using LabServer.Shared.Models.Uni;
using LabServer.Server.Helpers;

public partial class CoursesController : BaseRestController<CourseModel, CourseData>
{
    [HttpGet("{courseId}/labs")]
    public async Task<ApiRequestResult<IEnumerable<CourseLabData>>> GetCourseLabs(System.Int64 courseId)
    {
        var oneResult = await _getOneInternal(courseId);
        if (oneResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<IEnumerable<CourseLabData>>(oneResult.Item1);
        var course = oneResult.Item2 ?? throw new NotImplementedException("sanity check");

        return ApiRequestResult.Success<IEnumerable<CourseLabData>>(course.CourseLabs.ToData<CourseLabModel, CourseLabData>());
    }

    private async Task<Tuple<RestEntityStatus, CourseLabModel?>> _getOneCourseLab(System.Int64 courseId, System.Int64 courseLabId)
    {
        var courseResult = await _getOneInternal(courseId);
        if (courseResult.Item1 != RestEntityStatus.Ready)
            return new Tuple<RestEntityStatus, CourseLabModel?>(courseResult.Item1, null);
        var course = courseResult.Item2 ?? throw new NotImplementedException("sanity check");

        var courseLab = course.CourseLabs.SingleOrDefault(cl => cl.Id == courseLabId);
        return new Tuple<RestEntityStatus, CourseLabModel?>(courseLab != null 
                                                ? RestEntityStatus.Ready 
                                                : RestEntityStatus.NotFound, courseLab);
    }

    [HttpGet("{courseId}/labs/{courseLabId}")]
    public async Task<ApiRequestResult<CourseLabData>> GetOneCourseLab(System.Int64 courseId, System.Int64 courseLabId, DataConversionOption include = DataConversionOption.Default)
    {
        var courseLabResult = await _getOneCourseLab(courseId, courseLabId);
        if (courseLabResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<CourseLabData>(courseLabResult.Item1);
        var courseLab = courseLabResult.Item2 ?? throw new NotImplementedException("sanity check");
        return ApiRequestResult.Success<CourseLabData>(courseLab.ToData(include));
    }

    [HttpPost("{courseId}/labs")]
    public async Task<ApiRequestResult<CourseLabData>> CreateCourseLab(System.Int64 courseId, [FromBody] CourseLabData courseLabData)
    {
        var oneResult = await _getOneInternal(courseId);
        if (oneResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<CourseLabData>(oneResult.Item1);
        var course = oneResult.Item2;

        var newCourseLab = new CourseLabModel
        {
            Name = courseLabData.Name,
            GitLabName = GitLabNameTransformer.Transliterate(courseLabData.Name),
            CourseId = course.Id
        };

        course.CourseLabs.Add(newCourseLab);

        try
        {
            await Storage.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiRequestResult.Failure<CourseLabData>("db insert failed. constrints failed");
        }

        await DataHub.SendUpdate<CourseLabData, CourseLabModel>(newCourseLab);
        return ApiRequestResult.Success<CourseLabData>(newCourseLab.ToData());
    }

    [HttpPut("{courseId}/labs/{courseLabId}")]
    public async Task<ApiRequestResult<CourseLabData>> UpdateCourseLab(System.Int64 courseId, System.Int64 courseLabId,
            [FromBody] CourseLabData courseLabData)
    {
        var courseLabResult = await _getOneCourseLab(courseId, courseLabId);
        if (courseLabResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<CourseLabData>(courseLabResult.Item1);
        var courseLab = courseLabResult.Item2 ?? throw new NotImplementedException("sanity check");

        courseLab.Name = courseLabData.Name;
        courseLab.GitLabName = courseLabData.GitLabName;

        try
        {
            await Storage.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiRequestResult.Failure<CourseLabData>("db update error. constraints failed");
        }

        return ApiRequestResult.Success<CourseLabData>(courseLab.ToData());
    }
}