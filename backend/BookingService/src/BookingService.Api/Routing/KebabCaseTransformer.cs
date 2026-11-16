using System.Text.RegularExpressions;

namespace BookingService.Api.Routing
{
    /// <summary>
    /// Transforms route tokens into lowercase and kebab-case for consistent REST API URL formatting.
    /// </summary>
    public class KebabCaseTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;
            return Regex.Replace(value.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
        }
    }
}