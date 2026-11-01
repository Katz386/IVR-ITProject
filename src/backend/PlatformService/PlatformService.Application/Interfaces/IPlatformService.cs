using PlatformService.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Application.Interfaces
{
    public interface IPlatformService
    {
        public Task<bool> IncreaseArticlesCount();
        public Task<bool> DecreaseArticlesCount();
        public Task<bool> IncreaseTasksCount(string difficulty);
        public Task<bool> DecreaseTasksCount(string difficulty);
        public Task<bool> IncreaseUsersCount();
        public Task<GetStatisticsResponse> GetStatistics();
    }
}
