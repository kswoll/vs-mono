using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using MonoProgram.Package.Projects;
using MonoProgram.Package.PropertyPages;

namespace MonoProgram.Package.ProgramProperties
{
    public class MonoPropertiesStore : IDisposable, IPropertyStore
    {
        private readonly List<MonoProgramFlavorCfg> configs = new List<MonoProgramFlavorCfg>();
        private bool disposed;

        public event Action StoreChanged;

        /// <summary>
        /// Use the data passed in to initialize the Properties.
        /// </summary>
        /// <param name="dataObjects">
        /// This is normally only one our configuration object, which means that there will be only one elements in configs.
        /// </param>
        public void Initialize(object[] dataObjects)
        {
            // If we are editing multiple configuration at once, we may get multiple objects.
            foreach (var dataObject in dataObjects)
            {
                if (dataObject is IVsCfg)
                {
                    // This should be our configuration object, so retrive the specific
                    // class so we can access its properties.
                    var config = MonoProgramFlavorCfg.GetMonoProgramFlavorCfgFromVsCfg((IVsCfg)dataObject);

                    if (!configs.Contains(config))
                    {
                        configs.Add(config);
                    }
                }
            }
        }

        /// <summary>
        ///     Set the value of the specified property in storage.
        /// </summary>
        /// <param name="propertyName">Name of the property to set.</param>
        /// <param name="propertyValue">Value to set the property to.</param>
        public void Persist(string propertyName, string propertyValue)
        {
            // If the value is null, make it empty.
            if (propertyValue == null)
            {
                propertyValue = string.Empty;
            }

            foreach (var config in configs)
            {
                // Set the property
                config[propertyName] = propertyValue;
            }
            if (StoreChanged != null)
            {
                StoreChanged();
            }
        }

        /// <summary>
        /// Retrieve the value of the specified property from storage
        /// </summary>
        /// <param name="propertyName">Name of the property to retrieve</param>
        public string PropertyValue(string propertyName)
        {
            string value = null;
            if (configs.Count > 0)
                value = configs[0][propertyName];
            foreach (var config in configs)
            {
                if (config[propertyName] != value)
                {
                    // multiple config with different value for the property
                    value = string.Empty;
                    break;
                }
            }

            return value;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Protect from being called multiple times.
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                configs.Clear();
            }
            disposed = true;
        }
    }
}