using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Tasks.Command.UpdateCommand
{
    public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Task ID is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.")
                .Must(title => !string.IsNullOrWhiteSpace(title?.Trim()))
                .WithMessage("Title cannot be empty or whitespace.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.DueDate)
                .Must(d => d == null || d.Value.Date > DateTime.UtcNow.Date)
                .WithMessage("Due date must be in the future.");
        }
    }
}
