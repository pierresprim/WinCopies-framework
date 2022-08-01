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

//using System;
//using System.IO;

//namespace WinCopies.IO.FileProcesses
//{

//    public static class SearchMethods 
//    {

//        // /// <summary>
//        // /// Vérifie s'il faut ou non ajouter les dossiers et les fichiers aux éléments correspondant aux propriétés recherchées.
//        // /// </summary>
//        // /// <param name="path">Répertoire à vérifier.</param>
//        // /// <param name="ActionType">Type de processus.</param>
//        // /// <param name="LoadOnlyItemsWithSearchTermsForAllActions">Avec ActionType, détermine en fonction du processus s'il faut ajouter tous les éléments trouvés ou uniquement ceux qui correspondent aux termes recherchés.</param>
//        // /// <param name="Search_Terms">Termes à rechercher.</param>
//        // /// <returns>Retourne une valeur indiquant s'il faut ou non ajouter l'adresse actuellement testée. La valeur est true si les paramètres testés correspondent.</returns>
//        public static bool AddFile(System.IO.FileSystemInfo path, FileType fileType, ActionType ActionType, bool LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms_Properties Search_Terms)
//        {

//#if DEBUG 
//            Console.WriteLine(path.FullName);
//#endif

//            bool add_File = false;

//            if (ActionType != ActionType.Search && !LoadOnlyItemsWithSearchTermsForAllActions)

//                add_File = true;

//            else if (fileType == FileType.Folder || fileType == FileType.Drive)

//                add_File = switch_To_Know_If_Add_File(path, Search_Terms);

//            else

//                add_File = switch_To_Know_If_Add_File(path, Search_Terms);

//            return 

//                add_File;

//        } // end bool

//        // /// <summary>
//        // /// Détermine s'il faut ou non ajouter le dossier à la liste des éléments correspondant à la recherche.
//        // /// </summary>
//        // /// <param name="directory">Dossier à tester.</param>
//        // /// <param name="Search_Terms">Termes à vérifier.</param>
//        // /// <returns>Retourne une valeur indiquant s'il faut ou non ajouter le dossier actuellement testé. La valeur est true si les paramètres testés correspondent.</returns>
//        public static bool switch_To_Know_If_Add_File(System.IO.FileSystemInfo path, Search_Terms_Properties Search_Terms)
//        {

//            //switch (Search_Terms.Properties_To_Look_For)
//            //{

//            if (!Search_Terms.Look_For.HasFlag(Search_Terms_Properties.Look_For___Enum.Folders) | Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.Size)) return false;



//            // Search_Terms_Properties.Properties_To_Look_For_Enum.FullName:

//            if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.FullName))
//            {

//                if (!path.FullName.ToLower().Contains(Search_Terms.FullName.ToLower())) return false;

//                if (Search_Terms.FullName_FullWord && path.FullName.ToLower() != Search_Terms.FullName.ToLower()) return false;

//                if (Search_Terms.FullName_CaseSensitive)
//                {

//                    if (Search_Terms.FullName_FullWord && path.FullName != Search_Terms.FullName) return false;

//                    if (!path.FullName.Contains(Search_Terms.FullName)) return false;

//                } // end if

//            } // end if



//            // Search_Terms_Properties.Properties_To_Look_For_Enum.Name:

//            if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.Name))
//            {

//                if (!path.Name.ToLower().Contains(Search_Terms.Name.ToLower())) return false;

//                if (Search_Terms.Name_FullWord && path.Name.ToLower() != Search_Terms.Name.ToLower()) return false;

//                if (Search_Terms.Name_CaseSensitive)
//                {

//                    if (Search_Terms.Name_FullWord && path.Name != Search_Terms.Name) return false;

//                    if (!path.Name.Contains(Search_Terms.Name)) return false;

//                } // end if

//            } // end if



//            // Search_Terms_Properties.Properties_To_Look_For_Enum.Name:

//            if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.Extension))
//            {

//                if (!path.Extension.ToLower().Contains(Search_Terms.Extension.ToLower())) return false;

//                if (Search_Terms.Extension_FullWord && path.Extension.ToLower() != Search_Terms.Extension.ToLower()) return false;

//                if (Search_Terms.Extension_CaseSensitive)
//                {

//                    if (Search_Terms.Extension_FullWord && path.Extension != Search_Terms.Extension) return false;

//                    if (!path.Extension.Contains(Search_Terms.Extension)) return false;

//                } // end if

//            } // end if



//            // Search_Terms_Properties.Properties_To_Look_For_Enum.FolderPath:

//            if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.FolderPath))
//            {

//                if (!path.FullName.EndsWith(":") && !path.FullName.EndsWith(@":\") && !path.FullName.EndsWith(@":\\"))
//                {

//                    if (!((DirectoryInfo)path).Parent.FullName.ToLower().Contains(Search_Terms.FolderPath.ToLower())) return false;

//                    if (Search_Terms.FolderPath_FullWord && ((DirectoryInfo)path).Parent.FullName.ToLower() != Search_Terms.FolderPath.ToLower()) return false;

//                    if (Search_Terms.FolderPath_CaseSensitive)
//                    {

//                        if (Search_Terms.FolderPath_FullWord && ((DirectoryInfo)path).Parent.FullName != Search_Terms.FolderPath) return false;

//                        if (!((DirectoryInfo)path).Parent.FullName.Contains(Search_Terms.FolderPath)) return false;

//                    } // end if

//                } // end if

//            } // end if



//            // Search_Terms_Properties.Properties_To_Look_For_Enum.CreationDate:

//            if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.CreationDate))



//                if (!check_Date(Search_Terms.CreationDate, path.CreationTime, Search_Terms.CreationDate_Look_For)) return false;



//            // Search_Terms_Properties.Properties_To_Look_For_Enum.LastModificationDate:

//            if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.LastModificationDate))



//                if (!check_Date(Search_Terms.LastModificationDate, path.LastWriteTime, Search_Terms.LastModificationDate_Look_For)) return false;



//            // Search_Terms_Properties.Properties_To_Look_For_Enum.LastReadingDate:

//            if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.LastReadingDate))



//                if (!check_Date(Search_Terms.LastReadingDate, path.LastAccessTime, Search_Terms.LastReadingDate_Look_For)) return false;



//            //}

//            return true;

//        }

//        // /// <summary>
//        // /// Détermine s'il faut ajouter ou non le fichier à la liste des éléments correspondant à la recherche.
//        // /// </summary>
//        // /// <param name="file">Fichier à tester.</param>
//        // /// <param name="Search_Terms">Termes à vérifier.</param>
//        // /// <returns>Retourne une valeur indiquant s'il faut ou non ajouter le fichier actuellement testé. La valeur est true si les paramètres testés correspondent.</returns>
//        //public static bool switch_To_Know_If_Add_File(System.IO.FileInfo file, Search_Terms_Properties Search_Terms)
//        //{

//        //    //switch (Search_Terms.Properties_To_Look_For)
//        //    //{

//        //    if (!Search_Terms.Look_For.HasFlag(Search_Terms_Properties.Look_For___Enum.Files)) return false;



//        //    // Search_Terms_Properties.Properties_To_Look_For_Enum.FullName:

//        //    if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.FullName))
//        //    {

//        //        if (!file.FullName.ToLower().Contains(Search_Terms.FullName.ToLower())) return false;

//        //        if (Search_Terms.FullName_FullWord && file.FullName.ToLower() != Search_Terms.FullName.ToLower()) return false;

//        //        if (Search_Terms.FullName_CaseSensitive)
//        //        {

//        //            if (Search_Terms.FullName_FullWord && file.FullName != Search_Terms.FullName) return false;

//        //            if (!file.FullName.Contains(Search_Terms.FullName)) return false;

//        //        } // end if

//        //    } // end if



//        //    // Search_Terms_Properties.Properties_To_Look_For_Enum.Name:

//        //    if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.Name))
//        //    {

//        //        if (!file.Name.Substring(0, file.Name.LastIndexOf(".")).ToLower().Contains(Search_Terms.Name.ToLower())) return false;

//        //        if (Search_Terms.Name_FullWord && file.Name.Substring(0, file.Name.LastIndexOf(".")).ToLower() != Search_Terms.Name.ToLower()) return false;

//        //        if (Search_Terms.Name_CaseSensitive)
//        //        {

//        //            if (Search_Terms.Name_FullWord && file.Name.Substring(0, file.Name.LastIndexOf(".")) != Search_Terms.Name) return false;

//        //            if (!file.Name.Substring(0, file.Name.LastIndexOf(".")).Contains(Search_Terms.Name)) return false;

//        //        } // end if

//        //    } // end if



//        //    // Search_Terms_Properties.Properties_To_Look_For_Enum.Extension:

//        //    if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.Extension))
//        //    {

//        //        if (!file.Extension.ToLower().Contains(Search_Terms.Extension.ToLower())) return false;

//        //        if (Search_Terms.Extension_FullWord && file.Extension.ToLower() != Search_Terms.Extension.ToLower()) return false;

//        //        if (Search_Terms.Extension_CaseSensitive)
//        //        {

//        //            if (Search_Terms.Extension_FullWord && file.Extension != Search_Terms.Extension) return false;

//        //            if (!file.Extension.Contains(Search_Terms.Extension)) return false;

//        //        } // end if

//        //    } // end if



//        //    // Search_Terms_Properties.Properties_To_Look_For_Enum.FolderPath:

//        //    string directoryName = "";

//        //    if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.FolderPath))
//        //    {

//        //        if (!file.FullName.EndsWith(":") && !file.FullName.EndsWith(@":\") && !file.FullName.EndsWith(@":\\"))
//        //        {



//        //            directoryName = new DirectoryInfo(file.DirectoryName).FullName;



//        //            if (!directoryName.ToLower().Contains(Search_Terms.FolderPath.ToLower())) return false;

//        //            if (Search_Terms.FolderPath_FullWord && directoryName.ToLower() != Search_Terms.FolderPath.ToLower()) return false;

//        //            if (Search_Terms.FolderPath_CaseSensitive)
//        //            {

//        //                if (Search_Terms.FolderPath_FullWord && directoryName != Search_Terms.FolderPath) return false;

//        //                if (!directoryName.Contains(Search_Terms.FolderPath)) return false;

//        //            } // end if

//        //        } // end if

//        //    } // end if

//        //    // Search_Terms_Properties.Properties_To_Look_For_Enum.CreationDate:

//        //    if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.CreationDate))



//        //        if (!check_Date(Search_Terms.CreationDate, file.CreationTime, Search_Terms.CreationDate_Look_For)) return false;



//        //    // Search_Terms_Properties.Properties_To_Look_For_Enum.LastModificationDate:

//        //    if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.LastModificationDate))



//        //        if (!check_Date(Search_Terms.LastModificationDate, file.LastWriteTime, Search_Terms.LastModificationDate_Look_For)) return false;



//        //    // Search_Terms_Properties.Properties_To_Look_For_Enum.LastReadingDate:

//        //    if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.LastReadingDate))



//        //        if (!check_Date(Search_Terms.LastReadingDate, file.LastAccessTime, Search_Terms.LastReadingDate_Look_For)) return false;



//        //    // Search_Terms_Properties.Properties_To_Look_For_Enum.Size:



//        //    if (Search_Terms.Properties_To_Look_For.HasFlag(Search_Terms_Properties.Properties_To_Look_For___Enum.Size))



//        //        if (file.Length > Search_Terms.Size) return false;

//        //    //}

//        //    return true;

//        //} // end bool

//        // /// <summary>
//        // /// Vérifie si les propriétés à vérifier de la date mentionnées dans le paramètre date correspondent à celles des fichiers et répertoires mentionnés dans le paramètre file_Date.
//        // /// </summary>
//        // /// <param name="date">Propriétés indiquant les éléments de la date à vérifier.</param>
//        // /// <param name="file_Date">Fichier à vérifier.</param>
//        // /// <param name="items_To_Check">Éléments à vérifier.</param>
//        // /// <returns>Retourne une valeur booléenne qui indique si les paramètres à vérifier de la date mentionnée correspondent à ceux du fichier.</returns>
//        public static bool check_Date(Search_Terms_Properties.Date_Time_To_Search date, DateTime file_Date, Search_Terms_Properties.Date_Time_Look_For items_To_Check)
//        {

//            if (items_To_Check.HasFlag(Search_Terms_Properties.Date_Time_Look_For.Day)) if (date.Day != file_Date.Day) return false;

//            if (items_To_Check.HasFlag(Search_Terms_Properties.Date_Time_Look_For.Month)) if (date.Month != file_Date.Month) return false;

//            if (items_To_Check.HasFlag(Search_Terms_Properties.Date_Time_Look_For.Year)) if (date.Year != file_Date.Year) return false;

//            return true;

//        } // end bool

//    }

//}
