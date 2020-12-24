/* Copyright © Pierre Sprimont, 2020
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

#if WinCopies3

namespace WinCopies.IO.PropertySystem
{
    /// <summary>
    /// The property group of an <see cref="IProperty"/>.
    /// </summary>
    public enum PropertyGroup
    {
        /// <summary>
        /// Default property group. This the group for the common properties.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Document-related properties.
        /// </summary>
        Document,

        /// <summary>
        /// Media-related properties.
        /// </summary>
        Media,

        /// <summary>
        /// Audio-related properties.
        /// </summary>
        Audio,

        /// <summary>
        /// Music-related properties.
        /// </summary>
        Music,

        /// <summary>
        /// Image-related properties.
        /// </summary>
        Image,

        /// <summary>
        /// Photo-related properties.
        /// </summary>
        Photo,

        /// <summary>
        /// RecordedTV-related properties.
        /// </summary>
        RecordedTV,

        /// <summary>
        /// Video-related properties.
        /// </summary>
        Video,

        /// <summary>
        /// DRM-related properties.
        /// </summary>
        DRM,

        /// <summary>
        /// AppUserModel-related properties.
        /// </summary>
        AppUserModel,

        /// <summary>
        /// Calendar-related properties.
        /// </summary>
        Calendar,

        /// <summary>
        /// Communication-related properties.
        /// </summary>
        Communication,

        /// <summary>
        /// Computer-related properties.
        /// </summary>
        Computer,

        /// <summary>
        /// Device-related properties.
        /// </summary>
        Device,

        /// <summary>
        /// Volume-related properties.
        /// </summary>
        Volume,

        /// <summary>
        /// Contact-related properties.
        /// </summary>
        Contact,

        /// <summary>
        /// GPS-related properties.
        /// </summary>
        GPS,

        /// <summary>
        /// Identity-related properties.
        /// </summary>
        Identity,

        /// <summary>
        /// Journal-related properties.
        /// </summary>
        Journal,

        /// <summary>
        /// LayoutPattern-related properties.
        /// </summary>
        LayoutPattern,

        /// <summary>
        /// Link-related properties.
        /// </summary>
        Link,

        /// <summary>
        /// Message-related properties.
        /// </summary>
        Message,

        /// <summary>
        /// Note-related properties.
        /// </summary>
        Note,

        /// <summary>
        /// Notifications-related properties.
        /// </summary>
        Notifications,

        /// <summary>
        /// Search-related properties.
        /// </summary>
        Search,

        /// <summary>
        /// Software-related properties.
        /// </summary>
        Software,

        /// <summary>
        /// Sync-related properties.
        /// </summary>
        Sync,

        /// <summary>
        /// Task-related properties.
        /// </summary>
        Task
    }
}

#endif
