using BE.Data.Models;
namespace BE.Data.Dtos.UsersDTO
{
    public class GetAllDto : Users
    {


        public GetAllDto(Users user)
        {
            id = user.id;
            userCode = user.userCode;
            userPassword = user.userPassword;
            userCreated = user.userCreated;
            dateCreated = user.dateCreated;
            userModified = user.userModified;
            dateModified = user.dateModified;
            firstName = user.firstName;
            lastName = user.lastName;
            phoneNumber = user.phoneNumber;
            dOB = user.dOB;
            gender = user.gender;
            address = user.address;
            university = user.university;
            yearGraduated = user.yearGraduated;
            email = user.email;
            skype = user.skype;
            workStatus = user.workStatus;
            dateStartWork = user.dateStartWork;
            dateLeave = user.dateLeave;
            maritalStatus = user.maritalStatus;
            reasonResignation = user.reasonResignation;
            identitycard = user.identitycard;
            IdGroup = user.IdGroup;
            isDeleted = user.isDeleted;
        }

    }
}
