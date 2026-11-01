using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.Application.DTOs.Request
{
    public enum TaskSortBy
    {
        SortOrder,
        Difficulty,
        Created
    }

    public class GetTasksRequest
    {
        [Range(1, 100000)]
        public int page { get; set; } = 1;

        [MaxLength(150)]
        public string? search { get; set; }

        public string? difficulty { get; set; }

        public string? topic { get; set; }

        public bool hideSolved { get; set; }

        [MaxLength(10000)]
        public string? solvedTasks { get; set; }

        public TaskSortBy sortBy { get; set; } = TaskSortBy.Created;

        public bool sortDescending { get; set; } = true;
    }
}
