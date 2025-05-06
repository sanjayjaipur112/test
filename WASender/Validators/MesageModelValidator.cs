using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaAutoReplyBot.Models;
using WASender;
using WASender.Models;


namespace WASender.Validators
{
    public class MesageModelValidator : AbstractValidator<MesageModel>
    {
        public MesageModelValidator()
        {
            RuleFor(x => x.longMessage).NotEmpty().When(x=>x.buttons !=null && x.buttons.Count() >0 ).WithMessage(Strings.MessageShouldNotbeEmpty);
        }
    }
}
