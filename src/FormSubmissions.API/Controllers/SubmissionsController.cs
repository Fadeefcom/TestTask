using FormSubmissions.API.Mapping;
using FormSubmissions.API.Models;
using FormSubmissions.Domain.Aggregates.SubmissionAggregate;
using FormSubmissions.Domain.Interfaces;
using FormSubmissions.Domain.Services;
using FormSubmissions.Domain.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace FormSubmissions.API.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionRepository _submissions;
    private readonly IFormDefinitionRepository _forms;
    private readonly SubmissionDomainService _domainService;

    public SubmissionsController(
        ISubmissionRepository submissions,
        IFormDefinitionRepository forms,
        SubmissionDomainService domainService)
    {
        _submissions = submissions;
        _forms = forms;
        _domainService = domainService;
    }

    [HttpPost]
    public async Task<ActionResult<SubmissionDto>> Create([FromBody] CreateSubmissionRequest request, CancellationToken ct)
    {
        var form = await _forms.GetByIdAsync(request.FormId, ct);
        if (form == null)
            return NotFound(new { error = "Form not found" });

        _domainService.Validate(form, request.Payload);

        var submission = Submission.Create(form.Id, request.Payload);
        await _submissions.AddAsync(submission, ct);

        return CreatedAtAction(nameof(GetById), new { id = submission.Id.Value }, ApiMappingsProfile.ToDto(submission));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SubmissionDto>> GetById(Guid id, CancellationToken ct)
    {
        var sub = await _submissions.GetByIdAsync(id, ct);
        if (sub == null)
            return NotFound();

        return Ok(ApiMappingsProfile.ToDto(sub));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SubmissionDto>>> Search([FromQuery] SearchSubmissionsRequest request, CancellationToken ct)
    {
        var spec = new SubmissionSearchSpecification(request.FormId, request.Query, request.From, request.To);
        var items = await _submissions.SearchAsync(spec, ct);
        return Ok(items.Select(ApiMappingsProfile.ToDto).ToList());
    }
}
