using FormSubmissions.Domain.Aggregates.FormAggregate;
using Xunit;

namespace FormSubmissions.Domain.Tests;

public sealed class FormDefinitionTests
{
    [Fact]
    public void Create_WithValidFields_CreatesForm()
    {
        var fields = new List<FormFieldDefinition>
        {
            new("name", "Name", FormFieldType.Text, true, null, null, null, null),
            new("email", "Email", FormFieldType.Text, true, null, "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$", null, null),
            new("dob", "Birth date", FormFieldType.Date, false, null, null, null, null),
            new("role", "Role", FormFieldType.Dropdown, true, new[] { "dev", "qa" }, null, null, null),
            new("agree", "Agree", FormFieldType.Checkbox, true, null, null, null, null)
        };

        var form = FormDefinition.Create("Contact Form", fields);

        Assert.NotEqual(Guid.Empty, form.Id.Value);
        Assert.Equal("Contact Form", form.Name);
        Assert.Equal(1, form.Version.Value);
        Assert.Equal(5, form.Fields.Count);
    }

    [Fact]
    public void Create_WithEmptyName_Throws()
    {
        var fields = new List<FormFieldDefinition>
        {
            new("name", "Name", FormFieldType.Text, true, null, null, null, null)
        };

        Assert.Throws<ArgumentException>(() => FormDefinition.Create("  ", fields));
    }

    [Fact]
    public void Create_WithNoFields_Throws()
    {
        Assert.Throws<ArgumentException>(() => FormDefinition.Create("Any", new List<FormFieldDefinition>()));
    }

    [Fact]
    public void Create_WithDuplicateKeys_Throws()
    {
        var fields = new List<FormFieldDefinition>
        {
            new("name", "Name", FormFieldType.Text, true, null, null, null, null),
            new("name", "Name2", FormFieldType.Text, false, null, null, null, null)
        };

        Assert.Throws<ArgumentException>(() => FormDefinition.Create("Any", fields));
    }

    [Fact]
    public void BumpVersion_IncrementsVersion()
    {
        var fields = new List<FormFieldDefinition>
        {
            new("name", "Name", FormFieldType.Text, true, null, null, null, null)
        };

        var form = FormDefinition.Create("Any", fields);
        Assert.Equal(1, form.Version.Value);

        form.BumpVersion();
        Assert.Equal(2, form.Version.Value);
    }
}
