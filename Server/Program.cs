using System.Security.Claims;

using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using LabServer.Server.Service;
using LabServer.Server.Data;
using LabServer.Server.Models;
using LabServer.Server.Hubs;

using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        {
            policy.SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

// Add services to the container.
//var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
builder.Services.AddDbContext<LabsContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PgSql")));
    //options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));
    //options.UseMySql(builder.Configuration.GetConnectionString("SQLite"), serverVersion));

builder.Services.AddDefaultIdentity<UserModel>(options =>
    {
        options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
    })
    .AddRoles<RoleModel>()
    .AddEntityFrameworkStores<LabsContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSecurityKey"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!System.String.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
builder.Services.AddControllersWithViews();
// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "LabServer API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new System.String[] {}
        }
    });
});

//builder.Services.AddRazorPages();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" }
    );
});

builder.Services.AddScoped(typeof(IDBStorage<>), typeof(DBStorage<>));
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddSingleton<IGitLab, GitLabDriver>();
builder.Services.AddSingleton<MergeRequestMonitor>();
builder.Services.AddHostedService(
    provider => provider.GetRequiredService<MergeRequestMonitor>());
builder.Services.AddSingleton<TestRunner>();
builder.Services.AddHostedService(
    provider => provider.GetRequiredService<TestRunner>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    // SignalR hub
    endpoints.MapHub<DataHub>("/hub/data");
    
    endpoints.MapFallbackToFile("index.html");
});

app.Run();
