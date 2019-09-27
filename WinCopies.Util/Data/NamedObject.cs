﻿using System;
using WinCopies.Util;

namespace WinCopies.Util.Data
{

    /// <summary>
    /// Provides an object that defines a value with an associated name and notifies of the name or value change.
    /// </summary>
    public interface INamedObject : IValueObject

    {

        /// <summary>
        /// Gets or sets the name of this object.
        /// </summary>
        string Name { get; set; }

    }

    /// <summary>
    /// Provides an object that defines a value with an associated name and notifies of the name or value change.
    /// </summary>
    public interface INamedObject<T> : INamedObject, IValueObject<T>

    {

    }

    /// <summary>
    /// Provides an object that defines a value with an associated name and notifies of the name or value change.
    /// </summary>
    public class NamedObject : ViewModelBase, INamedObject
    {

        public bool IsReadOnly => false;

        private readonly string _name = null;

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name { get => _name; set => OnPropertyChanged(nameof(Name), nameof(_name), value, typeof(NamedObject)); }

        /// <summary>
        /// Determines whether this object is equal to a given object.
        /// </summary>
        /// <param name="obj">Object to compare to the current object.</param>
        /// <returns><see langword="true"/> if this object is equal to <paramref name="obj"/>, otherwise <see langword="false"/>.</returns>
        public bool Equals(WinCopies.Util.IValueObject obj) => new ValueObjectEqualityComparer().Equals(this, obj);

        private readonly object _value;

        /// <summary>
        /// Gets or sets the value of the object.
        /// </summary>
        public object Value { get => _value; set => OnPropertyChanged(nameof(Value), nameof(_value), value, typeof(NamedObject)); }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedObject"/> class.
        /// </summary>
        public NamedObject() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedObject"/> class using custom values.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public NamedObject(string name, object value)
        {

            _name = name;

            _value = value;

        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Removes the unmanaged resources and the managed resources if needed. If you override this method, you should call this implementation of this method in your override implementation to avoid unexpected results when using this object laater.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose managed resources, otherwise <see langword="false"/>.</param>
        protected virtual void Dispose(bool disposing)
        {

            if (disposedValue)

                return;

            if (Value is System.IDisposable _value)

                _value.Dispose();

            disposedValue = true;

        }

        ~NamedObject()
        {

            Dispose(false);

        }

        public void Dispose()
        {

            Dispose(true);

            GC.SuppressFinalize(this);

        }
        #endregion
    }

    /// <summary>
    /// Provides an object that defines a generic value with an associated name and notifies of the name or value change.
    /// </summary>
    /// <typeparam name="T">The type of the value of this object.</typeparam>
    public class NamedObject<T> : ViewModelBase, INamedObject<T>
    {

        public bool IsReadOnly => false;

        private readonly string _name = null;

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name { get => _name; set => OnPropertyChanged(nameof(Name), nameof(_name), value, typeof(NamedObject<T>)); }

        /// <summary>
        /// Determines whether this object is equal to a given object.
        /// </summary>
        /// <param name="obj">Object to compare to the current object.</param>
        /// <returns><see langword="true"/> if this object is equal to <paramref name="obj"/>, otherwise <see langword="false"/>.</returns>
        public bool Equals(WinCopies.Util.IValueObject obj) => new ValueObjectEqualityComparer().Equals(this, obj);

        /// <summary>
        /// Determines whether this object is equal to a given object.
        /// </summary>
        /// <param name="obj">Object to compare to the current object.</param>
        /// <returns><see langword="true"/> if this object is equal to <paramref name="obj"/>, otherwise <see langword="false"/>.</returns>
        public bool Equals(WinCopies.Util.IValueObject<T> obj) => new ValueObjectEqualityComparer<T>().Equals(this, obj);

        private readonly T _value;

        /// <summary>
        /// Gets or sets the value of the object.
        /// </summary>
        public T Value { get => _value; set => OnPropertyChanged(nameof(Value), nameof(_value), value, typeof(NamedObject)); }

        object WinCopies.Util.IValueObject.Value
        {

            get => _value; set

            {

                if (value is T _value)

                    Value = _value;

                else

                    throw new ArgumentException("Invalid type.", nameof(value));

            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedObject{T}"/> class.
        /// </summary>
        public NamedObject() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedObject{T}"/> class using custom values.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public NamedObject(string name, T value)
        {

            _value = value;

            _name = name;

        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Removes the unmanaged resources and the managed resources if needed. If you override this method, you should call this implementation of this method in your override implementation to avoid unexpected results when using this object laater.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose managed resources, otherwise <see langword="false"/>.</param>
        protected virtual void Dispose(bool disposing)
        {

            if (disposedValue)

                return;

            if (Value is System.IDisposable _value)

                _value.Dispose();

            disposedValue = true;

        }

        ~NamedObject()
        {

            Dispose(false);

        }

        public void Dispose()
        {

            Dispose(true);

            GC.SuppressFinalize(this);

        }
        #endregion
    }

}
