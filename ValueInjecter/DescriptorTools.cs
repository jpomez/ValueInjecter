﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Omu.ValueInjecter
{
    public static class DescriptorTools
    {
        /// <summary>
        /// Seek for a PropertyDescriptor within the collection by Name
        /// </summary>
        /// <returns>the search result or null if nothing was found</returns>
        public static PropertyInfo GetByName(this IEnumerable<PropertyInfo> collection, string name)
        {
            return collection.FirstOrDefault(prop => prop.Name == name);
        }

        /// <summary>
        /// Seek for a PropertyDescriptor within the collection by Name with option to ignore case
        /// </summary>
        /// <returns>search result or null if nothing was found</returns>
        public static PropertyInfo GetByName(this IEnumerable<PropertyInfo> collection, string name, bool ignoreCase)
        {
            return collection.FirstOrDefault(prop => prop.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        }

        /// <summary>
        /// Search for a PropertyDescriptor within the collection that is of a specific type T
        /// </summary>
        /// <returns>search result or null if nothing was found</returns>
        public static PropertyInfo GetByNameType<T>(this IEnumerable<PropertyInfo> collection, string name)
        {
            var p = collection.GetByName(name);
            if (p != null && p.PropertyType == typeof(T)) return p;
            return null;
        }
    }
}