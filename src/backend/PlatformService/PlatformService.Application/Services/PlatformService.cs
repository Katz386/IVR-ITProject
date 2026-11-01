using PlatformService.Application.DTOs.Response;
using PlatformService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Application.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly IPlatformRepository _platformRepository;

        public PlatformService(IPlatformRepository platformRepository)
        {
            _platformRepository = platformRepository;
        }

        public async Task<bool> DecreaseArticlesCount()
        {
            var s = await _platformRepository.GetStatistics();

            if (s == null)
            {
                Console.WriteLine("Statistics == null in DecreaseArticlesCount");
                return false;
            }

            s.ArticlesCount -= 1;

            if (await _platformRepository.SaveChangesAsync())
            {
                return true;
            }
            else
            {
                Console.WriteLine("Unhandled error in DecreaseArticlesCount");
                return false;
            }
        }

        public async Task<bool> DecreaseTasksCount(string difficulty)
        {
            var s = await _platformRepository.GetStatistics();

            if (s == null)
            {
                Console.WriteLine("Statistics == null in DecreaseTasksCount");
                return false;
            }

            switch (difficulty.ToLower().Trim())
            {
                case "easy":
                    s.EasyTasksCount -= 1;
                    break;
                case "medium":
                    s.MediumTasksCount -= 1;
                    break;
                case "hard":
                    s.HardTasksCount -= 1;
                    break;
            }

            if (await _platformRepository.SaveChangesAsync())
            {
                return true;
            }
            else
            {
                Console.WriteLine("Unhandled error in DecreaseTasksCount");
                return false;
            }
        }

        public async Task<GetStatisticsResponse> GetStatistics()
        {
            var s = await _platformRepository.GetStatistics();

            if (s == null)
                return new GetStatisticsResponse { Message = "Something went wrong", Status = 500 };

            return new GetStatisticsResponse { Statistics = s, Message = "Success", Status = 200 };
        }

        public async Task<bool> IncreaseArticlesCount()
        {
            var s = await _platformRepository.GetStatistics();

            if (s == null)
            {
                Console.WriteLine("Statistics == null in IncreaseArticlesCount");
                return false;
            }

            s.ArticlesCount += 1;

            if (await _platformRepository.SaveChangesAsync())
            {
                return true;
            }
            else
            {
                Console.WriteLine("Unhandled error in IncreaseArticlesCount");
                return false;
            }
        }

        public async Task<bool> IncreaseTasksCount(string difficulty)
        {
            var s = await _platformRepository.GetStatistics();

            if (s == null)
            {
                Console.WriteLine("Statistics == null in IncreaseTasksCount");
                return false;
            }

            switch (difficulty.ToLower().Trim())
            {
                case "easy":
                    s.EasyTasksCount += 1;
                    break;
                case "medium":
                    s.MediumTasksCount += 1;
                    break;
                case "hard":
                    s.HardTasksCount += 1;
                    break;
            }

            if (await _platformRepository.SaveChangesAsync())
            {
                return true;
            }
            else
            {
                Console.WriteLine("Unhandled error in IncreaseTasksCount");
                return false;
            }
        }

        public async Task<bool> IncreaseUsersCount()
        {
            var s = await _platformRepository.GetStatistics();

            if (s == null)
            {
                Console.WriteLine("Statistics == null in IncreaseUsersCount");
                return false;
            }

            s.UsersCount += 1;

            if (await _platformRepository.SaveChangesAsync())
            {
                return true;
            }
            else
            {
                Console.WriteLine("Unhandled error in IncreaseUsersCount");
                return false;
            }
        }
    }
}
