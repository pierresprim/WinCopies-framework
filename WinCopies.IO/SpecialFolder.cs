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

using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopies.IO
{

    /// <summary>
    /// The special folder types.
    /// </summary>
    public enum SpecialFolder
    {

        /// <summary>
        /// A casual file system item.
        /// </summary>
        OtherFolderOrFile = 0,

        /// <summary>
        /// The <see cref="KnownFolders.Contacts"/> known folder.
        /// </summary>
        Contacts,

        /// <summary>
        /// The <see cref="KnownFolders.Desktop"/> known folder.
        /// </summary>
        Desktop,

        /// <summary>
        /// The <see cref="KnownFolders.Documents"/> known folder.
        /// </summary>
        Documents,

        /// <summary>
        /// The <see cref="KnownFolders.Downloads"/> known folder.
        /// </summary>
        Downloads,

        /// <summary>
        /// The <see cref="KnownFolders.Favorites"/> known folder.
        /// </summary>
        Favorites,

        /// <summary>
        /// The <see cref="KnownFolders.GameTasks"/> known folder.
        /// </summary>
        GameTasks,

        /// <summary>
        /// The <see cref="KnownFolders.Links"/> known folder.
        /// </summary>
        Links,

        /// <summary>
        /// The <see cref="KnownFolders.Music"/> known folder.
        /// </summary>
        Music,

        /// <summary>
        /// The <see cref="KnownFolders.SampleMusic"/> known folder.
        /// </summary>
        SampleMusic,

        /// <summary>
        /// The <see cref="KnownFolders.Playlists"/> known folder.
        /// </summary>
        Playlists,

        /// <summary>
        /// The <see cref="KnownFolders.SamplePlaylists"/> known folder.
        /// </summary>
        SamplePlaylists,

        /// <summary>
        /// The <see cref="KnownFolders.Ringtones"/> known folder.
        /// </summary>
        Ringtones,

        /// <summary>
        /// The <see cref="KnownFolders.Pictures"/> known folder.
        /// </summary>
        Pictures,

        /// <summary>
        /// The <see cref="KnownFolders.SamplePictures"/> known folder.
        /// </summary>
        SamplePictures,

        /// <summary>
        /// The <see cref="KnownFolders.SavedGames"/> known folder.
        /// </summary>
        SavedGames,

        /// <summary>
        /// The <see cref="KnownFolders.SavedSearches"/> known folder.
        /// </summary>
        SavedSearches,

        /// <summary>
        /// The <see cref="KnownFolders.Videos"/> known folder.
        /// </summary>
        Videos,

        /// <summary>
        /// The <see cref="KnownFolders.SampleVideos"/> known folder.
        /// </summary>
        SampleVideos,

        /// <summary>
        /// The <see cref="KnownFolders.UsersLibraries"/> known folder.
        /// </summary>
        UsersLibraries,

        /// <summary>
        /// The <see cref="KnownFolders.Libraries"/> known folder.
        /// </summary>
        Libraries,

        /// <summary>
        /// The <see cref="KnownFolders.DocumentsLibrary"/> known folder.
        /// </summary>
        DocumentsLibrary,

        /// <summary>
        /// The <see cref="KnownFolders.PicturesLibrary"/> known folder.
        /// </summary>
        PicturesLibrary,

        /// <summary>
        /// The <see cref="KnownFolders.CameraRollLibrary"/> known folder.
        /// </summary>
        CameraRollLibrary,

        /// <summary>
        /// The <see cref="KnownFolders.SavedPicturesLibrary"/> known folder.
        /// </summary>
        SavedPicturesLibrary,

        /// <summary>
        /// The <see cref="KnownFolders.MusicLibrary"/> known folder.
        /// </summary>
        MusicLibrary,

        /// <summary>
        /// The <see cref="KnownFolders.VideosLibrary"/> known folder.
        /// </summary>
        VideosLibrary,

        /// <summary>
        /// The <see cref="KnownFolders.RecordedTVLibrary"/> known folder.
        /// </summary>
        RecordedTVLibrary,

        /// <summary>
        /// The <see cref="KnownFolders.UsersFiles"/> known folder.
        /// </summary>
        UsersFiles,

        /// <summary>
        /// The <see cref="KnownFolders.Profile"/> known folder.
        /// </summary>
        Profile,

        /// <summary>
        /// The <see cref="KnownFolders.LocalAppData"/> known folder.
        /// </summary>
        LocalAppData,

        /// <summary>
        /// The <see cref="KnownFolders.LocalAppDataLow"/> known folder.
        /// </summary>
        LocalAppDataLow,

        /// <summary>
        /// The <see cref="KnownFolders.RoamingAppData"/> known folder.
        /// </summary>
        RoamingAppData,

        /// <summary>
        /// The <see cref="KnownFolders.AdminTools"/> known folder.
        /// </summary>
        AdminTools,

        /// <summary>
        /// The <see cref="KnownFolders.CDBurning"/> known folder.
        /// </summary>
        CDBurning,

        /// <summary>
        /// The <see cref="KnownFolders.Cookies"/> known folder.
        /// </summary>
        Cookies,

        /// <summary>
        /// The <see cref="KnownFolders.History"/> known folder.
        /// </summary>
        History,

        /// <summary>
        /// The <see cref="KnownFolders.ImplicitAppShortcuts"/> known folder.
        /// </summary>
        ImplicitAppShortcuts,

        /// <summary>
        /// The <see cref="KnownFolders.InternetCache"/> known folder.
        /// </summary>
        InternetCache,

        /// <summary>
        /// The <see cref="KnownFolders.NetHood"/> known folder.
        /// </summary>
        NetHood,

        /// <summary>
        /// The <see cref="KnownFolders.PrintHood"/> known folder.
        /// </summary>
        PrintHood,

        /// <summary>
        /// The <see cref="KnownFolders.Programs"/> known folder.
        /// </summary>
        Programs,

        /// <summary>
        /// The <see cref="KnownFolders.QuickLaunch"/> known folder.
        /// </summary>
        QuickLaunch,

        /// <summary>
        /// The <see cref="KnownFolders.Recent"/> known folder.
        /// </summary>
        Recent,

        /// <summary>
        /// The <see cref="KnownFolders.SendTo"/> known folder.
        /// </summary>
        SendTo,

        /// <summary>
        /// The <see cref="KnownFolders.StartMenu"/> known folder.
        /// </summary>
        StartMenu,

        /// <summary>
        /// The <see cref="KnownFolders.Startup"/> known folder.
        /// </summary>
        Startup,

        /// <summary>
        /// The <see cref="KnownFolders.Templates"/> known folder.
        /// </summary>
        Templates,

        /// <summary>
        /// The <see cref="KnownFolders.UserPinned"/> known folder.
        /// </summary>
        UserPinned,

        /// <summary>
        /// The <see cref="KnownFolders.UserProgramFiles"/> known folder.
        /// </summary>
        UserProgramFiles,

        /// <summary>
        /// The <see cref="KnownFolders.UserProgramFilesCommon"/> known folder.
        /// </summary>
        UserProgramFilesCommon,

        /// <summary>
        /// The <see cref="KnownFolders.PublicDesktop"/> known folder.
        /// </summary>
        PublicDesktop,

        /// <summary>
        /// The <see cref="KnownFolders.PublicDocuments"/> known folder.
        /// </summary>
        PublicDocuments,

        /// <summary>
        /// The <see cref="KnownFolders.PublicDownloads"/> known folder.
        /// </summary>
        PublicDownloads,

        /// <summary>
        /// The <see cref="KnownFolders.PublicGameTasks"/> known folder.
        /// </summary>
        PublicGameTasks,

        /// <summary>
        /// The <see cref="KnownFolders.PublicMusic"/> known folder.
        /// </summary>
        PublicMusic,

        /// <summary>
        /// The <see cref="KnownFolders.PublicRingtones"/> known folder.
        /// </summary>
        PublicRingtones,

        /// <summary>
        /// The <see cref="KnownFolders.PublicPictures"/> known folder.
        /// </summary>
        PublicPictures,

        /// <summary>
        /// The <see cref="KnownFolders.PublicVideos"/> known folder.
        /// </summary>
        PublicVideos,

        /// <summary>
        /// The <see cref="KnownFolders.Public"/> known folder.
        /// </summary>
        Public,

        /// <summary>
        /// The <see cref="KnownFolders.UserProfiles"/> known folder.
        /// </summary>
        UserProfiles,

        /// <summary>
        /// The <see cref="KnownFolders.CommonStartMenu"/> known folder.
        /// </summary>
        CommonStartMenu,

        /// <summary>
        /// The <see cref="KnownFolders.Fonts"/> known folder.
        /// </summary>
        Fonts,

        /// <summary>
        /// The <see cref="KnownFolders.ProgramFiles"/> known folder.
        /// </summary>
        ProgramFiles,

        /// <summary>
        /// The <see cref="KnownFolders.ProgramFilesCommon"/> known folder.
        /// </summary>
        ProgramFilesCommon,

        /// <summary>
        /// The <see cref="KnownFolders.ProgramFilesX86"/> known folder.
        /// </summary>
        ProgramFilesX86,

        /// <summary>
        /// The <see cref="KnownFolders.ProgramFilesCommonX86"/> known folder.
        /// </summary>
        ProgramFilesCommonX86,

        /// <summary>
        /// The <see cref="KnownFolders.ProgramFilesX64"/> known folder.
        /// </summary>
        ProgramFilesX64,

        /// <summary>
        /// The <see cref="KnownFolders.ProgramFilesCommonX64"/> known folder.
        /// </summary>
        ProgramFilesCommonX64,

        /// <summary>
        /// The <see cref="KnownFolders.ProgramData"/> known folder.
        /// </summary>
        ProgramData,

        /// <summary>
        /// The <see cref="KnownFolders.CommonAdminTools"/> known folder.
        /// </summary>
        CommonAdminTools,

        /// <summary>
        /// The <see cref="KnownFolders.CommonPrograms"/> known folder.
        /// </summary>
        CommonPrograms,

        /// <summary>
        /// The <see cref="KnownFolders.CommonStartup"/> known folder.
        /// </summary>
        CommonStartup,

        /// <summary>
        /// The <see cref="KnownFolders.CommonTemplates"/> known folder.
        /// </summary>
        CommonTemplates,

        /// <summary>
        /// The <see cref="KnownFolders.DeviceMetadataStore"/> known folder.
        /// </summary>
        DeviceMetadataStore,

        /// <summary>
        /// The <see cref="KnownFolders.Windows"/> known folder.
        /// </summary>
        Windows,

        /// <summary>
        /// The <see cref="KnownFolders.ResourceDir"/> known folder.
        /// </summary>
        ResourceDir,

        /// <summary>
        /// The <see cref="KnownFolders.SystemX86"/> known folder.
        /// </summary>
        SystemX86,

        /// <summary>
        /// The <see cref="KnownFolders.System"/> known folder.
        /// </summary>
        System,

        /// <summary>
        /// The <see cref="KnownFolders.Computer"/> known folder.
        /// </summary>
        Computer,

        /// <summary>
        /// The <see cref="KnownFolders.Connections"/> known folder.
        /// </summary>
        Connections,

        /// <summary>
        /// The <see cref="KnownFolders.ControlPanel"/> known folder.
        /// </summary>
        ControlPanel,

        /// <summary>
        /// The <see cref="KnownFolders.Internet"/> known folder.
        /// </summary>
        Internet,

        /// <summary>
        /// The <see cref="KnownFolders.Network"/> known folder.
        /// </summary>
        Network,

        /// <summary>
        /// The <see cref="KnownFolders.OtherUsers"/> known folder.
        /// </summary>
        OtherUsers,

        /// <summary>
        /// The <see cref="KnownFolders.Printers"/> known folder.
        /// </summary>
        Printers,

        /// <summary>
        /// The <see cref="KnownFolders.RecycleBin"/> known folder.
        /// </summary>
        RecycleBin,

        /// <summary>
        /// The <see cref="KnownFolders.SearchCsc"/> known folder.
        /// </summary>
        SearchCsc,

        /// <summary>
        /// The <see cref="KnownFolders.SearchHome"/> known folder.
        /// </summary>
        SearchHome,

        /// <summary>
        /// The <see cref="KnownFolders.SearchMapi"/> known folder.
        /// </summary>
        SearchMapi,

        /// <summary>
        /// The <see cref="KnownFolders.AddNewPrograms"/> known folder.
        /// </summary>
        AddNewPrograms,

        /// <summary>
        /// The <see cref="KnownFolders.AppUpdates"/> known folder.
        /// </summary>
        AppUpdates,

        /// <summary>
        /// The <see cref="KnownFolders.ChangeRemovePrograms"/> known folder.
        /// </summary>
        ChangeRemovePrograms,

        /// <summary>
        /// The <see cref="KnownFolders.CommonOemLinks"/> known folder.
        /// </summary>
        CommonOemLinks,

        /// <summary>
        /// The <see cref="KnownFolders.Conflict"/> known folder.
        /// </summary>
        Conflict,

        /// <summary>
        /// The <see cref="KnownFolders.Games"/> known folder.
        /// </summary>
        Games,

        /// <summary>
        /// The <see cref="KnownFolders.LocalizedResourcesDir"/> known folder.
        /// </summary>
        LocalizedResourcesDir,

        /// <summary>
        /// The <see cref="KnownFolders.OriginalImages"/> known folder.
        /// </summary>
        OriginalImages,

        /// <summary>
        /// The <see cref="KnownFolders.PhotoAlbums"/> known folder.
        /// </summary>
        PhotoAlbums,

        /// <summary>
        /// The <see cref="KnownFolders.SidebarDefaultParts"/> known folder.
        /// </summary>
        SidebarDefaultParts,

        /// <summary>
        /// The <see cref="KnownFolders.SidebarParts"/> known folder.
        /// </summary>
        SidebarParts,

        /// <summary>
        /// The <see cref="KnownFolders.SyncManager"/> known folder.
        /// </summary>
        SyncManager,

        /// <summary>
        /// The <see cref="KnownFolders.SyncResults"/> known folder.
        /// </summary>
        SyncResults,

        /// <summary>
        /// The <see cref="KnownFolders.SyncSetup"/> known folder.
        /// </summary>
        SyncSetup,

        /// <summary>
        /// The <see cref="KnownFolders.TreeProperties"/> known folder.
        /// </summary>
        TreeProperties

    }

}
