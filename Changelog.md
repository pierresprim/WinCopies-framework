WinCopies-framework
===================

The WinCopies® software framework

CHANGELOG
---------

### 3.2.0-preview

- WinCopies.GUI.Windows.Window:
	- Close Win32 system command can be disabled using dependency property.
	- ShowHelpButton is now HelpButton.
	- Bug fixes.
- ProcessWindow inherits from WinCopies.GUI.Windows.Window.
- Add NavigationButton control.
- IButtonModel implements ICommandSource.
- IButtonModel\<T> is now defined as: IButtonModel<TContent, TCommandParameter> : IButtonModel, ICommandSource<TCommandParameter>, IContentControlModel<TContent>.
- Update generic models and view models for ICommandSource support.
- Add history management to ExplorerControl and (I)ExplorerControlBrowsableObjectInfoViewModel.

### 04/26/2021 3.1.0-preview

- Complete rewrite of Processes, file system enumeration and (I)PathInfo (related-)types.
- Add ListView control.
- Remove IBrowsableObjectInfoSelectorDictionary\<T> and BrowsableObjectInfoSelectorDictionary generic and non-generic types. Use the following WinCopies namespace (WinCopies.Util.Extensions package):
	- ISelectorDictionary
	- IEnumerableSelectorDictionary
	- SelectorDictionary
	- SelectorDictionary\<TIn, TOut>
	- EnumerableSelectorDictionary
- WinCopies.GUI.IO.ObjectModel.ExplorerControlBrowsableObjectInfoViewModel's constructor is now protected.

### 03/02/2021 3.0.0-preview

Complete rewrite.

### 07/20/2020 2.5.9-preview

#### WinCopies.IO & WinCopies.GUI.IO 2.5.9-preview

Misc BrowsableObjectInfo API updates, such as:

- (equality) comparison
- portable device browsing.
- PortableDeviceItemInfo is now called PortableDeviceObjectInfo in order to have the same nomenclature as the ShellObjectInfo class.
- PortableDevice(Object)Info.LocalizedName now return the same value as the Name property.

### 07/06/2020 2.5.8-preview1

#### WinCopies.IO 2.5.8-preview1

- Existing item changes:
	- LocalizedName and Description properties return N/A for ArchiveItemInfo
	- FileSystemObjectInfo.FileType is now abstract.
	- ShellObjectInfo.GetItems(Predicate<ShellObject> func):
		- func has been redefined as: Predicate<ShellObjectInfoEnumeratorStruct>
		- This method now returns PortableDeviceInfo when ShellObjectInfo represents the Computer virtual folder.
	- Performance upgrades.
- Removals:
	- FileTypes flags enum.
	- FileSystemObjectInfo's constructor does not have a fileType parameter anymore.
- Additions:
	- (I)PortableDevice(Object)Info
	- ShellObjectInfoEnumeratorStruct for ShellObjectInfo.GetItems' predicate
	- Path.PathSeparator const. Fixes #11
	- Public (static) methods regarding item type name and icon generation in FileSystemObjectInfo.
- Bug fixes.

#### WinCopies.GUI.Templates 2.5.8-preview

- Additions:
	- DataTemplate for ImageSource

### 05/25/2020 2.3.0-preview5

- Depends on WinCopies.Util 2.3.0-preview5

#### WinCopies.IO (2.3.0-preview5)

- Existing item updates:
	- The Size struct's inner value is now of the WinCopies.Util.CheckedUInt64 type.
	- The ArchiveItemInfoEnumerator class now implements the IEnumerable<IBrowsableObjectInfo> interface.
	- A factory which can be used in the ExplorerControlBrowsableObjectInfoViewModel class to create custom BrowsableObjectInfo(ViewModel)s.
	- Fixation of #6.
- Additions:
	- Description property to the IBrowsableObjectInfo interface.
	- Registry browsing (fixes #4):
		- (I)RegistryItemInfo interface and class.
		- RegistryItemType enum.
		- Registry static class.
		- RegistryException.
	- WMI browsing (fixes #5):
		- (I)WMIItemInfo interface and class.
		- WMIItemInfoEnumerator class.
		- (I)WMIItemInfoFactory(Options) classes and interfaces.
- Removals:
	- IBrowsableObjectInfo.GetItems(Predicate<IBrowsableObjectInfo> func) method.
	- WinCopies.IO.Path.PathSeparator const.

### 05/07/2020 2.2.0-preview4

- .Net Core and .Net Standard are now supported.

#### WinCopies.Data

- Package now depends on Newtonsoft.Json v9.0.1

#### WinCopies.GUI.Windows

- Package now depends on WinCopies.WindowsAPICodePack.Shell v2.0.0-preview6

#### First releases

- WinCopies.IO
- WinCopies.GUI.Controls
- WinCopies.GUI.IO
- WinCopies.GUI.Icons

### 02/09/2020 2.1

#### WinCopies.Util

Available for .Net Framework, .Net Core and .Net Standard*

- Existing items behavior updates:
	- This assembly now targets the 4.7.2 version of the .Net Framework instead of version 4.8 for the .Net Framework version of this assembly.

- Addings:
	- Interfaces:
		- IUIntIndexedCollection and IUIntIndexedCollection<T\>
		- UIntIndexedCollectionEnumeratorBase, UIntIndexedCollectionEnumerator and UIntIndexedCollectionEnumerator<T\>
		- WinCopies.Util.DotNetFix.IDisposable

- Removals:
	- WinCopies.Util.Extensions.ToImageSource(this Bitmap bitmap); this method was only available for the 2.0.0 version for the .Net Framework and has now been removed.

\* Some features are not available in the .Net Core and .Net Standard versions since these frameworks do not have the same structure as the .Net Framework. New packages that include these features will be released later.

### 2.0

#### WinCopies.Util

Available for .Net Framework, .Net Core and .Net Standard*

- Existing items behavior updates:
	- The view models OnPropertyChanged methods do not update the properties or fields anymore ; this feature has been replaced by the 'Update' methods added in the same classes.
	- WinCopies.Util.BackgroundWorker class:
		- The BackgroundWorker class now resets its properties in background.
		- If a ThreadAbortException is thrown, and is not caught, in the background thread, the BackgroundWorker will consider that a cancellation has occurred.
		- Info can now be passed to the Cancel and CancelAsync methods.
	- The 'If' methods perform a real 'xor' comparison in binary mode and are faster.
	- The 'If' methods now set up the out 'key' parameter with the second value and predicate pair that was checked, if any, instead of the default value for the key type when performing a 'xor' comparison.
	- The ApartmentState, WorkerReportsProgress and WorkerSupportsCancellation properties of the IBackgroundWorker interface are now settable.
	- The IsNullConverter class now uses the 'is' operator instead of '=='.
	- The WinCopies.Util.Data.ValueObject now implements the WinCopies.Util.IValueObject generic interface.**
	- The WinCopies.Util.IValueObject interface implements IDisposable, so all classes that implements the WinCopies.Util.IValueObject are also disposable.
	- The following items have been moved to the WinCopies.Collections.DotNetFix namespace:
		- NotifyCollectionChangedEventArgs
		- ObservableCollection:
			- ObservableCollection classes:
				- Now call base methods for avoinding reentrancy.
				- Now have the Serializable attribute
				- Now implement the IObservableCollection interface
		- ReadOnlyObservableCollection:
			- The ReadOnlyObservableCollection classes:
				- Now have the Serializable attribute.
	- The ConverterArrayParameter and ConverterArrayMultiParametersParameter classes can now be used in XAML.
	- The IsNullConverter now supports setting the parameter of the binding to true to get a reversed boolean.

- Obsolete items:
	- Classes and interfaces:
		- WinCopies.Util.Data.IValueObjects generic and non-generic:
			- are now obsoletes and have been replaced by the WinCopies.Util.IValueObject interfaces.**;
			- now inherit from the WinCopies.Util.IValueObject interfaces.**
		- WinCopies.Util.Data.CheckableObjects generic and non-generic:
			- are now obsoletes and have been replaced by the corresponding models and view models of the new WinCopies.GUI.Models and WinCopies.GUI.ViewModels packages.
		- (I)ReadOnlyArrayList
		- The Generic class is being replaced by the Resources class and will be removed in later versions.
	- Extension methods:
		- static bool ContainsOneValue(this IEnumerable array, Comparison<object\> comparison, out bool containsMoreThanOneValue, params object[] values) IEnumerable extension method (replaced by ContainsOneValue(this IEnumerable array, WinCopies.Collections.Comparison comparison, out bool containsMoreThanOneValue, params object[] values)).
		- static object GetNumValue(this Enum @enum, string enumName) Enum extension method. Replaced by:
			- GetNumValue(this Enum @enum)
			- WinCopies.Util.GetNumValue(Type enumType, string fieldName)
		- static bool Contains(this string s, IEqualityComparer<char\> comparer, string value) (replaced by Contains(this string s, string value, IEqualityComparer<char\> comparer)).
		- static bool Contains(this string s, char value, IEqualityComparer<char\> comparer, out int index) (replaced by array-common methods).
	- Util methods:
		- static (bool propertyChanged, object oldValue) SetPropertyWhenNotBusy<T\>(T bgWorker, string propertyName, string fieldName, object newValue, Type declaringType, BindingFlags bindingFlags = DefaultBindingFlagsForPropertySet, bool throwIfBusy = true) where T : IBackgroundWorker, INotifyPropertyChanged (replaced by the WinCopies.Util.Extensions.SetBackgroundWorkerProperty method overloads).
		- Static If methods with object-generic delegates have been replaced by ones with new non-generic delegates.
	- Misc:
		- The resources are now available from the new Resources static class.
		- The Microsoft.Shell namespace members are now available from the version 1.1.0 and later of the following NuGet package: https://www.nuget.org/packages/WinCopies.WindowsAPICodePack.Win32Native
		- The WinCopies.Util.Commands.ApplicationCommands.CloseWindow is now obsolete. Please use the System.Windows.Input.ApplicationCommands.Close command instead.

- Addings:
	- Classes:
		- EnumComparer
		- ArrayBuilder class to build arrays, lists and observable collections.
		- Comparer classes and interfaces for sorting support.
		- ValueObjectEqualityComparer
		- TreeNode
		- InterfaceDataTemplateSelector
		- WinCopies.Collections.DotNetFix.LinkedList
		- WinCopies.Collections.LinkedList
		- ReadOnlyLinkedList
		- EnumeratorCollection
		- MergedStylesExtension
		- Resources static class
	- Interfaces:
		- IDeepCloneable
		- IDisposable
		- IObservableCollection
		- IReadOnlyObservableCollection
		- WinCopies.Util.IValueObject
		- ITreeNode
		- IReadOnlyTreeNode
		- IObservableTreeNode
		- IReadOnlyObservableTreeNode
		- ILinkedList
	- Delegates:
		- EqualityComparison
		- FieldValidateValueCallback***
		- FieldValueChangedCallback***
		- PropertyValidateCallback***
		- PropertyValueChangedCallback***
		- ActionParams
		- Func
		- FuncParams
	- Methods:
		- Static methods:
			- 'SetField' static method
			- 'Between' static methods for the other numeric types
			- 'ThrowIfNull' static method
			- 'GetOrThrowIfNotType' static method
			- 'GetIf' methods
			- ThrowOnInvalidCopyToArrayOperation method
		- Extension methods:
			- ToStringWithoutAccents string extension method
			- Extension methods for LinkedLists
			- Extension methods for setting properties in BackgroundWorkers with an is-busy check.
			- Extension method for throwing if an object that implements IDisposable is disposing or disposed.
			- AsObjectEnumerable extension method to yield return items from an IEnumerable as items of an IEnumerable<T\>.
			- FirstOrDefault IEnumerable extension method. This is a generic method that looks for the first item of the given generic type parameter. If none item is found, the method returns the default value for the given generic type parameter.
			- LastOrDefault IEnumerable extension method. Same method as the FirstOrDefault method but to get the last item of an IEnumerable instead of the first one.
		- Misc:
			- Update methods in the view model classes to replace the update feature of the OnPropertyChanged methods of these classes. The OnPropertyChanged methods still exist in these classes, but now just raise the PropertyChanged event.
			- The view model classes now have an OnAutoPropertyChanged method to automatically set an auto-property and raise the PropertyChanged event.
			- The ReadOnlyObservableCollection has now an OnCollectionChanging protected virtual method.
	- Parameters:
		- The WinCopies.Util.Extensions.SetProperty/Field now have multiple new optional parameters to extend the capabilities of these methods.
	- Structures:
		- ValueObjectEnumerator structure
		- WrapperStructure structure

- Bug fixes:
	- BackgroundWorker class: when aborting, the RunWorkerCompleted event was raised twice.
	- BackgroundWorker class: when finalizing, an invalid operation exception was thrown if the BackgroundWorker was busy; now, the BackgroundWorker aborts the working instead of throwing an exception.
	- Is extension method: error when setting the typeEquality parameter to false.
	- String extension methods: unexpected results.

- Removals:
	- The 'performIntegrityCheck' parameter in 'SetProperty' methods has been replaced by the 'throwIfReadOnly' parameter.

- Misc:
	- ReadOnlyObservableCollection's CollectionChanging event has now the protected access modifier.
	- Some code now uses the 'in' parameter modifier.
	- The dependency package System.Windows.Interactivity.WPF has been replaced by the https://www.nuget.org/packages/Microsoft.Xaml.Behaviors.Wpf package.
	- Move resources from Generic.xaml to assembly's resource file.
	- Update doc.

\* Some features are not available in the .Net Core and .Net Standard versions since these frameworks do not have the same structure as the .Net Framework. New packages that include these features will be released later.

\*\* This also applies to the other already existing classes and interfaces, in the previous versions, that inherit from or implement these classes and interfaces.

\*\*\* See WinCopies.Util.Extensions.SetProperty/Field

#### First releases:

- WinCopies.Data
- WinCopies.GUI
- WinCopies.GUI.Models
- WinCopies.GUI.ViewModels
- WinCopies.GUI.Templates
- WinCopies.GUI.Windows

Project link
------------

[https://github.com/pierresprim/WinCopies-framework](https://github.com/pierresprim/WinCopies-framework)

License
-------

See [LICENSE](https://github.com/pierresprim/WinCopies-framework/blob/master/LICENSE) for the license of the WinCopies framework.

This framework uses some external dependencies. Each external dependency is integrated to the WinCopies framework under its own license, regardless of the WinCopies framework license.