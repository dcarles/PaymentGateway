namespace PaymentGateway.Data.Entities
{
    /// <summary>
    /// Base class to to all entities
    /// </summary>
    public abstract class BaseEntity
    {

        /// <summary>
        /// Utc format
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}