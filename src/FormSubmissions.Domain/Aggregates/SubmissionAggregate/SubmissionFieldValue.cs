using System.Text.Json;
using FormSubmissions.Domain.ValueObjects;

namespace FormSubmissions.Domain.Aggregates.SubmissionAggregate;

public readonly record struct SubmissionFieldValue(FieldKey Key, JsonElement Value);
