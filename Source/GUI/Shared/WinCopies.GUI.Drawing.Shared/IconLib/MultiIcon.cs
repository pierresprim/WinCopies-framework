//  Copyright (c) 2006, Gustavo Franco
//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.

using System;
using System.IO;
using System.Collections.Generic;

using WinCopies.GUI.Drawing.EncodingFormats;
using WinCopies.GUI.Drawing.Exceptions;

using static WinCopies.
#if !WinCopies3
    Util.Util
#else
    ThrowHelper
#endif
    ;
using static WinCopies.GUI.Drawing.Resources.ExceptionMessages;

namespace WinCopies.GUI.Drawing
{
    [Author("Franco, Gustavo")]
    public class MultiIcon : List<SingleIcon>
    {
        private int mSelectedIndex = -1;

        #region Properties
        public int SelectedIndex { get => mSelectedIndex; set => mSelectedIndex = value >= Count ? throw new ArgumentOutOfRangeException(nameof(SelectedIndex)) : value; }

        public string SelectedName
        {
            get => mSelectedIndex < 0 || mSelectedIndex >= Count ? null : this[mSelectedIndex].Name;

            set
            {
                ThrowIfNull(value, nameof(SelectedName));

                for (int i = 0; i < Count; i++)

                    if (this[i].Name.ToLower() == value.ToLower())
                    {
                        mSelectedIndex = i;

                        return;
                    }

                throw new InvalidDataException($"{nameof(SelectedName)} does not exist.");
            }
        }

        public string[] IconNames
        {
            get
            {
                var names = new string[Count];

                for (int i = 0; i < Count; i++)

                    names[i] = this[i].Name;

                return names;
            }
        }
        #endregion

        #region Indexers
        public SingleIcon this[in string name]
        {
            get
            {
                for (int i = 0; i < Count; i++)

                    if (this[i].Name.ToLower() == name.ToLower())

                        return this[i];

                return null;
            }
        }
        #endregion

        #region Constructors
        public MultiIcon() { }

        public MultiIcon(in IEnumerable<SingleIcon> collection) => AddRange(collection);

        public MultiIcon(in SingleIcon singleIcon)
        {
            Add(singleIcon ?? throw GetArgumentNullException(nameof(singleIcon)));

            SelectedName = singleIcon.Name;
        }
        #endregion

        #region Public Methods
        public SingleIcon Add(in string iconName)
        {
            // Lets Create the icon group
            // Add group to the master list and also lets give a name
            var singleIcon = /*Already exist?*/ Contains(iconName) ? throw new IconNameAlreadyExistException() : new SingleIcon(iconName);
            Add(singleIcon);
            return singleIcon;
        }

        public void Remove(in string iconName)
        {
            ThrowIfNull(iconName, nameof(iconName));

            // If not exist then do nothing
            int index = IndexOf(iconName);

            if (index > -1)

                RemoveAt(index);
        }

        public bool Contains(in string iconName) =>
            // Exist?
            IndexOf(iconName ?? throw GetArgumentNullException(nameof(iconName))) >= 0;

        public int IndexOf(in string iconName)
        {
            ThrowIfNull(iconName, nameof(iconName));

            // Exist?
            for (int i = 0; i < Count; i++)

                if (this[i].Name.ToLower() == iconName.ToLower())

                    return i;

            return -1;
        }

        public void Load(in string fileName)
        {
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            try
            {
                Load(fs);
            }

            finally
            {
                fs?.Close();
            }
        }

        public void Load(in System.IO.Stream stream)
        {
            ILibraryFormat baseFormat;

            if ((baseFormat = new IconFormat()).IsRecognizedFormat(stream))

                if (mSelectedIndex == -1)
                {
                    Clear();
                    Add(baseFormat.Load(stream)[0]);
                    this[0].Name = "Untitled";
                }

                else
                {
                    string currentName = this[mSelectedIndex].Name;
                    this[mSelectedIndex] = baseFormat.Load(stream)[0];
                    this[mSelectedIndex].Name = currentName;
                }

            else if ((baseFormat = new NEFormat()).IsRecognizedFormat(stream))

                CopyFrom(baseFormat.Load(stream));

            else if ((baseFormat = new PEFormat()).IsRecognizedFormat(stream))

                CopyFrom(baseFormat.Load(stream));

            else

                throw new InvalidFileException();

            SelectedIndex = Count > 0 ? 0 : -1;
        }

        public void Save(in string fileName, in MultiIconFormat format)
        {
            var fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);

            try
            {
                Save(fs, format);
            }

            finally
            {
                fs?.Close();
            }
        }

        public void Save(in System.IO.Stream stream, in MultiIconFormat format)
        {
            switch (format)
            {
                case MultiIconFormat.ICO:

                    (mSelectedIndex == -1 ? throw new InvalidIconSelectionException() : new IconFormat()).Save(this, stream);

                    break;

                case MultiIconFormat.ICL:

                    new NEFormat().Save(this, stream);

                    break;

                case MultiIconFormat.DLL:

                    new PEFormat().Save(this, stream);

                    break;

                case MultiIconFormat.EXE:
                case MultiIconFormat.OCX:
                case MultiIconFormat.CPL:
                case MultiIconFormat.SRC:

                    throw new NotSupportedException(FileFormatNotSupported);

                default:

                    throw new NotSupportedException(UnknownFileTypeDestination);
            }
        }
        #endregion

        #region Private Methods
        private void CopyFrom(in MultiIcon multiIcon)
        {
            mSelectedIndex = multiIcon.mSelectedIndex;
            Clear();
            AddRange(multiIcon);
        }
        #endregion
    }
}
