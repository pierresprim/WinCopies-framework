﻿#pragma checksum "..\..\..\..\Themes\Generic.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "C523DD75B4A0C649F23DB7F78C14D785022F93A911DB8F5932EA3027CF583A93"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AttachedCommandBehavior;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WinCopies.GUI.Controls;
using WinCopies.GUI.Explorer;
using WinCopies.GUI.Explorer.Data;
using WinCopies.GUI.Windows;
using WinCopies.GUI.Windows.Dialogs;
using WinCopies.GUI.Windows.Dialogs.Data;
using WinCopies.IO;
using WinCopies.Util;
using WinCopies.Util.Commands;
using WinCopies.Util.DataConverters;


namespace WinCopies.GUI.Explorer.Themes {
    
    
    /// <summary>
    /// Generic
    /// </summary>
    public partial class Generic : System.Windows.ResourceDictionary, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WinCopies.GUI.Explorer;component/themes/generic.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Themes\Generic.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 1:
            
            #line 149 "..\..\..\..\Themes\Generic.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.PreviousButton_Click);
            
            #line default
            #line hidden
            break;
            case 2:
            
            #line 152 "..\..\..\..\Themes\Generic.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ForwardButton_Click);
            
            #line default
            #line hidden
            break;
            case 3:
            
            #line 178 "..\..\..\..\Themes\Generic.xaml"
            ((System.Windows.Controls.TextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.TextBox_KeyDown);
            
            #line default
            #line hidden
            
            #line 178 "..\..\..\..\Themes\Generic.xaml"
            ((System.Windows.Controls.TextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            
            #line 178 "..\..\..\..\Themes\Generic.xaml"
            ((System.Windows.Controls.TextBox)(target)).TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.PART_TextBox_TextChanged);
            
            #line default
            #line hidden
            break;
            case 4:
            
            #line 180 "..\..\..\..\Themes\Generic.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            break;
            case 5:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.FrameworkElement.ContextMenuOpeningEvent;
            
            #line 280 "..\..\..\..\Themes\Generic.xaml"
            eventSetter.Handler = new System.Windows.Controls.ContextMenuEventHandler(this.ListViewItem_ContextMenuOpening);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            }
        }
    }
}

