/* Copyright © Pierre Sprimont, 2019
 *
 * This file is part of the WinCopies Framework.
 *
 * The WinCopies Framework is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The WinCopies Framework is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using WinCopies.Util;

namespace WinCopies.IO
{
    public interface IWMIItemInfoFactoryOptions : IDeepCloneable
    {

        IWMIItemInfoFactory Factory { get; }

        ConnectionOptions ConnectionOptions { get; set; }

        ObjectGetOptions ObjectGetOptions { get; set; }

        EnumerationOptions EnumerationOptions { get; set; }

    }

    public class WMIItemInfoFactoryOptions : IWMIItemInfoFactoryOptions
    {

        public IWMIItemInfoFactory Factory { get; internal set; }

        private ConnectionOptions _connectionOptions;

        private readonly DeepClone<ConnectionOptions> _connectionOptionsDelegate;

        /// <summary>
        /// Gets or sets options for the WMI connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">Exception thrown when this property is set while the <see cref="IBrowsableObjectInfoLoader{T}"/> is busy.</exception>
        protected virtual ConnectionOptions ConnectionOptionsOverride { get => _connectionOptions; set => _connectionOptions = value; }

        /// <summary>
        /// Gets or sets options for the WMI connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">Exception thrown when this property is set while the <see cref="IBrowsableObjectInfoLoader{T}"/> is busy.</exception>
        public ConnectionOptions ConnectionOptions
        {
            get => ConnectionOptionsOverride; set
            {

                if (!(Factory is null))

                    // BrowsableObjectInfoFactory.ThrowOnInvalidPropertySet(Factory);

                ConnectionOptionsOverride = value;

            }
        }

        private ObjectGetOptions _objectGetOptions;

        private readonly DeepClone<ObjectGetOptions> _objectGetOptionsDelegate;

        /// <summary>
        /// Gets or sets options for getting management objects.
        /// </summary>
        /// <exception cref="InvalidOperationException">Exception thrown when this property is set while the <see cref="IBrowsableObjectInfoLoader{T}"/> is busy.</exception>
        protected virtual ObjectGetOptions ObjectGetOptionsOverride
        {
            get => _objectGetOptions; set
            {

                // if (!(Factory is null))

                    // BrowsableObjectInfoFactory.ThrowOnInvalidPropertySet(Factory);

                _objectGetOptions = value;

            }
        }

        /// <summary>
        /// Gets or sets options for getting management objects.
        /// </summary>
        /// <exception cref="InvalidOperationException">Exception thrown when this property is set while the <see cref="IBrowsableObjectInfoLoader{T}"/> is busy.</exception>
        public ObjectGetOptions ObjectGetOptions

        {

            get => ObjectGetOptionsOverride; set
            {

                // if (!(Factory is null))

                    // BrowsableObjectInfoFactory.ThrowOnInvalidPropertySet(Factory);

                ObjectGetOptionsOverride = value;

            }

        }

        private EnumerationOptions _enumerationOptions;

        private readonly DeepClone<EnumerationOptions> _enumerationOptionsDelegate;

        /// <summary>
        /// Gets or sets options for management objects.
        /// </summary>
        /// <exception cref="InvalidOperationException">Exception thrown when this property is set while the <see cref="IBrowsableObjectInfoLoader{T}"/> is busy.</exception>
        protected virtual EnumerationOptions EnumerationOptionsOverride { get => _enumerationOptions; set => _enumerationOptions = value; }

        /// <summary>
        /// Gets or sets options for management objects.
        /// </summary>
        /// <exception cref="InvalidOperationException">Exception thrown when this property is set while the <see cref="IBrowsableObjectInfoLoader{T}"/> is busy.</exception>
        public virtual EnumerationOptions EnumerationOptions
        {
            get => EnumerationOptionsOverride; set
            {

                // if (!(Factory is null))

                    // BrowsableObjectInfoFactory.ThrowOnInvalidPropertySet(Factory);

                EnumerationOptionsOverride = value;

            }
        }

        public bool NeedsObjectsOrValuesReconstruction => true;

        public WMIItemInfoFactoryOptions() : this(null, null, null) { }

        public WMIItemInfoFactoryOptions(DeepClone<ConnectionOptions> connectionOptions, DeepClone<ObjectGetOptions> objectGetOptions, DeepClone<EnumerationOptions> enumerationOptions)

        {

            _connectionOptionsDelegate = connectionOptions;

            _connectionOptions = connectionOptions?.Invoke(null);

            _objectGetOptionsDelegate = objectGetOptions;

            _objectGetOptions = objectGetOptions?.Invoke(null);

            _enumerationOptionsDelegate = enumerationOptions;

            _enumerationOptions = enumerationOptions?.Invoke(null);

        }

        protected virtual void OnDeepClone(WMIItemInfoFactoryOptions wMIItemInfoFactoryOptions) { }

        protected virtual WMIItemInfoFactoryOptions DeepCloneOverride() => new WMIItemInfoFactoryOptions(_connectionOptionsDelegate, _objectGetOptionsDelegate, _enumerationOptionsDelegate);

        public object DeepClone()

        {

            WMIItemInfoFactoryOptions options = DeepCloneOverride();

            OnDeepClone(options);

            return options;

        }

    }
}
