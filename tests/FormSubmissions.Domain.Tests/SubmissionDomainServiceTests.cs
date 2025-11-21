using FormSubmissions.Domain.Aggregates.FormAggregate;
using FormSubmissions.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace FormSubmissions.Domain.Tests;

public sealed class SubmissionDomainServiceTests
{
    private static FormDefinition BuildForm()
    {
        var fields = new List<FormFieldDefinition>
        {
            new("name", "Name", FormFieldType.Text, true, null, null, null, null),
            new("email", "Email", FormFieldType.Text, true, null, "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$", null, null),
            new("role", "Role", FormFieldType.Dropdown, true, new[] { "dev", "qa" }, null, null, null),
            new("agree", "Agree", FormFieldType.Checkbox, true, null, null, null, null),
            new("age", "Age", FormFieldType.Number, false, null, null, 0, 130)
        };
        return FormDefinition.Create("Any", fields);
    }

    private static Dictionary<string, JsonElement> Payload(object o)
    {
        var json = JsonSerializer.Serialize(o);
        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
               ?? new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithValidPayload_DoesNotThrow()
    {
        var form = BuildForm();
        var payload = Payload(new
        {
            name = "Alice",
            email = "alice@test.com",
            role = "dev",
            agree = true,
            age = 33
        });

        var svc = new SubmissionDomainService();
        svc.Validate(form, payload);
    }

    [Fact]
    public void Validate_MissingRequiredField_Throws()
    {
        var form = BuildForm();
        var payload = Payload(new
        {
            email = "alice@test.com",
            role = "dev",
            agree = true
        });

        var svc = new SubmissionDomainService();
        Assert.Throws<ArgumentException>(() => svc.Validate(form, payload));
    }

    [Fact]
    public void Validate_EmptyRequiredString_Throws()
    {
        var form = BuildForm();
        var payload = Payload(new
        {
            name = " ",
            email = "alice@test.com",
            role = "dev",
            agree = true
        });

        var svc = new SubmissionDomainService();
        Assert.Throws<ArgumentException>(() => svc.Validate(form, payload));
    }

    [Fact]
    public void Validate_InvalidDropdownOption_Throws()
    {
        var form = BuildForm();
        var payload = Payload(new
        {
            name = "Alice",
            email = "alice@test.com",
            role = "manager",
            agree = true
        });

        var svc = new SubmissionDomainService();
        Assert.Throws<ArgumentException>(() => svc.Validate(form, payload));
    }

    [Fact]
    public void Validate_InvalidPattern_Throws()
    {
        var form = BuildForm();
        var payload = Payload(new
        {
            name = "Alice",
            email = "not-an-email",
            role = "dev",
            agree = true
        });

        var svc = new SubmissionDomainService();
        Assert.Throws<ArgumentException>(() => svc.Validate(form, payload));
    }

    [Fact]
    public void Validate_CheckboxNotBoolean_Throws()
    {
        var form = BuildForm();
        var payload = Payload(new
        {
            name = "Alice",
            email = "alice@test.com",
            role = "dev",
            agree = "yes"
        });

        var svc = new SubmissionDomainService();
        Assert.Throws<ArgumentException>(() => svc.Validate(form, payload));
    }

    [Fact]
    public void Validate_NumberBelowMin_Throws()
    {
        var form = BuildForm();
        var payload = Payload(new
        {
            name = "Alice",
            email = "alice@test.com",
            role = "dev",
            agree = true,
            age = -1
        });

        var svc = new SubmissionDomainService();
        Assert.Throws<ArgumentException>(() => svc.Validate(form, payload));
    }

    [Fact]
    public void Validate_NumberAboveMax_Throws()
    {
        var form = BuildForm();
        var payload = Payload(new
        {
            name = "Alice",
            email = "alice@test.com",
            role = "dev",
            agree = true,
            age = 200
        });

        var svc = new SubmissionDomainService();
        Assert.Throws<ArgumentException>(() => svc.Validate(form, payload));
    }

    [Fact]
    public void Validate_ExtraFieldsAllowed_DoesNotThrow()
    {
        var form = BuildForm();
        var payload = Payload(new
        {
            name = "Alice",
            email = "alice@test.com",
            role = "dev",
            agree = true,
            extra1 = "x",
            extra2 = 123
        });

        var svc = new SubmissionDomainService();
        svc.Validate(form, payload);
    }

    [Fact]
    public void Validate_MissingOptionalField_DoesNotThrow()
    {
        var form = BuildForm();
        var payload = Payload(new
        {
            name = "Alice",
            email = "alice@test.com",
            role = "dev",
            agree = true
        });

        var svc = new SubmissionDomainService();
        svc.Validate(form, payload);
    }
}
