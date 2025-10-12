namespace LabServer.Server.Data;

using System.Collections.Concurrent;
using System.Reflection;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using LabServer.Server.Models;
using LabServer.Server.Models.Uni;
using LabServer.Server.Service;


public class LabsContext : IdentityDbContext<UserModel, RoleModel, System.Int64>
{
    public IGitLab GitLabService { get; }
    public LabsContext(DbContextOptions options, IGitLab gitLabService) : base(options)
    {
        GitLabService = gitLabService;
    }

    // Courses
    public DbSet<CourseModel> Courses { get; set; }
    public DbSet<GroupCourseMapping> GroupCourseMapping { get; set; } // courses to groups
    // Students, Groups
    public DbSet<StudentModel> Students { get; set; }
    public DbSet<GroupModel> Groups { get; set; }
    public DbSet<GroupProfessorMapping> GroupProfessorMapping { get; set; } // groups to profs (AKA users)
    // Labs
    public DbSet<CourseLabModel> CourseLabs { get; set; } // general lab data (description, link to course, etc.)
    public DbSet<GroupCourseLabMapping> GroupCourseLabMapping { get; set; } // lab assigned to a particular students' group (includes such info as assingmen (start) date, deadline)
    public DbSet<StudentLabModel> StudentLabs { get; set; } // student's instance of the lab (open date, state (inProgress, completed), links to lab submissions)
    public DbSet<StudentLabSubmissionModel> StudentLabSubmissions { get; set; } // lab submissions (via GitLab merge requests, links to test runs)
    // Tests
    public DbSet<TestModel> LabTests { get; set; } // general info about the test
    public DbSet<CourseLabTestMapping> CourseLabTestMapping { get; set; } // application of the test to a particular lab (e.g. test such as AntiPlagiarism might be reused)
    public DbSet<TestRunModel> TestRuns { get; set; } // test scheduling, executino for a particular lab submission

    private static ConcurrentDictionary<Type, PropertyInfo>? _dbSets;
    public DbSet<D> GetStorage<D>() where D : DBObjectModel
    {
        if (_dbSets == null)
        {
            _dbSets = new ConcurrentDictionary<Type, PropertyInfo>();
            var properties = this.GetType().GetProperties();

            foreach (var property in properties)
            {
                var propType = property.PropertyType;

                var isDbSet = propType.IsGenericType && (typeof(DbSet<>).IsAssignableFrom(propType.GetGenericTypeDefinition()));

                if (isDbSet)
                {
                    _dbSets.TryAdd(propType.GenericTypeArguments.First(), property);
                }
            }
        }

        if (!_dbSets.ContainsKey(typeof(D)))
            throw new NotImplementedException("requested type is not stored in DB");

        return _dbSets[typeof(D)].GetValue(this, null) as DbSet<D> ?? throw new NotImplementedException("invalid type in dictionary");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // students to groups
        builder.Entity<StudentModel>(entity =>
        {
            entity.HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // groups to profs
        // profs to groups
        // via GroupProfessorMapping
        builder.Entity<GroupProfessorMapping>(entity =>
        {
            entity.HasOne(gpm => gpm.Entity)
                .WithMany(g => g.AccessMappings)
                .HasForeignKey(gpm => gpm.EntityId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(gpm => gpm.User)
                .WithMany(u => u.GroupsMapping)
                .HasForeignKey(gpm => gpm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // courses to groups
        // groups to courses
        // iva GroupCourseMapping
        builder.Entity<GroupCourseMapping>(entity =>
        {
            entity.HasOne(gcm => gcm.Group)
                .WithMany(g => g.CoursesMapping)
                .HasForeignKey(gcm => gcm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(gcm => gcm.Course)
                .WithMany(c => c.GroupsMapping)
                .HasForeignKey(gcm => gcm.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // group courses to group courses labs (AKA instances of labs for a particular group with additional info, e.g. deadline)
        builder.Entity<GroupCourseLabMapping>(entity =>
        {
            entity.HasOne(gclm => gclm.GroupCourse)
                .WithMany(gcm => gcm.GroupCourseLabs)
                .HasForeignKey(gclm => gclm.GroupCourseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(gclm => gclm.CourseLab)
                .WithMany(cl => cl.AssignedGroups)
                .HasForeignKey(gclm => gclm.CourseLabId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // course labs to courses
        builder.Entity<CourseLabModel>(entity =>
        {
            entity.HasOne(cl => cl.Course)
                .WithMany(c => c.CourseLabs)
                .HasForeignKey(cl => cl.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // course labs to tests
        // test to courses
        // via mapping
        builder.Entity<CourseLabTestMapping>(entity =>
        {
            entity.HasOne(map => map.CourseLab)
                .WithMany(cl => cl.TestMapping)
                .HasForeignKey(map => map.CourseLabId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(map => map.Test)
                .WithMany(t => t.LabMapping)
                .HasForeignKey(map => map.TestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // stud labs to students
        // stud labs to group course labs (AKA group lab instances to student lab instances)
        builder.Entity<StudentLabModel>(entity =>
        {
            entity.HasOne(sl => sl.Student)
                .WithMany(s => s.Labs)
                .HasForeignKey(sl => sl.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(sl => sl.GroupCourseLab)
                .WithMany(gcm => gcm.LabsForStudents)
                .HasForeignKey(sl => sl.GroupCourseLabId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // stud lab submissions to stud labs
        builder.Entity<StudentLabSubmissionModel>(entity =>
        {
            entity.HasOne(sls => sls.StudentLab)
                .WithMany(sl => sl.LabSubmissions)
                .HasForeignKey(sls => sls.StudentLabId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // course lab test to student lab submission
        // student lab submission to course lab test
        // via mapping AKA TestRun
        builder.Entity<TestRunModel>(entity =>
        {
            entity.HasOne(trm => trm.CourseLabTest)
                .WithMany(clt => clt.TestRuns)
                .HasForeignKey(trm => trm.CourseLabTestMappingId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(trm => trm.StudentLabSubmission)
                .WithMany(sls => sls.TestRuns)
                .HasForeignKey(trm => trm.StudentLabSubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }

}

