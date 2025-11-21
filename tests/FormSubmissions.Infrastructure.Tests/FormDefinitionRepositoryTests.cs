using FormSubmissions.Domain.Aggregates.FormAggregate;
using FormSubmissions.Infrastructure.Persistence.InMemoryStore;
using FormSubmissions.Infrastructure.Persistence.Repositories;
using Xunit;

namespace FormSubmissions.Infrastructure.Tests;

public sealed class FormDefinitionRepositoryTests
{
    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsSameForm()
    {
        var store = new InMemoryFormStore();
        var repo = new FormDefinitionRepository(store);

        var form = FormDefinition.Create(
            "Contact",
            new List<FormFieldDefinition>
            {
                new("name", "Name", FormFieldType.Text, true, null, null, null, null)
            }
        );

        await repo.AddAsync(form, CancellationToken.None);

        var loaded = await repo.GetByIdAsync(form.Id.Value, CancellationToken.None);

        Assert.NotNull(loaded);
        Assert.Equal(form.Id.Value, loaded!.Id.Value);
        Assert.Equal("Contact", loaded.Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSortedByName()
    {
        var store = new InMemoryFormStore();
        var repo = new FormDefinitionRepository(store);

        var a = FormDefinition.Create(
            "A Form",
            new List<FormFieldDefinition>
            {
                new("a", "A", FormFieldType.Text, true, null, null, null, null)
            }
        );

        var b = FormDefinition.Create(
            "B Form",
            new List<FormFieldDefinition>
            {
                new("b", "B", FormFieldType.Text, true, null, null, null, null)
            }
        );

        await repo.AddAsync(b, CancellationToken.None);
        await repo.AddAsync(a, CancellationToken.None);

        var all = await repo.GetAllAsync(CancellationToken.None);

        Assert.Equal(2, all.Count);
        Assert.Equal("A Form", all[0].Name);
        Assert.Equal("B Form", all[1].Name);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        var store = new InMemoryFormStore();
        var repo = new FormDefinitionRepository(store);

        var loaded = await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(loaded);
    }
}
