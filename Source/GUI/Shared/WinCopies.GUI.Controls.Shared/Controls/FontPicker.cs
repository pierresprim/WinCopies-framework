using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WinCopies.Collections.Generic;
using WinCopies.Desktop;
using WinCopies.Linq;
using WinCopies.Util.Data;

using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.Controls
{
    public class FontPicker : Control
    {
        protected class Option : CheckableNamedObjectViewModel
        {
            private readonly
#if CS8
                FuncNull
#else
                Func
#endif
                _func;

            public object
#if CS8
                ?
#endif
                Value => _func();

            public Option(in string name,
#if CS8
                FuncNull
#else
                Func
#endif
                whenTrue,
#if CS8
                FuncNull
#else
                Func
#endif
                whenFalse) : base(name ?? throw GetArgumentNullException(nameof(name))) => _func =
                whenTrue == null ? throw GetArgumentNullException(nameof(whenTrue)) :
                whenFalse == null ? throw GetArgumentNullException(nameof(whenFalse)) :
#if !CS9
                (
#if CS8
                FuncNull
#else
                Func
#endif
                )(
#endif
                () => IsChecked ?
                whenTrue() :
                whenFalse()
#if !CS9
                )
#endif
                ;

            protected override void OnStatusChanged()
            {
                base.OnStatusChanged();

                OnPropertyChanged(nameof(Value));
            }
        }

        private static DependencyPropertyKey RegisterReadOnly<T>(in string propertyName, in T defaultValue) => Util.Desktop.UtilHelpers.RegisterReadOnly<T, FontPicker>(propertyName, new PropertyMetadata(defaultValue));
        private static DependencyProperty Register<T>(in string propertyName, in T defaultValue) => Util.Desktop.UtilHelpers.Register<T, FontPicker>(propertyName, defaultValue);



        private static readonly DependencyPropertyKey FontFamiliesPropertyKey = RegisterReadOnly(nameof(FontFamilies), Fonts.SystemFontFamilies.OrderBy(f => f.Source));

        public static readonly DependencyProperty FontFamiliesProperty = FontFamiliesPropertyKey.DependencyProperty;

        public IOrderedEnumerable<FontFamily> FontFamilies { get => (IOrderedEnumerable<FontFamily>)GetValue(FontFamiliesProperty); set => SetValue(FontFamiliesPropertyKey, value); }

        public static readonly DependencyProperty SelectedFontFamilyProperty = Register<FontFamily>(nameof(SelectedFontFamily), new FontFamily("Segoe UI"));

        public FontFamily SelectedFontFamily { get => (FontFamily)GetValue(SelectedFontFamilyProperty); set => SetValue(SelectedFontFamilyProperty, value); }



        private static readonly DependencyPropertyKey OptionsPropertyKey = RegisterReadOnly(nameof(Options), new ReadOnlyArray<Option>(new Option("Bold", () => FontWeights.Bold, () => FontWeights.Normal), new Option("Italic", () => FontStyles.Italic, () => FontStyles.Normal), new Option("Underline", () => TextDecorations.Underline, () => null)));

        public static readonly DependencyProperty OptionsProperty = OptionsPropertyKey.DependencyProperty;

        public IReadOnlyList<ICheckableNamedObject> Options { get => (IReadOnlyList<ICheckableNamedObject>)GetValue(OptionsProperty); set => SetValue(OptionsPropertyKey, value); }



        private static readonly DependencyPropertyKey FontSizesPropertyKey = RegisterReadOnly(nameof(FontSizes), new double[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 });

        public static readonly DependencyProperty FontSizesProperty = FontSizesPropertyKey.DependencyProperty;

        public IEnumerable<double> FontSizes { get => (IEnumerable<double>)GetValue(FontSizesProperty); set => SetValue(FontSizesPropertyKey, value); }

        public static readonly DependencyProperty SelectedFontSizeProperty = Register<double>(nameof(SelectedFontSize), 12d);

        public double SelectedFontSize { get => (double)GetValue(SelectedFontSizeProperty); set => SetValue(SelectedFontSizeProperty, value); }



        static FontPicker() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<FontPicker>();
    }
}
