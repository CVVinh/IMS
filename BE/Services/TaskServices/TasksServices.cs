using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.TaskDtos;
using BE.Data.Enums.TaskEnums;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using BE.Services.MemberProjectServices;
using Microsoft.EntityFrameworkCore;

namespace BE.Services.TaskServices
{
    public interface ITasksServices
    {
        Task<bool> deleteTaskByIdAsync(int Id);
        Task<BaseResponse<Tasks>> getTaskByIdAsync(int Id);
        Task<BaseResponse<Tasks>> addNewTaskAsync(AddNewTaskDto addNewTaskDto);
        Task<BaseResponse<ICollection<Tasks>>> GetTasksByIdUserAtProjectAsync(int idUser, int idProject);
    }

    public class TasksServices : ITasksServices
    {
        private readonly AppDbContext _appContext;
        private readonly IMemberProjectServices _memberProjectServices;
        public TasksServices(AppDbContext appContext, 
                            IMemberProjectServices memberProjectServices)
        {
            _appContext = appContext;
            _memberProjectServices = memberProjectServices;
        }

        public async Task<BaseResponse<Tasks>> addNewTaskAsync(AddNewTaskDto addNewTaskDto)
        {
            var success = false;
            var message = "";
            var data = new Tasks();
            try
            {
                #region genarate taskcode
                var projectDb = await _appContext.Projects.FindAsync(addNewTaskDto.idProject);
                if (projectDb == null)
                {
                    success = false;
                    message = "Cannot find project";
                    data = null;
                    return new BaseResponse<Tasks>(success, message, data);
                }
                var taskCode = await _appContext.tasks.Where(t => t.idProject == addNewTaskDto.idProject)
                                                      .OrderByDescending(t => t.taskCode)
                                                      .ToListAsync();
                var code = "";
                if (!taskCode.Any())
                {
                    code = CommonHelper.RandomString(4, projectDb.ProjectCode, 1);
                }
                else
                {
                    var maxCode = Int32.Parse(taskCode[0].taskCode.Replace(  projectDb.ProjectCode, "")) + 1;
                    code = CommonHelper.RandomString(4, projectDb.ProjectCode, maxCode);
                }                    
                #endregion

                var newTask = new Tasks
                {
                    taskCode = code,
                    assignee = addNewTaskDto.assignee,
                    taskName = addNewTaskDto.taskName,
                    description = addNewTaskDto.description,
                    workTime = addNewTaskDto.workTime,
                    status = addNewTaskDto.status,
                    startTaskDate = addNewTaskDto.startTaskDate,
                    endTaskDate = addNewTaskDto.endTaskDate,
                    createUser = addNewTaskDto.createUser,
                    idProject = addNewTaskDto.idProject,
                    createTaskDate = DateTime.Now
                };

                await _appContext.tasks.AddAsync(newTask);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Add new task successfully";
                data = newTask;
                return new BaseResponse<Tasks>(success,message,data);
            }
            catch
            {
                message = "Adding new task failed! Starting date or ending date isn't in the correct format !";
                data = null;
                return new BaseResponse<Tasks>(success, message, data);
            }
        }

        public async Task<bool> deleteTaskByIdAsync(int Id)
        {
            try
            {
                var result = await _appContext.tasks.SingleOrDefaultAsync(h => h.idTask == Id &&    
                                                                          h.status != Status.Deleted);
                if (result is null)
                {
                    return false;
                }

                result.status = Status.Deleted;
                await _appContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<BaseResponse<Tasks>> getTaskByIdAsync(int Id)
        {
            var success = false;
            var message = "";
            var data = new Tasks();
            try
            {
                var checkIdTask = await _appContext.tasks.Where( t => t.idTask == Id && t.status != Status.Deleted).FirstOrDefaultAsync();
                if (checkIdTask is null)
                {
                    message = "Id task does not exist";
                    return new BaseResponse<Tasks>(success, message, data);
                }

                success = true;
                message = "Getting task by id task sucessfully";
                data = checkIdTask;
                return new BaseResponse<Tasks>(success, message, data);
            }
            catch
            {
                message = "Don't connect database !";
                data = null;
                return new BaseResponse<Tasks>(success, message, data);
            }
        }

        public async Task<BaseResponse<ICollection<Tasks>>> GetTasksByIdUserAtProjectAsync(int idUser, int idProject)
        {
            var success = false;
            var message = "";
            var data = new List<Tasks>();
            try
            {
                // test idUser existed in project
                var memberProject = await _memberProjectServices.GetMemberByIdUserAtProjectAsync(idUser, idProject);
                if (!memberProject._success)
                {
                    message = memberProject._Message;
                    return new BaseResponse<ICollection<Tasks>>(success, message, data);
                }

                // getting tasks of idUser in project
                var tasks = await _appContext.tasks.Where(t => t.assignee == memberProject._Data.Id &&
                                                                    t.status != Status.Deleted).ToListAsync();
                if (!tasks.Any())
                {
                    message = "This user doesn't have task !";
                    return new BaseResponse<ICollection<Tasks>>(success, message, data);
                }

                data = tasks.ToList();
                success = true;
                message = "Getting tasks of idUser in project successfully";
                return new BaseResponse<ICollection<Tasks>>(success, message, data);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return new BaseResponse<ICollection<Tasks>>(success, message, data);
            }
        }
    }
}

