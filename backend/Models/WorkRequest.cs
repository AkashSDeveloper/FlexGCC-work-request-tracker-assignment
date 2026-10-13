using System.ComponentModel.DataAnnotations;
namespace Tracker.Models;
public enum Priority { Low, Medium, High }
public enum WorkStatus { New, InProgress, Blocked, Completed }
public class WorkRequest {
 public int Id { get; set; }
 [Required, MaxLength(100)] public string Title { get; set; } = "";
 [Required, MaxLength(120)] public string ClientName { get; set; } = "";
 [MaxLength(2000)] public string? Description { get; set; }
 public Priority Priority { get; set; } = Priority.Medium;
 public WorkStatus Status { get; set; } = WorkStatus.New;
 public DateTime DueDate { get; set; }
 public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
 public DateTime? UpdatedDate { get; set; }
 [MaxLength(4000)] public string? Notes { get; set; }
}
