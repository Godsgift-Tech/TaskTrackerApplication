using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Tasks.Queries.GetTaskById
{
    public class GetTaskCompletionReportQueryValidator
     : AbstractValidator<GetTaskCompletionReportQuery>
    {
        public GetTaskCompletionReportQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
