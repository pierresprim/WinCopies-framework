﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace WinCopies.Util.Data
{
    public class ConverterArrayParameter : MarkupExtension
    {

        public IEnumerable<IValueConverter> Converters { get; set; }

        public object Parameter { get; set; }

        public ConverterArrayParameter(IEnumerable<IValueConverter> converters, object parameter)

        {

            Converters = converters;

            Parameter = parameter;

        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;

    }

    public class ConverterArrayMultiParametersParameter : MarkupExtension
    {

        public IList<IValueConverter> Converters { get; set; }

        public IList<object> Parameters { get; set; }

        public ConverterArrayMultiParametersParameter(IList<IValueConverter> converters, IList<object> parameters)
        {

            if (converters.Count != parameters.Count) throw new ArgumentException("converters and parameters does not have the same number of parameters.");

            Converters = converters;

            Parameters = parameters;

        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class ConverterArray : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (parameter is ConverterArrayParameter converterArrayParameter)

            {

                foreach (IValueConverter converter in converterArrayParameter.Converters)

                    value = converter.Convert(value, ((ValueConversionAttribute)converter.GetType().GetCustomAttributes(typeof(ValueConversionAttribute), true).FirstOrDefault())?.TargetType ?? typeof(object), converterArrayParameter.Parameter, culture);

                return value;

            }

            if (parameter is ConverterArrayMultiParametersParameter converterArrayMultiParametersParameter)

            {

                IValueConverter converter = null;

                for (int i = 0; i < converterArrayMultiParametersParameter.Converters.Count; i++)
                {

                    converter = converterArrayMultiParametersParameter.Converters[i];

                    value = converter.Convert(value, ((ValueConversionAttribute)converter.GetType().GetCustomAttributes(typeof(ValueConversionAttribute), true).FirstOrDefault())?.TargetType ?? typeof(object), converterArrayMultiParametersParameter.Parameters[i], culture);

                }

                return value;

            }

            return null;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (parameter is ConverterArrayParameter converterArrayParameter)

            {

                foreach (IValueConverter converter in converterArrayParameter.Converters)

                    value = converter.ConvertBack(value, ((ValueConversionAttribute)converter.GetType().GetCustomAttributes(typeof(ValueConversionAttribute), true).FirstOrDefault())?.SourceType ?? typeof(object), converterArrayParameter.Parameter, culture);

                return value;

            }

            if (parameter is ConverterArrayMultiParametersParameter converterArrayMultiParametersParameter)

            {

                IValueConverter converter = null;

                for (int i = 0; i < converterArrayMultiParametersParameter.Converters.Count; i++)
                {

                    converter = converterArrayMultiParametersParameter.Converters[i];

                    value = converter.ConvertBack(value, ((ValueConversionAttribute)converter.GetType().GetCustomAttributes(typeof(ValueConversionAttribute), true).FirstOrDefault())?.SourceType ?? typeof(object), converterArrayMultiParametersParameter.Parameters[i], culture);

                }

                return value;

            }

            return null;

        }
    }

    public class MultiConverterArrayParameter : MarkupExtension
    {

        public IMultiValueConverter Converter { get; set; }

        public IEnumerable<IValueConverter> Converters { get; set; }

        public object Parameter { get; set; }

        public MultiConverterArrayParameter(IEnumerable<IValueConverter> converters, IMultiValueConverter converter, object parameter)

        {

            Converter = converter;

            Converters = converters;

            Parameter = parameter;

        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;

    }

    public class MultiConverterArrayMultiParametersParameter : MarkupExtension
    {

        public IMultiValueConverter Converter { get; set; }

        public IList<IValueConverter> Converters { get; set; }

        public object Parameter { get; set; }

        public IList<object> Parameters { get; set; }

        public MultiConverterArrayMultiParametersParameter(IList<IValueConverter> converters, IMultiValueConverter converter, object parameter, IList<object> parameters)
        {

            if (converters.Count != parameters.Count) throw new ArgumentException("converters and parameters does not have the same number of parameters.");

            Converter = converter;

            Converters = converters;

            Parameter = parameter;

            Parameters = parameters;

        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class MultiConverterArray : MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (parameter is MultiConverterArrayParameter multiConverterArrayParameter)

            {

                object value = null;

                for (int i = 0; i < values.Length; i++)

                {

                    value = values[i];

                    foreach (IValueConverter converter in multiConverterArrayParameter.Converters)

                        value = converter.Convert(value, ((ValueConversionAttribute)converter.GetType().GetCustomAttributes(typeof(ValueConversionAttribute), true).FirstOrDefault())?.TargetType ?? typeof(object), multiConverterArrayParameter.Parameter, culture);

                    values[i] = value;

                }

                value = multiConverterArrayParameter.Converter.Convert(values, ((MultiValueConversionAttribute)multiConverterArrayParameter.Converter.GetType().GetCustomAttributes(typeof(MultiValueConversionAttribute), true).FirstOrDefault())?.TargetType ?? typeof(object), multiConverterArrayParameter.Parameter, culture);

                return value;

            }

            if (parameter is MultiConverterArrayMultiParametersParameter multiConverterArrayMultiParametersParameter)

            {

                IValueConverter converter = null;

                object value = null;

                for (int i = 0; i < values.Length; i++)

                {

                    value = values[i];

                    for (int j = 0; j < multiConverterArrayMultiParametersParameter.Converters.Count; j++)
                    {

                        converter = multiConverterArrayMultiParametersParameter.Converters[j];

                        value = converter.Convert(value, ((ValueConversionAttribute)converter.GetType().GetCustomAttributes(typeof(ValueConversionAttribute), true).FirstOrDefault())?.TargetType ?? typeof(object), multiConverterArrayMultiParametersParameter.Parameters[j], culture);

                    }

                    values[i] = value;

                }

                value = multiConverterArrayMultiParametersParameter.Converter.Convert(values, ((MultiValueConversionAttribute)converter.GetType().GetCustomAttributes(typeof(MultiValueConversionAttribute), true).FirstOrDefault())?.TargetType ?? typeof(object), multiConverterArrayMultiParametersParameter.Parameter, culture);

                return value;

            }

            return null;

        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {

            if (parameter is MultiConverterArrayParameter multiConverterArrayParameter)

            {

                object[] values = null;

                Type[] types = ((MultiValueConversionAttribute)multiConverterArrayParameter.Converter.GetType().GetCustomAttributes(typeof(MultiValueConversionAttribute), true).FirstOrDefault())?.SourceTypes;

                if (types == null)

                {

                    types = new Type[targetTypes.Length];

                    for (int i = 0; i < targetTypes.Length; i++)

                        types[i] = typeof(object);

                }

                values = multiConverterArrayParameter.Converter.ConvertBack(value, types, multiConverterArrayParameter.Parameter, culture);

                for (int i = 0; i < values.Length; i++)

                {

                    value = values[i];

                    foreach (IValueConverter converter in multiConverterArrayParameter.Converters)

                        value = converter.ConvertBack(value, ((ValueConversionAttribute)converter.GetType().GetCustomAttributes(typeof(ValueConversionAttribute), true).FirstOrDefault())?.TargetType ?? typeof(object), multiConverterArrayParameter.Parameter, culture);

                    values[i] = value;

                }

                return values;

            }

            if (parameter is MultiConverterArrayMultiParametersParameter multiConverterArrayMultiParametersParameter)

            {

                object[] values = null;

                Type[] types = ((MultiValueConversionAttribute)multiConverterArrayMultiParametersParameter.Converter.GetType().GetCustomAttributes(typeof(MultiValueConversionAttribute), true).FirstOrDefault())?.SourceTypes;

                if (types == null)

                {

                    types = new Type[targetTypes.Length];

                    for (int i = 0; i < targetTypes.Length; i++)

                        types[i] = typeof(object);

                }

                values = multiConverterArrayMultiParametersParameter.Converter.ConvertBack(value, types, multiConverterArrayMultiParametersParameter.Parameter, culture);

                for (int i = 0; i < values.Length; i++)

                {

                    value = values[i];

                    for (int j = 0; j < multiConverterArrayMultiParametersParameter.Converters.Count; j++)
                    {
                        IValueConverter converter = multiConverterArrayMultiParametersParameter.Converters[j];
                        value = converter.ConvertBack(value, ((ValueConversionAttribute)converter.GetType().GetCustomAttributes(typeof(ValueConversionAttribute), true).FirstOrDefault())?.TargetType ?? typeof(object), multiConverterArrayMultiParametersParameter.Parameters[j], culture);
                    }

                    values[i] = value;

                }

                return values;

            }

            return null;

        }
    }
}
