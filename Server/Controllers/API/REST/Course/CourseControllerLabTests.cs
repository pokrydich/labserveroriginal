namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using LabServer.Server.Models.Uni;
using LabServer.Server.Models;
using LabServer.Server.Hubs;

using LabServer.Shared.Models;
using LabServer.Shared.Models.Uni;

public partial class CoursesController : BaseRestController<CourseModel, CourseData>
{
    [HttpGet("{courseId}/labs/{courseLabId}/tests")]
    public async Task<ApiRequestResult<IEnumerable<CourseLabTestData>>> GetCourseLabTests(System.Int64 courseId, System.Int64 courseLabId)
    {
        var courseLabResult = await _getOneCourseLab(courseId, courseLabId);
        if (courseLabResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<IEnumerable<CourseLabTestData>>(courseLabResult.Item1);
        var courseLab = courseLabResult.Item2 ?? throw new NotImplementedException("sanity check");


        return ApiRequestResult.Success<IEnumerable<CourseLabTestData>>(courseLab.TestMapping.ToData<CourseLabTestMapping, CourseLabTestData>());
    }

    [HttpPost("{courseId}/labs/{courseLabId}/tests")]
    public async Task<ApiRequestResult<CourseLabTestData>> CreateCourseLabTest(System.Int64 courseId, System.Int64 courseLabId,
                                                        [FromBody] CourseLabTestData courseLabTestData)
    {
        var courseLabResult = await _getOneCourseLab(courseId, courseLabId);
        if (courseLabResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<CourseLabTestData>(courseLabResult.Item1);
        var courseLab = courseLabResult.Item2 ?? throw new NotImplementedException("sanity check");

        var newCourseLabTestMapping = new CourseLabTestMapping
        {
            CourseLabId = courseLab.Id,
            TestId = courseLabTestData.TestId,
            Activated = courseLabTestData.Activated
        };
        courseLab.TestMapping.Add(newCourseLabTestMapping);
        try
        {
            await Storage.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiRequestResult.Failure<CourseLabTestData>("db update failed. constrains failed");
        }

        await DataHub.SendUpdate<CourseLabTestData, CourseLabTestMapping>(newCourseLabTestMapping, DataConversionOption.Parent);
        return ApiRequestResult.Success<CourseLabTestData>(newCourseLabTestMapping.ToData());
    }

    [HttpPut("{courseId}/labs/{courseLabId}/tests/{testId}")]
    public async Task<ApiRequestResult<CourseLabTestData>> UpdateCourseLabTest(System.Int64 courseId, System.Int64 courseLabId,
                            System.Int64 testId, [FromBody] CourseLabTestData courseLabTestData)
    {
        var courseLabResult = await _getOneCourseLab(courseId, courseLabId);
        if (courseLabResult.Item1 != RestEntityStatus.Ready)
            return _returnBadResult<CourseLabTestData>(courseLabResult.Item1);
        var courseLab = courseLabResult.Item2 ?? throw new NotImplementedException("sanity check");

        var courseLabTest = courseLab.TestMapping.SingleOrDefault(tm => tm.CourseLabId == courseLab.Id && tm.TestId == testId);
        if (courseLabTest == null)
            return ApiRequestResult.Failure<CourseLabTestData>("not found");

        courseLabTest.Activated = courseLabTestData.Activated;
        try
        {
            await Storage.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiRequestResult.Failure<CourseLabTestData>("db update error. constraints failed");
        }

        await DataHub.SendUpdate<CourseLabTestData, CourseLabTestMapping>(courseLabTest, DataConversionOption.Full);
        return ApiRequestResult.Success<CourseLabTestData>(courseLabTest.ToData());
    }
}