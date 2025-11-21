using FluentValidation;
using FormSubmissions.API.Models;

namespace FormSubmissions.API.Validation;

public sealed class CreateFormDefinitionRequestValidator : AbstractValidator<CreateFormDefinitionRequest>
{
    public CreateFormDefinitionRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Must(n => n.Trim() == n);

        RuleFor(x => x.Fields)
            .NotNull()
            .Must(f => f.Count > 0)
            .WithMessage("Fields must contain at least one item");

        RuleForEach(x => x.Fields)
            .SetValidator(new FormFieldDefinitionDtoValidator());

        RuleFor(x => x.Fields)
            .Must(HaveUniqueKeys)
            .WithMessage("Field keys must be unique");
    }

    private static bool HaveUniqueKeys(IReadOnlyList<FormFieldDefinitionDto> fields)
    {
        return fields
            .Select(f => f.Key)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() == fields.Count;
    }
}
