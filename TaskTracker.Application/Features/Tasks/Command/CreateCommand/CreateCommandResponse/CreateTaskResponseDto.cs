using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Tasks.Command.CreateCommand.CreateCommandResponse
{
    public class CreateTaskResponseDto
    {
        public string Message { get; set; } = default!;
     
    }
}
