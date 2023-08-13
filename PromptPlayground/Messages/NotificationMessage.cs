using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Messages
{
    public class NotificationMessage
    {
        public enum NotificationType
        {
            Information,
            Success,
            Warning,
            Error
        }
        public NotificationType Level { get; set; }
        public string Title { get; set; }
        public string? Message { get; set; }
        public NotificationMessage(string title, string? message, NotificationType level = NotificationType.Information)
        {
            Level = level;
            Title = title;
            Message = message;
        }
    }
}
