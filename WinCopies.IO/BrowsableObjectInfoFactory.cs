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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopies.IO
{

    /// <summary>
    /// Provides a base class for <see cref="BrowsableObjectInfo"/> factories.
    /// </summary>
    public abstract class BrowsableObjectInfoFactory : IBrowsableObjectInfoFactory

    {

        //public static bool ValidatePropertySet(IBrowsableObjectInfoFactory factory) => factory.Loader?.IsBusy == false;

        //public static void ThrowOnInvalidPropertySet(IBrowsableObjectInfoFactory factory)

        //{

        //    if ( factory.Loader?.IsBusy == true)

        //        throw new InvalidOperationException($"The Path's ItemsLoader of the current {nameof(BrowsableObjectInfoFactory)} is busy.");

        //}

        public IBrowsableObjectInfo Path { get; }

        protected virtual void OnDeepClone(BrowsableObjectInfoFactory factory) { }

        protected abstract BrowsableObjectInfoFactory DeepCloneOverride();

        public virtual object DeepClone()

        {

            BrowsableObjectInfoFactory browsableObjectInfoFactory = DeepCloneOverride();

            OnDeepClone(browsableObjectInfoFactory);

            return browsableObjectInfoFactory;

        }

        /// <summary>
        /// Gets a value that indicates whether this object needs to reconstruct objects on deep cloning.
        /// </summary>
        public virtual bool NeedsObjectsOrValuesReconstruction => false;

    }

}
