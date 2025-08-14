using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Tasks.Command.CompleteTaskCommand
{
    public class CompleteTaskCommandValidator : AbstractValidator<CompleteTaskCommand>
    {
        public CompleteTaskCommandValidator()
        {
            // Ensure task ID is provided
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Task ID is required.");

            // Ensure User ID is provided
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            // Ensure IsManager flag is not null (optional, depending on your setup)
            RuleFor(x => x.IsManager)
                .NotNull()
                .WithMessage("IsManager value must be specified.");
        }
    }
}
