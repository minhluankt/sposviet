using System;

namespace Application.DTOs.Logs
{
    public class AuditLogResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ComId { get; set; }
        public string Type { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string AffectedColumns { get; set; }
        public string PrimaryKey { get; set; }
    }
    public class AuditLogByUser: AuditLogResponse
    {
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
    }
}