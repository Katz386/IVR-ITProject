using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserService.Application.DTOs.Request
{
    public class EditProfileRequest
    {
        [MaxLength(150, ErrorMessage = "Too long")]
        public string? Name { get; set; }
        public bool? IsPrivate { get; set; }
    }
}
