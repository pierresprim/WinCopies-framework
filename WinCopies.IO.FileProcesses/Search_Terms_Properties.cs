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

//namespace WinCopies.IO.FileProcesses
//{

//    /// <summary>
//    /// Classe exposant des propriétés qui permettent, lorsque la classe est instanciée, de définir les propriétés de recherche de fichiers et dossiers.
//    /// </summary>
//    public class Search_Terms_Properties
//    {

//        /// <summary>
//        /// Énumération des différents types de propriétés à vérifier. Cette énumération peut contenir plusieurs valeurs et est donc organisée selon un champ de bits.
//        /// </summary>
//        [Flags]
//        public enum Properties_To_Look_For___Enum
//        {

//            /// <summary>
//            /// Ne vérifie aucune catégorie.
//            /// </summary>
//            None = 0,

//            /// <summary>
//            /// Vérifie le répertoire complet - répertoire ( dossier ) et nom du fichier / dossier.
//            /// </summary>
//            FullName = 1,

//            /// <summary>
//            /// Vérifie le nom du fichier / dossier.
//            /// </summary>
//            Name = 2,

//            /// <summary>
//            /// Vérifie l'extension du nom du fichier / dossier.
//            /// </summary>
//            Extension = 4,

//            /// <summary>
//            /// Vérifie le répertoire du fichier / dossier.
//            /// </summary>
//            FolderPath = 8,

//            /// <summary>
//            /// Vérifie la date de création du fichier / dossier.
//            /// </summary>
//            CreationDate = 16,

//            /// <summary>
//            /// Vérifie la date de dernière modification du dossier / fichier.
//            /// </summary>
//            LastModificationDate = 32,

//            /// <summary>
//            /// Vérifie la dernière date de lecture du dossier / fichier.
//            /// </summary>
//            LastReadingDate = 64,

//            /// <summary>
//            /// Vérifie la taille du fichier. Pour les dossiers, cette option n'est pas encore gérée dans cette version du framework de WinCopies.
//            /// </summary>
//            Size = 128

//        }

//        /// <summary>
//        /// Énumération des différents types de dossiers / fichiers / lecteurs, à vérifier. Cette énumération peut contenir plusieurs valeurs à la fois et est donc organisée selon un champ de bits.
//        /// </summary>
//        [Flags]
//        public enum Look_For___Enum
//        {

//            /// <summary>
//            /// Ne recherche rien.
//            /// </summary>
//            None = 0,

//            /// <summary>
//            /// Recherche dans les dossiers.
//            /// </summary>
//            Folders = 1,

//            /// <summary>
//            /// Recherche dans les fichiers.
//            /// </summary>
//            Files = 2,

//            /// <summary>
//            /// Recherche des lecteurs entiers.
//            /// </summary>
//            Drives = 4

//        }

//        /// <summary>
//        /// Énumération des différentes propriétés de la date à vérifier, par ex. le jour, le mois ou l'année. Cette énumération peut contenir plusieurs valeurs à la fois et est donc organisée selon un champ de bits.
//        /// </summary>
//        [Flags]
//        public enum Date_Time_Look_For
//        {

//            /// <summary>
//            /// Ne vérifie aucune propriété de la date.
//            /// </summary>
//            None = 0,

//            /// <summary>
//            /// Vérifie le jour de la date à comparer.
//            /// </summary>
//            Day = 1,

//            /// <summary>
//            /// Vérifie le mois de la date à comparer.
//            /// </summary>
//            Month = 2,

//            /// <summary>
//            /// Vérifie l'année de la date à comparer.
//            /// </summary>
//            Year = 4,


//            /// <summary>
//            /// Vérifie l'heure de la date à comparer.
//            /// </summary>
//            Hour = 8,

//            /// <summary>
//            /// Vérifie les minutes de la date à comparer.
//            /// </summary>
//            Minutes = 16,

//            /// <summary>
//            /// Vérifie les secondes de la date à comparer.
//            /// </summary>
//            Seconds = 32

//        }

//        public class Date_Time_To_Search
//        {

//            public int Day { get; set; }

//            public int Month { get; set; }

//            public int Year { get; set; }

//            public static Date_Time_To_Search Now
//            {

//                get
//                {

//                    DateTime date_Time_Now = DateTime.Now;

//                    Date_Time_To_Search date_Time = new Date_Time_To_Search();

//                    date_Time.Day = date_Time_Now.Day;

//                    date_Time.Month = date_Time_Now.Month;

//                    date_Time.Year = date_Time_Now.Year;

//                    return date_Time;

//                }

//            }

//            public Date_Time_To_Search()
//            {

//                Day = DateTime.Now.Day;

//                Month = DateTime.Now.Month;

//                Year = DateTime.Now.Year;

//            } // end Date_Time_To_Search

//            public static Date_Time_To_Search ConvertFrom(string value)
//            {

//                DateTime Dot_Net_Date_Time = DateTime.Now;

//                Date_Time_To_Search date_Time = new Date_Time_To_Search();

//                //int results = 0;

//                bool ok = true;

//                if (!value.Contains("*"))
//                {

//                    ok = DateTime.TryParse(value, out Dot_Net_Date_Time);

//                    if (ok)
//                    {

//                        date_Time.Day = Dot_Net_Date_Time.Day;

//                        date_Time.Month = Dot_Net_Date_Time.Month;

//                        date_Time.Year = Dot_Net_Date_Time.Year;

//                    } // end if

//                } // end if

//                else if (value.Length == 9)
//                {

//                    ok = (value[0] == '*' || char.IsNumber(value[0])) &&

//                        value[1] == '/' &&

//                        (value[2] == '*' || char.IsNumber(value[2])) &&

//                        (value[3] == '*' || char.IsNumber(value[3])) &&

//                        value[4] == '/' &&

//                        (value[5] == '*' || char.IsNumber(value[5])) &&

//                        (value[6] == '*' || char.IsNumber(value[6])) &&

//                        (value[7] == '*' || char.IsNumber(value[7])) &&

//                        (value[8] == '*' || char.IsNumber(value[8]));

//                    if (ok)
//                    {

//                        date_Time.Day = int.Parse(value.Substring(0, 1));

//                        date_Time.Month = int.Parse(value.Substring(2, 2));

//                        date_Time.Year = int.Parse(value.Substring(5, 4));

//                    } // end if

//                } // end else if

//                else if (value.Length == 10)
//                {

//                    int i = -1;

//                    foreach (char chr in value)
//                    {

//                        i++;

//                        if (i == 2 | i == 5)
//                            if (chr == '/') continue;

//                            else { ok = false; break; }

//                        ok = value[i] == '*' || char.IsNumber(value[i]);

//                        if (!ok) break;

//                    }



//                    if (ok)
//                    {

//                        date_Time.Day = int.Parse(value.Substring(0, 2));

//                        date_Time.Month = int.Parse(value.Substring(3, 2));

//                        date_Time.Year = int.Parse(value.Substring(6, 4));

//                    } // end if

//                } // end else if

//                else ok = false;

//                if (ok) return date_Time; else return null;

//            } // end Date_Time_To_Search

//        } // end class

//        public Properties_To_Look_For___Enum Properties_To_Look_For { get; set; }

//        public Look_For___Enum Look_For { get; set; }



//        // FullName Region

//        /// <summary>
//        /// Le nom complet ( chemin d'accès ) à vérifier.
//        /// </summary>
//        public string FullName { get; set; }

//        /// <summary>
//        /// Indique s'il faut ajouter les éléments analysés à la liste des éléments trouvés uniquement s'ils  correspondent en entier au terme recherché. La valeur par défaut est false, ce qui signifie que les éléments seront ajoutés même s'ils ne correspondent que partiellement au terme de recherche indiqué.
//        /// </summary>
//        public bool FullName_FullWord { get; set; }

//        /// <summary>
//        /// Indique si la recherche doit être sensible à la casse. La valeur par défaut est false, ce qui signifie qu'il n'y aura aucune restriction dans la recherche en fonction de la casse.
//        /// </summary>
//        public bool FullName_CaseSensitive { get; set; }



//        // Name Region

//        /// <summary>
//        /// Le nom à chercher.
//        /// </summary>
//        public string Name { get; set; }

//        /// <summary>
//        /// Indique s'il faut ajouter les éléments analysés à la liste des éléments trouvés uniquement s'ils  correspondent en entier au terme recherché. La valeur par défaut est false, ce qui signifie que les éléments seront ajoutés même s'ils ne correspondent que partiellement au terme de recherche indiqué.
//        /// </summary>
//        public bool Name_FullWord { get; set; }

//        /// <summary>
//        /// Indique si la recherche doit être sensible à la casse. La valeur par défaut est false, ce qui signifie qu'il n'y aura aucune restriction dans la recherche en fonction de la casse.
//        /// </summary>
//        public bool Name_CaseSensitive { get; set; }



//        // Extension Region

//        /// <summary>
//        /// L'extension à tester.
//        /// </summary>
//        public string Extension { get; set; }

//        /// <summary>
//        /// Indique s'il faut ajouter les éléments analysés à la liste des éléments trouvés uniquement s'ils  correspondent en entier au terme recherché. La valeur par défaut est false, ce qui signifie que les éléments seront ajoutés même s'ils ne correspondent que partiellement au terme de recherche indiqué.
//        /// </summary>
//        public bool Extension_FullWord { get; set; }

//        /// <summary>
//        /// Indique si la recherche doit être sensible à la casse. La valeur par défaut est false, ce qui signifie qu'il n'y aura aucune restriction dans la recherche en fonction de la casse.
//        /// </summary>
//        public bool Extension_CaseSensitive { get; set; }



//        // FolderPath Region

//        /// <summary>
//        /// Le nom du répertoire des fichiers / dossiers à vérifier.
//        /// </summary>
//        public string FolderPath { get; set; }

//        /// <summary>
//        /// Indique s'il faut ajouter les éléments analysés à la liste des éléments trouvés uniquement s'ils  correspondent en entier au terme recherché. La valeur par défaut est false, ce qui signifie que les éléments seront ajoutés même s'ils ne correspondent que partiellement au terme de recherche indiqué.
//        /// </summary>
//        public bool FolderPath_FullWord { get; set; }

//        /// <summary>
//        /// Indique si la recherche doit être sensible à la casse. La valeur par défaut est false, ce qui signifie qu'il n'y aura aucune restriction dans la recherche en fonction de la casse.
//        /// </summary>
//        public bool FolderPath_CaseSensitive { get; set; }



//        // CreationDate Region

//        /// <summary>
//        /// Obtient ou définit une valeur qui indique s'il faut ou non vérifier la date de création des éléments éventuellement mentionnée.
//        /// </summary>
//        public Date_Time_Look_For CreationDate_Look_For { get; set; }

//        /// <summary>
//        /// Obtient ou définit une valeur qui indique la date de création à vérifier si la propriété CreationDate_Look_For a la valeur True.
//        /// </summary>
//        public Date_Time_To_Search CreationDate { get; set; }



//        //  LastModificationDate Region

//        /// <summary>
//        /// Obtient ou définit une valeur qui indique s'il faut ou non vérifier la date de dernière modification des éléments éventuellement mentionnée.
//        /// </summary>
//        public Date_Time_Look_For LastModificationDate_Look_For { get; set; }

//        /// <summary>
//        /// Obtient ou définit une valeur qui indique la date de dernière modification à vérifier si la propriété CreationDate_Look_For a la valeur True.
//        /// </summary>
//        public Date_Time_To_Search LastModificationDate { get; set; }



//        // LastReadingDate Region

//        /// <summary>
//        /// Obtient ou définit une valeur qui indique s'il faut ou non vérifier la date de dernier accès des éléments éventuellement mentionnée.
//        /// </summary>
//        public Date_Time_Look_For LastReadingDate_Look_For { get; set; }

//        /// <summary>
//        /// Obtient ou définit une valeur qui indique la date de dernier accès à vérifier si la propriété CreationDate_Look_For a la valeur True.
//        /// </summary>
//        public Date_Time_To_Search LastReadingDate { get; set; }



//        /// <summary>
//        /// Obtient ou dénit une valeur qui indique la taille à comparer.
//        /// </summary>
//        public long Size { get; set; }

//        /// <summary>
//        /// Initialise une nouvelle instance de la classe Search_Terms_Properties.
//        /// </summary>
//        public Search_Terms_Properties()
//        {

//            Properties_To_Look_For = Properties_To_Look_For___Enum.None;

//            Look_For = Look_For___Enum.None;

//            FullName = "";

//            FullName_FullWord = false;

//            FullName_CaseSensitive = false;

//            Name = "";

//            Name_FullWord = false;

//            Name_CaseSensitive = false;

//            Extension = "";

//            Extension_FullWord = false;

//            Extension_CaseSensitive = false;

//            FolderPath = "";

//            FolderPath_FullWord = false;

//            FolderPath_CaseSensitive = false;



//            CreationDate_Look_For = Date_Time_Look_For.None;

//            CreationDate = Date_Time_To_Search.Now;

//            LastModificationDate_Look_For = Date_Time_Look_For.None;

//            LastModificationDate = Date_Time_To_Search.Now;

//            LastReadingDate_Look_For = Date_Time_Look_For.None;

//            LastReadingDate = Date_Time_To_Search.Now;



//            Size = 0;

//        }

//    }

//}
