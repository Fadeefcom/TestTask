using FormSubmissions.API.Mapping;
using FormSubmissions.API.Models;
using FormSubmissions.Domain.Aggregates.FormAggregate;
using FormSubmissions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FormSubmissions.API.Controllers;

[ApiController]
[Route("api/forms")]
public class FormsController : ControllerBase
{
    private readonly IFormDefinitionRepository _forms;

    public FormsController(IFormDefinitionRepository forms)
    {
        _forms = forms;
    }

    [HttpPost]
    public async Task<ActionResult<FormDefinitionDto>> Create([FromBody] CreateFormDefinitionRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var fields = request.Fields.Select(f => new FormFieldDefinition(
            f.Key,
            f.Label,
            Enum.Parse<FormFieldType>(f.Type, true),
            f.Required,
            f.Options,
            f.Pattern,
            f.Min,
            f.Max
        )).ToList();

        var form = FormDefinition.Create(request.Name, fields);
        await _forms.AddAsync(form, ct);
        return CreatedAtAction(nameof(GetById), new { id = form.Id.Value }, ApiMappingsProfile.ToDto(form));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FormDefinitionDto>>> GetAll(CancellationToken ct)
    {
        var items = await _forms.GetAllAsync(ct);
        return Ok(items.Select(ApiMappingsProfile.ToDto).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FormDefinitionDto>> GetById(Guid id, CancellationToken ct)
    {
        var form = await _forms.GetByIdAsync(id, ct);
        if (form == null)
            return NotFound();

        return Ok(ApiMappingsProfile.ToDto(form));
    }
}
