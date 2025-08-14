using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Features.Common.Interfaces;

namespace TaskTracker.Application.Features.Tasks.Command.DeleteCommand
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemoryCache _cache;

        public DeleteTaskCommandHandler(ITaskRepository taskRepository, IMemoryCache cache)
        {
            _taskRepository = taskRepository;
            _cache = cache;
        }

        public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _taskRepository.DeleteAsync(request.Id, request.UserId);

            if (deleted)
            {
                _cache.Remove($"Task_{request.UserId}_{request.Id}");
            }

            return deleted;
        }
    }

}
