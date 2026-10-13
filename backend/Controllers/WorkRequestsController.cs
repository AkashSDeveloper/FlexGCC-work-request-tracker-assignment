using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Data;
using Tracker.Models;
namespace Tracker.Controllers;

[ApiController]
[Route("api/work-requests")]
public class WorkRequestsController(AppDbContext db) : ControllerBase {
 public record CreateRequest(string Title, string ClientName, string? Description, Priority Priority, WorkStatus Status, DateTime DueDate);
 public record StatusRequest(string Status);
 public record NoteRequest(string Note);
 object Error(string code,string message) => new { error = new { code, message, traceId = HttpContext.TraceIdentifier } };

 [HttpGet]
 public async Task<IActionResult> List(string? status,string? search,int page=1,int pageSize=10) {
  if(page<1||pageSize<1||pageSize>100) return BadRequest(Error("invalid_pagination","page must be >= 1 and pageSize must be 1-100."));
  var q=db.WorkRequests.AsNoTracking().AsQueryable();
  if(!string.IsNullOrWhiteSpace(status)) {
   if(!Enum.TryParse<WorkStatus>(status,true,out var s)||!Enum.IsDefined(s)) return BadRequest(Error("invalid_status","Allowed statuses: New, InProgress, Blocked, Completed."));
   q=q.Where(x=>x.Status==s);
  }
  if(!string.IsNullOrWhiteSpace(search)) { var term=search.Trim(); q=q.Where(x=>x.Title.Contains(term)||x.ClientName.Contains(term)); }
  var total=await q.CountAsync();
  var items=await q.OrderByDescending(x=>x.CreatedDate).Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
  return Ok(new { items,page,pageSize,total });
 }

 [HttpGet("{id:int}")]
 public async Task<IActionResult> Get(int id) {
  var item=await db.WorkRequests.FindAsync(id);
  return item is null ? NotFound(Error("not_found","Work request was not found.")) : Ok(item);
 }

 [HttpPost]
 public async Task<IActionResult> Create(CreateRequest dto) {
  if(string.IsNullOrWhiteSpace(dto.Title)||string.IsNullOrWhiteSpace(dto.ClientName)||dto.DueDate==default)
   return BadRequest(Error("validation_error","Title, client name, and due date are required."));
  if(!Enum.IsDefined(dto.Priority)) return BadRequest(Error("invalid_priority","Priority must be Low, Medium, or High."));
  if(!Enum.IsDefined(dto.Status)) return BadRequest(Error("invalid_status","Status must be New, InProgress, Blocked, or Completed."));
  if(dto.Title.Length>100||dto.ClientName.Length>120||(dto.Description?.Length??0)>2000)
   return BadRequest(Error("validation_error","One or more fields exceed their maximum length."));
  var item=new WorkRequest { Title=dto.Title.Trim(),ClientName=dto.ClientName.Trim(),Description=dto.Description,Priority=dto.Priority,Status=dto.Status,DueDate=dto.DueDate,CreatedDate=DateTime.UtcNow };
  db.WorkRequests.Add(item); await db.SaveChangesAsync();
  return CreatedAtAction(nameof(Get),new { id=item.Id },item);
 }

 [HttpPatch("{id:int}/status")]
 public async Task<IActionResult> UpdateStatus(int id,StatusRequest dto) {
  var item=await db.WorkRequests.FindAsync(id);
  if(item is null) return NotFound(Error("not_found","Work request was not found."));
  if(!Enum.TryParse<WorkStatus>(dto.Status,true,out var status)||!Enum.IsDefined(status)) return BadRequest(Error("invalid_status","Allowed statuses: New, InProgress, Blocked, Completed."));
  item.Status=status; item.UpdatedDate=DateTime.UtcNow; await db.SaveChangesAsync(); return Ok(item);
 }

 [HttpPost("{id:int}/notes")]
 public async Task<IActionResult> AddNote(int id,NoteRequest dto) {
  var item=await db.WorkRequests.FindAsync(id);
  if(item is null) return NotFound(Error("not_found","Work request was not found."));
  if(string.IsNullOrWhiteSpace(dto.Note)||dto.Note.Length>1000) return BadRequest(Error("validation_error","Note is required and must be at most 1000 characters."));
  var line=$"{DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC - {dto.Note.Trim()}";
  item.Notes=string.IsNullOrWhiteSpace(item.Notes)?line:item.Notes+Environment.NewLine+line;
  item.UpdatedDate=DateTime.UtcNow; await db.SaveChangesAsync(); return Ok(item);
 }
}
