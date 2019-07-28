﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopies.IO
{

    /// <summary>
    /// Provides common properties for <see cref="IBrowsableObjectInfo"/> factories.
    /// </summary>
    public interface IBrowsableObjectInfoFactory

    {

        /// <summary>
        /// Gets the <see cref="IBrowsableObjectInfo"/> associated to this <see cref="IBrowsableObjectInfoFactory"/>.
        /// </summary>
        IBrowsableObjectInfo Path { get; }

        /// <summary>
        /// Whether to add the current <see cref="IBrowsableObjectInfoFactory"/> to all the new objects created from this <see cref="IBrowsableObjectInfoFactory"/>.
        /// </summary>
        bool UseCurrentFactoryRecursively { get; set; }

    }

}
