using BE.Data.Enums.TaskEnums;

namespace BE.Data.Dtos.TaskDtos
{
    public class EditTaskByIdDto
    {
        public int? assignee { get; set; }
        public string taskName { get; set; }
        public int? workTime { get; set; }
        public string? description { get; set; }
        public Status status { get; set; }
        public DateTime? startTaskDate { get; set; }
        public DateTime? endTaskDate { get; set; }
        public int idProject { get; set; }
    }
}
