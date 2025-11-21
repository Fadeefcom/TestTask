using FormSubmissions.BuildingBlocks.Domain;
using FormSubmissions.Domain.ValueObjects;

namespace FormSubmissions.Domain.Events;

public sealed record SubmissionCreatedDomainEvent(SubmissionId SubmissionId, FormId FormId) : DomainEvent;
