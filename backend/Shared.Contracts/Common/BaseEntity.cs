using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Common
{
    /// <summary>
    /// Base class for domain entities providing common identity, audit timestamps, and soft-delete support.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>Gets or sets the unique identifier for the entity.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the entity was created.</summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;

        /// <summary>Gets or sets the UTC timestamp when the entity was last modified.</summary>
        public DateTime Modified { get; set; } = DateTime.UtcNow;

        /// <summary>Gets or sets a value indicating whether the entity is soft-deleted.</summary>
        public bool IsDeleted { get; set; } = false;
    }
}