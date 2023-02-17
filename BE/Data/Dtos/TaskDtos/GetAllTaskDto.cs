using BE.Data.Enums.TaskEnums;

namespace BE.Data.Dtos.TaskDto
{
    public class GetAllTaskDto
    {
        public int idTask { get; set; }
        public int idParent { get; set; }
        public string taskName { get; set; }
        public string? description { get; set; }
        public Status status { get; set; }
        public DateTime? startTaskDate { get; set; }
        public DateTime? endTaskDate { get; set; }
        public DateTime createTaskDate { get; set; }
        public int createUser { get; set; }
        public int idProject { get; set; }
    }
}
