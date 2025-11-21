using FormSubmissions.BuildingBlocks.Domain;
using FormSubmissions.Domain.ValueObjects;

namespace FormSubmissions.Domain.Events;

public sealed record AttachmentAddedDomainEvent(SubmissionId SubmissionId, string StorageKey) : DomainEvent;
