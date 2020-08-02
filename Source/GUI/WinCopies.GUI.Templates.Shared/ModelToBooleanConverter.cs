using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using WinCopies.GUI.Controls.Models;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Templates
{

    public enum ModelToBooleanConverterParameter

    {

        Header,

        Content,

        Items

    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class ModelToBooleanConverter : ConverterBase

    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)

        {

            if (value is IDataTemplateSelectorsModel _value && parameter is ModelToBooleanConverterParameter _parameter)

#if NETFRAMEWORK

                switch (_parameter)

                {

                    case ModelToBooleanConverterParameter.Header:

                        return !(_value.ModelDataTemplateSelectors?.HeaderDataTemplateSelector is null);

                    case ModelToBooleanConverterParameter.Content:

                        return !(_value.ModelDataTemplateSelectors?.ContentDataTemplateSelector is null);

                    case ModelToBooleanConverterParameter.Items:

                        return !(_value.ModelDataTemplateSelectors?.ItemDataTemplateSelector is null);

                    default:

                        return false;

                }

#else

                return _parameter switch
                {
                    ModelToBooleanConverterParameter.Header => !(_value.ModelDataTemplateSelectors?.HeaderDataTemplateSelector is null),
                    ModelToBooleanConverterParameter.Content => !(_value.ModelDataTemplateSelectors?.ContentDataTemplateSelector is null),
                    ModelToBooleanConverterParameter.Items => !(_value.ModelDataTemplateSelectors?.ItemDataTemplateSelector is null),
                    _ => false,
                };

#endif

            else

                return false;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    }

}
