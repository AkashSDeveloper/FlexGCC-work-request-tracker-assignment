using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Tracker.Data;
using Tracker.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5080");
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=workrequests.db"));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.WorkRequests.Any()) {
        db.WorkRequests.AddRange(
            new WorkRequest { Title="Review onboarding flow", ClientName="Northstar Ltd", Description="Review the customer onboarding workflow.", Priority=Priority.High, Status=WorkStatus.InProgress, DueDate=DateTime.UtcNow.Date.AddDays(5) },
            new WorkRequest { Title="Prepare monthly report", ClientName="Acme Co", Priority=Priority.Medium, Status=WorkStatus.New, DueDate=DateTime.UtcNow.Date.AddDays(10) });
        db.SaveChanges();
    }
}
app.MapControllers();
app.Run();
