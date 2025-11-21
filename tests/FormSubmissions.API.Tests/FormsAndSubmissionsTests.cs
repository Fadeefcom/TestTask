using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace FormSubmissions.API.Tests;

public sealed class FormsAndSubmissionsTests
{
    [Fact]
    public async Task CreateForm_ThenGetById_ReturnsSameForm()
    {
        await using var factory = new ApiFactory();
        var client = await factory.CreateClientAsync();

        var createForm = new
        {
            name = "Test Form",
            fields = new object[]
            {
                new { key = "name", label = "Name", type = "Text", required = true },
                new { key = "email", label = "Email", type = "Text", required = true, pattern = "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$" },
                new { key = "dob", label = "Birth date", type = "Date", required = false },
                new { key = "role", label = "Role", type = "Dropdown", required = true, options = new[] { "dev", "qa" } },
                new { key = "agree", label = "Agree", type = "Checkbox", required = true }
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/forms", createForm);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdJson = await createResponse.Content.ReadAsStringAsync();
        using var createdDoc = JsonDocument.Parse(createdJson);
        var id = createdDoc.RootElement.GetProperty("id").GetGuid();

        var getResponse = await client.GetAsync($"/api/forms/{id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getJson = await getResponse.Content.ReadAsStringAsync();
        using var getDoc = JsonDocument.Parse(getJson);

        Assert.Equal(id, getDoc.RootElement.GetProperty("id").GetGuid());
        Assert.Equal("Test Form", getDoc.RootElement.GetProperty("name").GetString());
        Assert.True(getDoc.RootElement.GetProperty("fields").GetArrayLength() >= 5);
    }

    [Fact]
    public async Task CreateSubmission_ThenSearchByQuery_FindsSubmission()
    {
        await using var factory = new ApiFactory();
        var client = await factory.CreateClientAsync();

        var createForm = new
        {
            name = "Search Form",
            fields = new object[]
            {
                new { key = "name", label = "Name", type = "Text", required = true },
                new { key = "age", label = "Age", type = "Number", required = false, min = 0, max = 130 },
                new { key = "topic", label = "Topic", type = "Dropdown", required = true, options = new[] { "a", "b" } },
                new { key = "date", label = "Date", type = "Date", required = true },
                new { key = "agree", label = "Agree", type = "Checkbox", required = true }
            }
        };

        var formResp = await client.PostAsJsonAsync("/api/forms", createForm);
        Assert.Equal(HttpStatusCode.Created, formResp.StatusCode);

        var formJson = await formResp.Content.ReadAsStringAsync();
        using var formDoc = JsonDocument.Parse(formJson);
        var formId = formDoc.RootElement.GetProperty("id").GetGuid();

        var uniqueName = "Alice Johnson " + Guid.NewGuid().ToString("N");
        var createSubmission = new
        {
            formId,
            payload = new
            {
                name = uniqueName,
                age = 33,
                topic = "a",
                date = "2025-11-21",
                agree = true
            }
        };

        var subResp = await client.PostAsJsonAsync("/api/submissions", createSubmission);
        Assert.Equal(HttpStatusCode.Created, subResp.StatusCode);

        var searchResp = await client.GetAsync($"/api/submissions?query=alice&formId={formId}");
        Assert.Equal(HttpStatusCode.OK, searchResp.StatusCode);

        var searchJson = await searchResp.Content.ReadAsStringAsync();
        using var searchDoc = JsonDocument.Parse(searchJson);

        Assert.Equal(JsonValueKind.Array, searchDoc.RootElement.ValueKind);
        Assert.True(searchDoc.RootElement.GetArrayLength() >= 1);

        var found = searchDoc.RootElement.EnumerateArray()
            .Any(e =>
                e.GetProperty("formId").GetGuid() == formId &&
                e.GetProperty("payload").GetProperty("name").GetString() == uniqueName
            );

        Assert.True(found);
    }

    [Fact]
    public async Task CreateSubmission_WithMissingRequiredField_ReturnsServerError()
    {
        await using var factory = new ApiFactory();
        var client = await factory.CreateClientAsync();

        var createForm = new
        {
            name = "Validation Form",
            fields = new object[]
            {
                new { key = "name", label = "Name", type = "Text", required = true },
                new { key = "agree", label = "Agree", type = "Checkbox", required = true },
                new { key = "topic", label = "Topic", type = "Dropdown", required = true, options = new[] { "x" } },
                new { key = "date", label = "Date", type = "Date", required = true },
                new { key = "email", label = "Email", type = "Text", required = false }
            }
        };

        var formResp = await client.PostAsJsonAsync("/api/forms", createForm);
        Assert.Equal(HttpStatusCode.Created, formResp.StatusCode);

        var formJson = await formResp.Content.ReadAsStringAsync();
        using var formDoc = JsonDocument.Parse(formJson);
        var formId = formDoc.RootElement.GetProperty("id").GetGuid();

        var badSubmission = new
        {
            formId,
            payload = new
            {
                agree = true,
                topic = "x",
                date = "2025-11-21"
            }
        };

        var subResp = await client.PostAsJsonAsync("/api/submissions", badSubmission);
        Assert.Equal(HttpStatusCode.InternalServerError, subResp.StatusCode);
    }

    [Fact]
    public async Task SearchSubmissions_ByDateRange_FiltersCorrectly()
    {
        await using var factory = new ApiFactory();
        var client = await factory.CreateClientAsync();

        var createForm = new
        {
            name = "Date Filter Form",
            fields = new object[]
            {
                new { key = "name", label = "Name", type = "Text", required = true },
                new { key = "date", label = "Date", type = "Date", required = true },
                new { key = "topic", label = "Topic", type = "Dropdown", required = true, options = new[] { "a" } },
                new { key = "agree", label = "Agree", type = "Checkbox", required = true },
                new { key = "email", label = "Email", type = "Text", required = false }
            }
        };

        var formResp = await client.PostAsJsonAsync("/api/forms", createForm);
        Assert.Equal(HttpStatusCode.Created, formResp.StatusCode);

        var formJson = await formResp.Content.ReadAsStringAsync();
        using var formDoc = JsonDocument.Parse(formJson);
        var formId = formDoc.RootElement.GetProperty("id").GetGuid();

        var s1 = new { formId, payload = new { name = "One " + Guid.NewGuid().ToString("N"), date = "2025-11-21", topic = "a", agree = true } };
        var s2 = new { formId, payload = new { name = "Two " + Guid.NewGuid().ToString("N"), date = "2025-11-21", topic = "a", agree = true } };

        var r1 = await client.PostAsJsonAsync("/api/submissions", s1);
        var r2 = await client.PostAsJsonAsync("/api/submissions", s2);

        Assert.Equal(HttpStatusCode.Created, r1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, r2.StatusCode);

        var from = DateTimeOffset.UtcNow.AddMinutes(-1).ToString("O");
        var to = DateTimeOffset.UtcNow.AddMinutes(1).ToString("O");

        var searchResp = await client.GetAsync($"/api/submissions?formId={formId}&from={Uri.EscapeDataString(from)}&to={Uri.EscapeDataString(to)}");
        Assert.Equal(HttpStatusCode.OK, searchResp.StatusCode);

        var searchJson = await searchResp.Content.ReadAsStringAsync();
        using var searchDoc = JsonDocument.Parse(searchJson);

        Assert.True(searchDoc.RootElement.GetArrayLength() >= 2);
        Assert.All(searchDoc.RootElement.EnumerateArray(), e =>
        {
            Assert.Equal(formId, e.GetProperty("formId").GetGuid());
        });
    }
}
