using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Request;
using UserService.Application.DTOs.Response;
using UserService.Application.Interfaces;
using UserService.Domain.Models;

namespace UserService.Application.Service
{
    public class UserService : IUserService
    {
        private readonly string[] arr = ["Furry", "Diddy", "Epic", "Giga", "Bobby", "Bot", "Big", "King", "Hacker", "Clanker", "Nerd", "Wise"];

        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> CreateProfile(Guid id)
        {
            Profile? profile = null;

            if (await _userRepository.CreateProfile(new Profile
            {
                Id = id,
                Name = arr[Random.Shared.Next(arr.Length)] + "Bob-" + Random.Shared.Next(1000, 10000).ToString(),
                TasksSolved = "",
                ArticlesRead = "",
                IsPrivate = true,
                Created = DateTime.UtcNow,
                ArticlesReadCount = 0,
                EasyTasksSolvedCount = 0,
                MediumTasksSolvedCount = 0,
                HardTasksSolvedCount = 0,
                LastActivity = DateTime.UtcNow,
                LastTaskSolvedDate = null,
                LastTaskSolvedId = -1,
                AdminComment = ""
            })) {
                Console.WriteLine($"Profile created. Account id = {id}");
                return true;
            }
            else
            {
                Console.WriteLine($"Unknown internal error in CreateProfile. Account id = {id}");
                return false;
            }
        }

        public async Task<ReadArticleResponse> ReadArticle(string id, int articleId)
        {
            Guid guid;

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out guid))
                return new ReadArticleResponse { Message = "Invalid account id", Status = 400 };

            Profile? profile = await _userRepository.GetProfileByAccountId(guid);

            if (profile == null)
            {
                Console.WriteLine($"Profile not found. Id = {id}");
                return new ReadArticleResponse { Message = "Profile not found", Status = 404 };
            }

            if (articleId < 0 || articleId > 2000)
            {
                Console.WriteLine($"{profile.Id} is trying to increase the amount of read articles");
                return new ReadArticleResponse { Message = "Bad article id", Status = 400 };
            }

            if (IsNumInSequence(profile.ArticlesRead, articleId.ToString()))
                return new ReadArticleResponse { Message = "Article has already been read", Status = 400 };

            profile.ArticlesRead = InsertNumber(profile.ArticlesRead, articleId.ToString());

            profile.ArticlesReadCount += 1;

            profile.LastActivity = DateTime.UtcNow;

            if (await _userRepository.SaveChangesAsync())
            {
                return new ReadArticleResponse { Message = "Success!", Status = 200 };
            }
            else
            {
                return new ReadArticleResponse { Message = "Unknown internal error", Status = 500 };
            }
        }

        public async Task<bool> SolveTask(Guid id, int taskId, string difficulty)
        {
            Profile? profile = await _userRepository.GetProfileByAccountId(id);

            if (profile == null)
            {
                Console.WriteLine($"Profile not found. Id = {id}");
                return false;
            }

            if (IsNumInSequence(profile.TasksSolved, taskId.ToString()))
                return true;

            profile.TasksSolved = InsertNumber(profile.TasksSolved, taskId.ToString());

            switch (difficulty.ToLower().Trim())
            {
                case "easy":
                    profile.EasyTasksSolvedCount += 1;
                    break;
                case "medium":
                    profile.MediumTasksSolvedCount += 1;
                    break;
                case "hard":
                    profile.HardTasksSolvedCount += 1;
                    break;
            }

            profile.LastActivity = DateTime.UtcNow;
            profile.LastTaskSolvedDate = DateTime.UtcNow;
            profile.LastTaskSolvedId = taskId;

            if (await _userRepository.SaveChangesAsync())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string InsertNumber(string s, string ins)
        {
            if (s == "")
                return ins;

            int a = int.Parse(ins);
            int b = a;
            string result = "";

            foreach (string part in s.Split(','))
            {
                int k = part.IndexOf('-');

                int x = k == -1 ? int.Parse(part) : int.Parse(part.Substring(0, k));
                int y = k == -1 ? x : int.Parse(part.Substring(k + 1));

                if (y < a - 1)
                {
                    result += (x == y ? x.ToString() : x + "-" + y) + ",";
                }
                else if (x > b + 1)
                {
                    result += (a == b ? a.ToString() : a + "-" + b) + ",";
                    a = x;
                    b = y;
                }
                else
                {
                    a = Math.Min(a, x);
                    b = Math.Max(b, y);
                }
            }

            return result + (a == b ? a.ToString() : a + "-" + b);
        }

        private static bool IsNumInSequence(string s, string num)
        {
            int n = int.Parse(num);

            foreach (string part in s.Split(","))
            {
                int k = part.IndexOf('-');

                if (k == -1)
                {
                    if (part == num)
                        return true;
                }
                else
                {
                    int x = int.Parse(part.Substring(0, k));
                    int y = int.Parse(part.Substring(k + 1));

                    if (n >= x && n <= y)
                        return true;
                }
            }

            return false;
        }

        public async Task<EditProfileResponse> EditProfile(string id, EditProfileRequest request)
        {

            Guid guid;

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out guid))
                return new EditProfileResponse { Message = "Invalid account id", Status = 400 };

            Profile? profile = await _userRepository.GetProfileByAccountId(guid);

            if (profile == null)
            {
                Console.WriteLine($"Profile not found. Id = {guid}");
                return new EditProfileResponse { Message = "Profile not found", Status = 404 };
            }

            try
            {
                request.Name ??= profile.Name;
                request.IsPrivate ??= profile.IsPrivate;

                profile.Name = request.Name;
                profile.IsPrivate = (bool)request.IsPrivate;

                await _userRepository.SaveChangesAsync();

                return new EditProfileResponse { Profile = profile, Message = "Success", Status = 200 };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new EditProfileResponse { Message = "Unknown internal error in EditArticle", Status = 500 };
            }
        }

        public async Task<GetProfileResponse> GetProfile(string id)
        {
            Guid guid;

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out guid))
                return new GetProfileResponse { Message = "Invalid account id", Status = 400 };

            Profile? profile = await _userRepository.GetProfileByAccountId(guid);

            if (profile == null)
            {
                Console.WriteLine($"Profile not found. Id = {id}");
                return new GetProfileResponse { Message = "Profile not found", Status = 404 };
            }

            return new GetProfileResponse { Profile = profile, Message = "Success", Status = 200 };
        }
    }
}
