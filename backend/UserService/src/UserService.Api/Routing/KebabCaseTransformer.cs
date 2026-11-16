using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace UserService.Api.Routing
{
    /// <summary>
    /// Transforms route tokens into lowercase and kebab-case for consistent REST API URL formatting.
    /// </summary>
    public class KebabCaseTransformer : IOutboundParameterTransformer
    {
        /// <summary>
        /// Converts PascalCase or camelCase values into hyphen-separated lowercase format (e.g., UserManagement -> user-management).
        /// </summary>
        /// <param name="value">The route parameter value.</param>
        /// <returns>The transformed kebab-case string, or null if the value is null.</returns>
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;

            return Regex.Replace(value.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
        }
    }
}