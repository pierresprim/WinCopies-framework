﻿using System;

namespace WinCopies.IO
{
    public interface IWMIItemsLoader : IBrowsableObjectInfoLoader
    {

        /// <summary>
        /// Gets or sets the WMI item types to load.
        /// </summary>
        /// <exception cref="InvalidOperationException">Exception thrown when this property is set while the <see cref="WMIItemsLoader"/> is busy.</exception>
        WMIItemTypes WMIItemTypes { get; set; }
    }
}
