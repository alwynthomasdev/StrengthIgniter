using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.SecurityQuestion
{
    public class SecurityQuestionModel
    {
        public int SecurityQuestionId { get; set; }
        public string QuestionText { get; set; }
    }
}
