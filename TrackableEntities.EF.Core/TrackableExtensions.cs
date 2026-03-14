using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core
{
    /// <summary>
    /// Extension methods for classes implementing ITrackable.
    /// </summary>
    public static class TrackableExtensions
    {
        /// <summary>
        /// Convert TrackingState to EntityState.
        /// </summary>
        /// <param name="state">Trackable entity state</param>
        /// <returns>EF entity state</returns>
        public static EntityState ToEntityState(this TrackingState state)
        {
            switch (state)
            {
                case TrackingState.Added:
                    return EntityState.Added;
                case TrackingState.Modified:
                    return EntityState.Modified;
                case TrackingState.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }

        /// <summary>
        /// Convert EntityState to TrackingState.
        /// </summary>
        /// <param name="state">EF entity state</param>
        /// <returns>Trackable entity state</returns>
        public static TrackingState ToTrackingState(this EntityState state)
        {
            switch (state)
            {
                case EntityState.Added:
                    return TrackingState.Added;
                case EntityState.Modified:
                    return TrackingState.Modified;
                case EntityState.Deleted:
                    return TrackingState.Deleted;
                default:
                    return TrackingState.Unchanged;
            }
        }

        /// <summary>
        /// Sets an original value for a modified property.
        /// </summary>
        /// <param name="trackable">The trackable entity</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="originalValue">The original value before modification</param>
        public static void SetOriginalValue(this ITrackable trackable, string propertyName, object originalValue)
        {
            if (trackable.OriginalValues == null)
                trackable.OriginalValues = new Dictionary<string, object>();

            trackable.OriginalValues[propertyName] = originalValue;
        }

        /// <summary>
        /// Gets the original value for a property if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the property value</typeparam>
        /// <param name="trackable">The trackable entity</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="value">The original value if it exists</param>
        /// <returns>True if the original value exists, false otherwise</returns>
        public static bool GetOriginalValue<T>(this ITrackable trackable, string propertyName, out T value)
        {
            value = default;
            if (trackable.OriginalValues?.TryGetValue(propertyName, out var obj) == true)
            {
                value = (T)obj;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a change description showing old and new values for all modified properties.
        /// </summary>
        /// <param name="trackable">The trackable entity</param>
        /// <returns>A dictionary of property changes</returns>
        public static IDictionary<string, PropertyChange> GetPropertyChanges(this ITrackable trackable)
        {
            var changes = new Dictionary<string, PropertyChange>();
            if (trackable.ModifiedProperties == null) return changes;

            foreach (var propertyName in trackable.ModifiedProperties)
            {
                object originalValue = null;
                object currentValue = null;

                if (trackable.OriginalValues?.TryGetValue(propertyName, out originalValue) == true)
                {
                    // Get current value via reflection
                    var property = trackable.GetType().GetProperty(propertyName);
                    if (property != null)
                        currentValue = property.GetValue(trackable);
                }

                changes[propertyName] = new PropertyChange
                {
                    PropertyName = propertyName,
                    OriginalValue = originalValue,
                    CurrentValue = currentValue
                };
            }

            return changes;
        }
    }
}
