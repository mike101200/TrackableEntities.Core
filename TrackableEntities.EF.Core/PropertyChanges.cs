namespace TrackableEntities.EF.Core
{
    /// <summary>
    /// Represents a property change with original and current values.
    /// </summary>
    public class PropertyChange
    {
        /// <summary>
        /// The name of the property that changed.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The original value before modification.
        /// </summary>
        public object OriginalValue { get; set; }

        /// <summary>
        /// The current value after modification.
        /// </summary>
        public object CurrentValue { get; set; }

        /// <summary>
        /// Returns a string representation of the change.
        /// </summary>
        /// <returns>A string in the format "PropertyName: OriginalValue -> CurrentValue"</returns>
        public override string ToString() => $"{PropertyName}: {OriginalValue} -> {CurrentValue}";
    }
}