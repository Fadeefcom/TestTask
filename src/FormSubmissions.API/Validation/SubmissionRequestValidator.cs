using FluentValidation;
using FormSubmissions.API.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FormSubmissions.API.Validation;

public sealed class SubmissionRequestValidator : AbstractValidator<CreateSubmissionRequest>
{
    private static readonly Regex EmailRegex = new("^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public SubmissionRequestValidator()
    {
        RuleFor(x => x.FormId)
            .NotEmpty();

        RuleFor(x => x.Payload)
            .NotNull()
            .Must(p => p.Count > 0)
            .WithMessage("Payload must contain at least one field");

        RuleForEach(x => x.Payload)
            .Must(kv => !string.IsNullOrWhiteSpace(kv.Key))
            .WithMessage("Payload keys must be non-empty");

        RuleForEach(x => x.Payload)
            .Must(kv => kv.Value.ValueKind != JsonValueKind.Undefined)
            .WithMessage("Payload values must be defined");

        RuleFor(x => x.Payload)
           .Must(EmailIfPresentIsValid)
           .WithMessage("Email is invalid");
    }

    private static bool EmailIfPresentIsValid(IReadOnlyDictionary<string, JsonElement> payload)
    {
        if (!payload.TryGetValue("email", out var emailEl))
            return true;

        if (emailEl.ValueKind != JsonValueKind.String)
            return false;

        var email = emailEl.GetString();
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }
}
