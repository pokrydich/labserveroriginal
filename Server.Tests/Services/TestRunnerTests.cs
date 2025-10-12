namespace LabServer.Server.Tests.Services;

using System.Net;
using GitLab.Models;
using GitLab.Models.MergeRequest;
using GitLab.Models.Note;
using LabServer.Server.Models.Uni;
using LabServer.Server.Service;
using LabServer.Server.Tests.TestFixtures;
using LabServer.Shared.Models.TestAPI;
using LabServer.Shared.Models.Uni;

public class TestRunnerTests
{
    private static GitLabMergeRequest CreateMergeRequest()
        => new GitLabMergeRequest
        {
            Id = 300,
            Iid = 15,
            ProjectId = 200,
            TargetProjectId = 600,
            SourceProjectId = 550,
            Title = "Lab submission",
            StateRaw = "opened",
            CreatedAtRaw = "2025-01-01T00:00:00.000Z",
            Author = new MergeUserInfo { Id = 42, Name = "Student", Username = "student" },
            Assignees = new List<MergeUserInfo>(),
            CommitHash = "abcdef",
            WebUrl = "http://gitlab.test/mr/15"
        };

    private static GitLab.GitLabClient CreateGitLabClient(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        var client = new GitLab.GitLabClient("http://gitlab.test", "token");
        var clientField = typeof(GitLab.GitLabClient).GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic);
        clientField!.SetValue(client, new HttpClient(new StubHttpMessageHandler(handler)));
        return client;
    }

    private static (LabsContext context, StudentLabSubmissionModel submission) SeedSubmissionGraph(IGitLab gitLab, bool addScheduledRun = false)
    {
        var context = InMemoryDbContextFactory.CreateContext(Guid.NewGuid().ToString(), gitLab);

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
            Course = course,
            TestMapping = new List<CourseLabTestMapping>()
        };
        course.CourseLabs = new List<CourseLabModel> { courseLab };
        var testModel = new TestModel
        {
            Name = "Static analysis",
            TestServerUrl = "http://test-runner",
            LabMapping = new List<CourseLabTestMapping>()
        };
        var mapping = new CourseLabTestMapping
        {
            CourseLab = courseLab,
            Test = testModel,
            Activated = true,
            TestRuns = new List<TestRunModel>()
        };
        courseLab.TestMapping.Add(mapping);
        testModel.LabMapping.Add(mapping);

        var groupCourse = new GroupCourseMapping
        {
            Group = group,
            Course = course,
            GroupCourseLabs = new List<GroupCourseLabMapping>()
        };
        var groupCourseLab = new GroupCourseLabMapping
        {
            GroupCourse = groupCourse,
            CourseLab = courseLab,
            StartDate = DateTime.UtcNow,
            LabsForStudents = new List<StudentLabModel>()
        };
        courseLab.AssignedGroups = new List<GroupCourseLabMapping> { groupCourseLab };
        groupCourse.GroupCourseLabs.Add(groupCourseLab);

        var student = new StudentModel
        {
            Name = "Student",
            Username = "student",
            Email = "student@test.dev",
            Group = group,
            GitLabUserId = 42,
            Labs = new List<StudentLabModel>()
        };
        group.Students = new List<StudentModel> { student };

        var studentLab = new StudentLabModel
        {
            GroupCourseLab = groupCourseLab,
            Student = student,
            Status = StudentLabStatus.cInProgress,
            GitLabProjectId = 600,
            LabSubmissions = new List<StudentLabSubmissionModel>()
        };
        groupCourseLab.LabsForStudents.Add(studentLab);
        student.Labs.Add(studentLab);

        var submission = new StudentLabSubmissionModel
        {
            StudentLab = studentLab,
            Status = StudentLabSubmissionStatus.cActive,
            GitLabMergeRequestId = 300,
            GitLabMergeRequestIid = 15,
            SubmittedDate = DateTime.UtcNow,
            TestRuns = new List<TestRunModel>()
        };
        studentLab.LabSubmissions.Add(submission);

        if (addScheduledRun)
        {
            var existingRun = new TestRunModel
            {
                CourseLabTestMapping = mapping,
                StudentLabSubmission = submission,
                State = TestRunState.Scheduled,
                ScheduledDate = DateTime.UtcNow.AddMinutes(-5)
            };
            submission.TestRuns.Add(existingRun);
            mapping.TestRuns.Add(existingRun);
        }

        context.Groups.Add(group);
        context.Courses.Add(course);
        context.CourseLabs.Add(courseLab);
        context.LabTests.Add(testModel);
        context.CourseLabTestMapping.Add(mapping);
        context.GroupCourseMapping.Add(groupCourse);
        context.GroupCourseLabMapping.Add(groupCourseLab);
        context.Students.Add(student);
        context.StudentLabs.Add(studentLab);
        context.StudentLabSubmissions.Add(submission);

        context.SaveChanges();

        return (context, submission);
    }

    [Fact]
    public async Task ExecuteAsync_SchedulesNewTestRun()
    {
        var gitLabClient = CreateGitLabClient(_ => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(JsonSerializer.Serialize(new GitLabNote { Id = 1, Body = "ok" }), System.Text.Encoding.UTF8, "application/json")
        });

        var mergeRequest = CreateMergeRequest();
        var gitLabMock = new Mock<IGitLab>();
        gitLabMock.Setup(m => m.GetOne<GitLabMergeRequest>(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(ApiResult<GitLabMergeRequest>.MakeSuccess(mergeRequest).WithClient(gitLabClient));

        var (context, submission) = SeedSubmissionGraph(gitLabMock.Object, addScheduledRun: false);

        var scheduleCalled = new ManualResetEventSlim(false);
        var httpHandler = new StubHttpMessageHandler(request =>
        {
            if (request.RequestUri!.AbsoluteUri.EndsWith("/schedule"))
            {
                var body = request.Content!.ReadAsStringAsync().Result;
                var payload = JsonSerializer.Deserialize<ScheduleTestRequestModel>(body);
                Assert.Equal(mergeRequest.CommitHash, payload!.MergeRequest.CommitHash);
                scheduleCalled.Set();
                return StubHttpMessageHandler.JsonResponse(new ScheduleTestResponseModel { Success = true });
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var serviceScopeFactory = new TestServiceScopeFactory(context);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"Services:{nameof(TestRunner)}:period"] = "00:00:01"
            })
            .Build();
        var runner = new TestRunner(serviceScopeFactory, configuration);

        var httpField = typeof(TestRunner).GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic);
        httpField!.SetValue(runner, new HttpClient(httpHandler));

        await runner.StartAsync(CancellationToken.None);
        Assert.True(scheduleCalled.Wait(TimeSpan.FromSeconds(2)));
        await Task.Delay(100);

        await context.Entry(submission).Collection(s => s.TestRuns).LoadAsync();
        Assert.Single(submission.TestRuns);
        Assert.Equal(TestRunState.Scheduled, submission.TestRuns.Single().State);

        var stopCts = new CancellationTokenSource();
        stopCts.Cancel();
        await runner.StopAsync(stopCts.Token);
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesScheduledRunWithResult()
    {
        var gitLabClient = CreateGitLabClient(request =>
        {
            Assert.EndsWith("/notes", request.RequestUri!.AbsolutePath);
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(JsonSerializer.Serialize(new GitLabNote { Id = 2, Body = "ok" }), System.Text.Encoding.UTF8, "application/json")
            };
        });

        var mergeRequest = CreateMergeRequest();
        var gitLabMock = new Mock<IGitLab>();
        gitLabMock.Setup(m => m.GetOne<GitLabMergeRequest>(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(ApiResult<GitLabMergeRequest>.MakeSuccess(mergeRequest).WithClient(gitLabClient));

        var (context, submission) = SeedSubmissionGraph(gitLabMock.Object, addScheduledRun: true);

        await context.Entry(submission).Collection(s => s.TestRuns).LoadAsync();
        var existingRun = submission.TestRuns.Single();

        var resultCalled = new ManualResetEventSlim(false);
        var httpHandler = new StubHttpMessageHandler(request =>
        {
            if (request.RequestUri!.AbsoluteUri.EndsWith("/getresult"))
            {
                resultCalled.Set();
                return StubHttpMessageHandler.JsonResponse(new GetTestResultResponseModel
                {
                    TestCompleted = true,
                    Success = true,
                    Message = "All good"
                });
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var serviceScopeFactory = new TestServiceScopeFactory(context);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"Services:{nameof(TestRunner)}:period"] = "00:00:01"
            })
            .Build();
        var runner = new TestRunner(serviceScopeFactory, configuration);
        var httpField = typeof(TestRunner).GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic);
        httpField!.SetValue(runner, new HttpClient(httpHandler));

        await runner.StartAsync(CancellationToken.None);
        Assert.True(resultCalled.Wait(TimeSpan.FromSeconds(2)));
        await Task.Delay(100);

        Assert.True(existingRun.Success);
        Assert.Equal("All good", existingRun.Message);

        var stopCts = new CancellationTokenSource();
        stopCts.Cancel();
        await runner.StopAsync(stopCts.Token);
    }
}
