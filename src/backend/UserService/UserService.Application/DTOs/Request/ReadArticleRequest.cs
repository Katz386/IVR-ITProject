using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.DTOs.Request
{
    public class ReadArticleRequest
    {
        [Range(1, 2000000, ErrorMessage = "Id is too big")]
        public int Id { get; set; }
    }
}
