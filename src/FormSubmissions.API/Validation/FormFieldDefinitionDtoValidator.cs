using FluentValidation;
using FormSubmissions.API.Models;
using System.Text.RegularExpressions;

namespace FormSubmissions.API.Validation;

public sealed class FormFieldDefinitionDtoValidator : AbstractValidator<FormFieldDefinitionDto>
{
    private static readonly string[] AllowedTypes =
    {
        "Text", "Dropdown", "Date", "Radio", "Checkbox", "Number"
    };

    public FormFieldDefinitionDtoValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .Must(k => k.Trim() == k)
            .WithMessage("Key must not contain leading/trailing spaces");

        RuleFor(x => x.Label)
            .NotEmpty()
            .Must(l => l.Trim() == l);

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => AllowedTypes.Any(a => string.Equals(a, t, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Type is not supported");

        When(x => string.Equals(x.Type, "Dropdown", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(x.Type, "Radio", StringComparison.OrdinalIgnoreCase), () =>
                  {
                      RuleFor(x => x.Options)
                          .NotNull()
                          .Must(o => o!.Count > 0)
                          .WithMessage("Options are required for Dropdown/Radio");

                      RuleForEach(x => x.Options!)
                          .NotEmpty()
                          .Must(o => o.Trim() == o);

                      RuleFor(x => x.Options!)
                          .Must(o => o.Distinct(StringComparer.OrdinalIgnoreCase).Count() == o.Count)
                          .WithMessage("Options must be unique");
                  });

        When(x => !string.Equals(x.Type, "Dropdown", StringComparison.OrdinalIgnoreCase) &&
                  !string.Equals(x.Type, "Radio", StringComparison.OrdinalIgnoreCase), () =>
                  {
                      RuleFor(x => x.Options)
                          .Must(o => o == null || o.Count == 0)
                          .WithMessage("Options are allowed only for Dropdown/Radio");
                  });

        When(x => string.Equals(x.Type, "Text", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.Pattern)
                .Must(p => p == null || IsValidRegex(p))
                .WithMessage("Pattern is not a valid regex");
        });

        When(x => !string.Equals(x.Type, "Text", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.Pattern)
                .Must(p => p == null)
                .WithMessage("Pattern is allowed only for Text");
        });

        When(x => string.Equals(x.Type, "Number", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x)
                .Must(f => !(f.Min.HasValue ^ f.Max.HasValue))
                .WithMessage("Min and Max should be set together");

            RuleFor(x => x)
                .Must(f => !f.Min.HasValue || !f.Max.HasValue || f.Min.Value <= f.Max.Value)
                .WithMessage("Min must be <= Max");
        });

        When(x => !string.Equals(x.Type, "Number", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.Min)
                .Must(m => m == null)
                .WithMessage("Min is allowed only for Number");

            RuleFor(x => x.Max)
                .Must(m => m == null)
                .WithMessage("Max is allowed only for Number");
        });
    }

    private static bool IsValidRegex(string pattern)
    {
        try
        {
            _ = new Regex(pattern);
            return true;
        }
        catch
        {
            return false;
        }
    }
}