using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Request;
using UserService.Application.DTOs.Response;

namespace UserService.Application.Interfaces
{
    public interface IUserService
    {
        public Task<bool> CreateProfile(Guid id);
        public Task<ReadArticleResponse> ReadArticle(string id, int articleId);
        public Task<bool> SolveTask(Guid id, int taskId);
        public Task<EditProfileResponse> EditProfile(string id, EditProfileRequest request);
        public Task<GetProfileResponse> GetProfile(string id);
    }
}
