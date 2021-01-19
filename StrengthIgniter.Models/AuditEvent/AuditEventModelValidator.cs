using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.AuditEvent
{
    internal class AuditEventModelValidator : AbstractValidator<AuditEventModel>
    {
        public AuditEventModelValidator()
        {
            RuleFor(x => x.EventType)
                .NotEmpty()
                .WithMessage("EventType is required.")
                .WithErrorCode("EventType.Required");
        }
    }
}
