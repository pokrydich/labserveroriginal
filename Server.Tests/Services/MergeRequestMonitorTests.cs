namespace LabServer.Server.Tests.Services;

using GitLab.Models.MergeRequest;
using LabServer.Server.Models.Uni;
using LabServer.Server.Service;
using LabServer.Server.Tests.TestFixtures;
using LabServer.Shared.Models.Uni;

public class MergeRequestMonitorTests
{
    [Fact]
    public async Task ExecuteAsync_AddsNewSubmissionForNewMergeRequest()
    {
        var iterationReached = new ManualResetEventSlim(false);

        var mergeRequest = new GitLabMergeRequest
        {
            Id = 200,
            Iid = 10,
            ProjectId = 1,
            TargetProjectId = 500,
            SourceProjectId = 300,
            Title = "Lab submission",
            StateRaw = "opened",
            CreatedAtRaw = "2025-01-01T00:00:00.000Z",
            Author = new MergeUserInfo { Id = 42, Name = "Student", Username = "student" },
            Assignees = new List<MergeUserInfo>(),
            CommitHash = "abc123",
            WebUrl = "http://gitlab.test/mr/10"
        };

        var gitLabMock = new Mock<IGitLab>();
        gitLabMock.Setup(m => m.GetMergeRequests())
            .ReturnsAsync(new List<GitLabMergeRequest> { mergeRequest })
            .Callback(() => iterationReached.Set());

        await using var context = InMemoryDbContextFactory.CreateContext(Guid.NewGuid().ToString(), gitLabMock.Object);

        var group = new GroupModel
        {
            Name = "Group 1",
            GitLabName = "group1",
            AccessMappings = new List<GroupProfessorMapping>()
        };
        var course = new CourseModel
        {
            Name = "Course",
            GitLabName = "course"
        };
        var courseLab = new CourseLabModel
        {
            Name = "Lab1",
            GitLabName = "lab1",
            Course = course
        };
        var groupCourse = new GroupCourseMapping
        {
            Group = group,
            Course = course
        };
        var groupCourseLab = new GroupCourseLabMapping
        {
            GroupCourse = groupCourse,
            CourseLab = courseLab,
            StartDate = DateTime.UtcNow
        };
        var student = new StudentModel
        {
            Name = "Student",
            Username = "student",
            Email = "student@test.dev",
            Group = group,
            GitLabUserId = 42
        };
        var studentLab = new StudentLabModel
        {
            GroupCourseLab = groupCourseLab,
            Student = student,
            Status = StudentLabStatus.cInProgress,
            GitLabProjectId = 500,
            LabSubmissions = new List<StudentLabSubmissionModel>()
        };

        group.Students = new List<StudentModel> { student };
        groupCourse.GroupCourseLabs = new List<GroupCourseLabMapping> { groupCourseLab };
        groupCourseLab.LabsForStudents = new List<StudentLabModel> { studentLab };
        student.Labs = new List<StudentLabModel> { studentLab };

        context.Groups.Add(group);
        context.Courses.Add(course);
        context.CourseLabs.Add(courseLab);
        context.GroupCourseMapping.Add(groupCourse);
        context.GroupCourseLabMapping.Add(groupCourseLab);
        context.Students.Add(student);
        context.StudentLabs.Add(studentLab);

        await context.SaveChangesAsync();

        using var serviceScopeFactory = new TestServiceScopeFactory(context);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"Services:{nameof(MergeRequestMonitor)}:period"] = "00:00:01"
            })
            .Build();

        var monitor = new MergeRequestMonitor(gitLabMock.Object, serviceScopeFactory, configuration);

        await monitor.StartAsync(CancellationToken.None);
        Assert.True(iterationReached.Wait(TimeSpan.FromSeconds(2)));
        await Task.Delay(100); // allow persistence

        Assert.Equal(1, await context.StudentLabSubmissions.CountAsync());
        var submission = await context.StudentLabSubmissions.SingleAsync();
        Assert.Equal(mergeRequest.Id, submission.GitLabMergeRequestId);
        Assert.Equal(StudentLabSubmissionStatus.cActive, submission.Status);

        var stopCts = new CancellationTokenSource();
        stopCts.Cancel();
        await monitor.StopAsync(stopCts.Token);
    }
}
