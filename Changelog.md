WinCopies-framework
===================

The WinCopies® software framework

CHANGELOG
---------

### 3.18-preview

- Add:
	- new projects
	- support for .Net 8
	- new types
- Update to latest frameworks.

#### WinCopies.Installer

##### WinCopies.Installer namespace

- Remove the CommonInstallerPageBase\<TNext, TData>.Icon property.
- File struct:
	- implements the new IFile interface.
	- has a new read-only property (string Name). The structure constructor has been updated consequently.
- Add a new property to InstallerPage: Icon.
- Redesign IProcessData.
- Default/ProcessData are now located directly in the current namespace.
- ProcessData:
	- the Files property type is IEnumerable\<KeyValuePair\<string, IFile>>.
	- add new properties and update methods.
- IProcessDataViewModel:
	- the CurrentItemProgress, Log, and OverallProgress properties are read-only.
	- has new read-only properties string StepName and byte StepData.
- IInstaller: new properties.
- Installer:
	- the Files property type is now IEnumerable\<KeyValuePair\<string, IFile>>.
	- GetStartPage is now called only on the first call to the StartPage property. It was previous called by the constructor.
- ProcessPageViewModel:
	- ProcessDataViewModel:
		- BackgroundWorker: new protected virtual method: DoExtraWork(DoWorkEventArgs e).
		- the OverallProgress and CurrentItemProgress property setters are now protected.
		- the Log property is now read-only.
		- the GetEnumerator() method return type is now IEnumerator\<KeyValuePair\<string, IFile>>.
		- has new properties and methods.
	- implements IProcessPageViewModel.
- UserGroupPageViewModel.UserGroupDataViewModel: new property: object ExtraData.
- InstallerViewModel:
	- is now abstract
	- has new properties
	- has a new method: protected virtual IProcessPageViewModel GetProcessPageViewModel(IProcessPage processPage).
	- the IInstaller.Current explicit property implementation setter now throws a NotSupportedException instead of an InvalidOperationException.

##### Misc

- WinCopies.Installer.GUI:
	- Destination: removed the Reset() and AddCommandBindings() methods. More generally, destination path handling itself was delegated to the new BrowseTextBox/Base types.
	- Destination and UserGroup types have a new dependency property: object ExtraData.
	- Process has new dependency properties: string StepName and byte StepData.
- Update XAML views.

### 3.17.1.2-preview

Bug fixes in WinCopies.Installer package:
	- The resource manager was internal instead of public.
	- ProcessPage.DefaultProcessData: GetResources method threw a NullArgumentException when RelativePathResourcesType returns a null value.

### 3.17.1.1-preview

- Updated resources.

### 3.17.1-preview

- Bugs fixes:
	- ExplorerControlViewModel.OnGoCommandExecuted: the method was not handling path or plugin not found errors.
	- WinCopies.GUI.IO.Application: BrowsableObjectInfoSelectors were not registered.
	- BrowsableObjectInfoWindow's protected constructor has a new parameter that indicates whether a default tab should be added automatically.
	- BrowsableObjectInfoURL2(in string path) constructor: the given path was not correctly parsed.
	- WinCopies.IO.Shell.ObjectModel.BrowsableObjectInfo.RegisterDefaultBrowsableObjectInfoSelector: the Action value of the property was pushing a predicate with a wrong type to check for constants.
	- WinCopies.IO.Shell.BrowsableObjectInfoPlugin's constructor: WinCopies.IO.Shell.ObjectModel.BrowsableObjectInfo.RegisterDefaultBrowsableObjectInfoSelector was not registered.
	- WinCopies.IO.Consts.Protocols has new constant (FILE). The absence of this constant was leading to a bug when looking for a BrowsableObjectInfo to handle the file:// protocol.
- WinCopies.IO.ObjectModel.BrowsableObjectInfo:
	- new static method: PromptPathNotFound.
	- Create method has new parameters.
- WinCopies.IO.Consts.Protocols has been moved to WinCopies.IO.Consts.Shell namespace.

### 3.17-preview

- WinCopies.GUI.IO:
	- Bug fixed in BitmapSources loader. An InvalidCastException was thrown when the application definition type did not inherit from the default class.
	- new interface: IApplication. Application implements it.
	- Application:
		- Logger is now a read-only property instead of a read-only field.
		- new static method: Initialize. This method can be used to initialize application definitions that do not inherit from the default class. They have to implement IApplication to be passed to this method.
	- ExplorerControlViewModel: new constructor.
- WinCopies.GUI.Shell: remove ExplorerControlViewModel class. Its method has been replaced with the new constructor added to the class of the same name in the WinCopies.GUI.IO namespace and package.
- WinCopies.IO.ObjectModel.BrowsableObjectInfo: new static method: Create. This method returns an IBrowsableObjectInfo from a given path, based on loaded plugins.

### 3.16-preview

- Add new type: WinCopies.GUI.IO.Application.
- WinCopies.IO package:
	- WinCopies.IO.Shell.ComponentSources.Bitmap.BitmapSourceProvider is now static. New method overloads (Create) replace the constructors.
	- PluginInfo\<T>: BitmapSourceProviderOverride always returns a non-null value. If InnerObjectGenericOverride.BitmapSourceProvider returns a null value, then the property returns a default value.
- WinCopies.IO.Shell package: BitmapSourcesStruct and BitmapSources types, both in the WinCopies.IO.Shell.ComponentSources.Bitmap namespace are now in the WinCopies.IO package.
- WinCopies.GUI.Models package: depends on WinCopies.Util 3.18 instead of 3.18-preview.

### 3.15-preview

- Add plugin support for WinCopies IO Framework based applications.
- New packages:
	- WinCopies.Installer: base package for GUI installer creation.
	- WinCopies.IO.SQL(.Common): base package for SQL plugins (-.SQL) and effective plugin (-.Common) for WinCopies IO Framework based applications.
- WinCopies.IO.Shell package split into three packages: itself, which contains effective IO code, WinCopies.IO.Drawing for drawing operations and WinCopies.IO.Reflection for reflection support. The last two are not yet functional.
- New controls.
- Completely redesign SQL, IO and doc building APIs.
- .Net 6+: bug fixed in LinkRun: process runner restored to the same as for .Net 5.
- InterfaceDataTemplateSelector XAML Static Resource: key renamed WinCopies.Templates.InterfaceDataTemplateSelector.
- TabControl style no longer has a key.
- New styles and templates.
- All XAML items are now shared.
- Add nullable attributes.
- EntityPropertyAttribute: IsId and IsPseudoId properties replaced with the new IdStatus property, for which the value type is the new IdStatus enumeration.
- PackageLoadContext moved to WinCopies.Util package. It is available starting with CS8 and was renamed AssemblyLoadContext.
- DotNetEnumUnderlyingType enumeration: underlying type is byte.
- DotNetType has a new property: Namespace.

### 3.14-preview

- New types.
- IEntityCollection/\<T>: implement respectively IDisposableEnumerable/\<T>.
- DBEntity\<T> has a new constructor.
- DBEntityCollection\<T> and Writer: new static methods.
- Writer.UpdateItems\<T, U>(in string, bool, Predicate\<T>, Action\<T, U>?, Action\<T>?, Converter\<T, Type>, in Func\<IEnumerable\<U>>, in Func\<U, Type, DBEntityCollection\<T>, T>, Action\<U, T, DBEntityCollection\<T>>, Action\<U, ulong>, Action\<UpdateItemsStruct>?) has a new parameter: in bool isGenericType.
- DotNetEnum: new method GetEnumUnderlyingType().
- Add support for constants in doc builder.
- Bug fixed in IO commands.

### 3.13.2-preview

- New types.

- MySQLConnectionConstants:
	- was renamed to MySQLConnectionHelper.
	- EQUAL and IS moved to new type WinCopies.Data.SQL.SQLConstants.
- MySQLConnection:
	- PrepareCommand(MySqlCommand, IEnumerable\<IParameter>) was renamed to PrepareCommandParameters. There is still a method named PrepareCommand, but it will also prepare the command itself.
- SQLConnection.UseDBOverride returns a uint? value.
- (I)(My)SQLConnection: new properties and methods.

- BrowsableObjectInfoWindow: update window design.

- DocBuilder:
	- Writer has new methods, some of them can be called directly in a Main procedure.
	- bug fixed: non public or non protected parent classes were processed when a nested type was.

### 3.13.1-preview

- Move AppBrowsableObjectInfo\<T>, PluginInfo\<T>, BrowsableObjectInfoStartPage\<T>, to WinCopies.IO.Shell.ObjectModel.
- BrowsableObjectInfoWindow: bug fixed in Paste command handling.
- ExplorerControlViewModel: bug fixed: it was the view model that added to the history instead of the inner object.

### 3.13-preview

- Add:
	- support for .Net 7.
	- new types.
	- new static and extension methods.
	- new constants.
	- new WPF templates, styles, resources and controls.
	- new packages.
	- IBrowsableInfoObjects that represent the application root path and an application plugin.
	- (Preview)TripleClick routed events to WinCopies.GUI.Controls.DotNetFix.TextBox.
	- a sample DLL for doc building testing.
- Bug fixes and misc improvements.
- Update:
	- WinCopies.GUI.Shell.ObjectModel.ExplorerControlViewModel methods.
	- BrowsableObjectInfoWindowTemplate display style.
	- BrowsableObjectInfoWindow(Template) to display context menu info tips in the status bar.
	- BrowsableObjectInfoWindow:
		- add property: StatusBarLabel.
		- all constructors are protected.
		- handles the BrowseToParent command (from Util/Desktop).
- Remove:
	- ExplorerControlListViewContextMenuRequestedEventArgs (replaced with ContextMenuRequestedEventArgs).
	- ExplorerControlListView:
		- ContextMenuRequestedEvent
		- ContextMenuRequested
		as they are not used in the new implementation.
- InputBox: add new ShowDialog method overload.
- Window:
	- inherits from GlassWindow from WAPICP.
	- handles Alt+Up keyboard shortcut as the BrowseToParent command from Util/Desktop.
	- bugs fixed in window system menu handling.
- WinCopies.IO.ObjectModel.(I)BrowsableObjectInfo: new properties and methods.
- IShellObjectInfoBase:
	- add IsArchiveOpen property.
	- replace ArchiveFileStream property with GetArchiveFileStream() method. The return type now is StreamInfo.
- IExplorerControlViewModel:
	- SelectedItems has new return type.
	- SelectedItems2 has been removed.
- IBrowsableObjectInfoViewModel has new events.
- BrowsableObjectInfoViewModel has new methods and events.
- BrowsableObjectInfoCollectionViewModel has new constructors.
- IBrowsableObjectInfoFactory has a new method.
- IExplorerControlViewModel: new property: OpenInNewContextCommand.
- ExplorerControlViewModel:
	- New property: OpenInNewContextCommand.
	- New method: OnPathSelectedItemsChanged.
	- OnTryExecuteCommand and OnPathAdded has new parameters.
	- Selected items are cleared when disposing.
	- Update OnCreateNew/RenameItem method definitions.
	- Root items can be updated depending on the current item.
	- Bugs fixed in command handling.
- ExplorerControl:
	- OnGoToPageCanExecute(CanExecuteRoutedEventArgs e): bug fixed: an exception was thrown when 'e' is null.
	- Update default list display style.
- (I)ExplorerControlViewModel and ExplorerControl:
	- History property type is ReadOnlyHistoryObservableCollection<IBrowsableObjectInfo>.
- ExplorerControlListViewItem:
	- inherits from the new ListViewItem control instead of the system one.
	- has new:
		- routed event: ContextMenuRequested.
		- method(s) (overrides) and behavior to handle mouse right click and Apps key down events, and raise the new ContextMenuRequested consequently.
- ExplorerControlListViewItemContent has new properties for other image sizes.
- FileSystemDialogBoxPredicateConverter.GetPredicate's 'mode' method parameter no longer has the 'in' modifier.
- IHistoryCollection.CanMovePrevious/NextFromCurrent => CanMoveBack/Forward
- BrowsableObjectInfoWindow: bugs fixed in command handling.
- BrowsableObjectInfoWindowTemplate: remove 'new tab' menu items. They have been replaced with a single one.
- WinCopies.GUI.Shell.ObjectModel:
	- ArchiveProcessInitializer\<T> has been moved to the WinCopies.GUI.Shell namespace.
	- redesign BrowsableObjectInfo.
- Redesign WinCopies.IO.BitmapSources-related types.
- Redesign ArchiveFileInfoEnumeratorStruct.
- Redesign ShellObjectInfo types + bug fixes.
- Redesign non-WinCopies.IO.ObjectModel BrowsableObjectInfo-related types.
- BrowsableObjectInfoPlugin: new abstract property and methods.
- SingleIconInfo returns its parent IBrowsableObjectInfo as root items.
- WinCopies.IO.ObjectModel.RegistryItemInfo\<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>'s constructors are protected.
- WinCopies.IO.Guids.Shell.Process has been moved to WinCopies.IO.Consts.Guids.Shell.

### 3.12.1-preview

- Add constructor to ProcessWindow with a custom ObservableCollection\<IProcess>.
- Bug fixed in process template: the SourcePath property data binding mode was set to OneTime.
- Remove PathCommonToStringConverter.

### 3.12-preview

- Update monitoring.
- Add file system dialogs.
- WinCopies.GUI.Shell.ObjectModel.ExplorerControlBrowsableObjectInfoViewModel:
	- => ExplorerControlViewModel
	- constructor is now protected.
- WinCopies.GUI.IO.ObjectModel:
	- IExplorerControlBrowsableObjectInfoViewModel:
		- => IExplorerControlViewModel
		- add Start/StopMonitoring() methods.
	- ExplorerControlBrowsableObjectInfoViewModelStruct:
		- => ExplorerControlViewModelStruct
		- is now in the WinCopies.GUI.IO namespace.
	- ExplorerControlBrowsableObjectInfoViewModel
		- => ExplorerControlViewModel
		- some properties throw an exception when attempting to get or set value.
	- BrowsableObjectInfoViewModel:
		- add Filter property and new methods
		- an exception is thrown when attempting to get or set some properties when the object is disposed.
	- (I)(ExplorerControl)BrowsableObjectInfoViewModel: new properties and methods.
- Add DialogWindowBase class. Some items of DialogWindow have been moved to this new class.
- Move IO Guids to WinCopies.IO.Guids.Shell namespace (WinCopies.IO.Shell assembly).
- Add WinCopies.IO.Path.Match method.
- New extension methods.
- Update (I)MenuItemModel interfaces and classes.
- IBrowsableObjectInfoCollectionViewModel:
	- Paths property type is now IList<IExplorerControlViewModel> instead of ICollection<IExplorerControlViewModel>.
	- New property (IsCheckBoxVisible).
- Bug fixes when:
	- disposing BrowsableObjectInfoCollectionViewModel.
	- cancelling generation of custom process parameters.
	- ExplorerControlViewModel: selected items are cleared when disposing.
- Bug fixed in:
	- WinCopies.IO.PathTypes\<T>.PathInfo constructors: an ArgumentNullException was thrown when attempting to initialize a root path.
	- Archive compression.
- (I)ExplorerControlViewModel:
	- CustomProcessParametersGeneratedEventHandler => CustomProcessParametersGenerated
	- constructor: path cannot be null.
	- new properties and methods.
- CustomProcessParametersGeneratedEventArgs:
	- add Process property and 'process' parameter to constructor.
- Re-design WinCopies.GUI.Shell.BrowsableObjectInfo- view models, BrowsableObjectInfoWindow and types related to archive process.
- MovingRecursivelyEnumerablePath => MoveRecursivelyEnumerablePath.
- WinCopies.IO.ObjectModel.(I)BrowsableObjectInfo: add methods.
- WinCopies.IO.Process:
	- ObjectModel.ProcessObjectModelTypes<TItemsIn, TItemsOut, TFactory, TError, TAction, TProcessDelegates, TProcessEventDelegates, TProcessDelegateParam>:
		- Process class:
			- some previously abstract methods have now a default implementation.
			- Convert methods changed.
		- DefaultProcesses<TOptions>: DefaultProcess2 and DefaultDestinationProcess2 have a default implementation for some methods of the Process class.
	- The content of the IProcessFactoryProcessInfo interface is now in the new IProcessFactoryProcessInfoBase interface.
	- IProcessFactory: new property (RenameItemProcessCommand).
	- IProcessInfo implements IProcessFactoryProcessInfoBase.
	- IProcessCommands:
		- => IProcessCommand
		- all methods have been re-defined.
- Update resources.

### 3.11-preview

- Add:
	- monitoring: the view model paths are updated when a path on disk is added, renamed or removed.
	- methods to BrowsableObjectInfoViewModel in order to load and sort items.
	- new types.
	- new properties and methods to IO.(I)BrowsableObjectInfo.
	- null-check argument validation to IO.BrowsableObjectInfo.RegisterCallback.
	- BrowsableObjectInfoWindow.GetDefaultDataContext method.
- Re-design:
	- BrowsableObjectInfoCallback
	- BrowsableObjectInfoCallbackQueue
- IO.(I)BrowsableObjectInfo.RegisterCallback(s)(Override) method definitions has been updated.
- (I)BrowsableObjectInfoCollectionViewModel:
	- the interface implements DotNetFix.IDisposable.
	- class:
		- inherits from ViewModelBase
		- BrowsableObjectInfoCollectionViewModel(Collection<IExplorerControlBrowsableObjectInfoViewModel> collection) constructor has been removed.
		- all IExplorerControlBrowsableObjectInfoViewModel items are disposed on remove.
		- new protected methods added.
- BrowsableObjectInfoWindow: the IExplorerControlBrowsableObjectInfoViewModel collection of the BrowsableObjectInfoWindowViewModel view model is cleared on close and all items are disposed before the paths being cleared.
- Bug fixes:
	- (I)ExplorerControlBrowsableObjectInfoViewModel:
		- the interface has new methods.
		- class:
			- the bitmap sources loader was not properly reinitialized between each loop.
			- BrowseToParent command predicate threw an exception if the declaring object was disposed.
			- when the path changed, it was loaded two times because of the history selected path change event handler.
	- BrowsableObjectInfoWindow: an exception was thrown when attempting to get the Shell context menu for the selected item.
	- ShellObjectInfo: when representing a ShellLink, threw an exception when browsability was requested if the path did not exist.

### 3.10-preview

- Update ExplorerControlBrowsableObjectInfoViewModel to load bitmap sources of the current inner path and the new intermediate bitmap sources of the BitmapSourcesLinker class.
- Update FileSystemObjectInfo default bitmap sources.
- Add:
	- intermediate bitmap sources in BitmapSourcesLinker
	- new types.
	- IBitmapSourceProvider.Intermediate property.
	- BitmapSourceProviderAbstract.Intermediate(Override) properties.
	- IBitmapSourcesLinker.LoadSteps
	- protected methods to BitmapSourcesLinker.
	- IBrowsableObjectInfo.IsLocalRoot property.
	- BrowsableObjectInfo.IsLocalRoot(Override) properties.
	- Extensions.GetBrowsableAsValue method.

### 3.9.1-preview

- Add:
	- ContextMenuRequested routed event to ExplorerControlListView.
	- drag and drop in ListView.
- Update BrowsableObjectInfo related types in order to have more centralized plug-in handling.

### 3.8-preview

- Bug fixed:
	- when adding a title bar menu item: the menu item remained enabled even if its IsEnabled property is set to false.
	- on disposing WMIItemInfo objects.
- Update IBrowsableObjectInfo bitmap source properties in order to add a linker to the default and actual bitmap sources so that the actual ones can loaded on background. All bitmap sources implementation has been updated consequently.
- Icons:
	- is in WinCopies.IO.Shell.
	- TryGetFolderBitmapSource method has been replaced with an extension method.
- Add:
	- methods to ExplorerControlBrowsableObjectInfoViewModel.
	- new types.
	- support for drag and drop in ListView.

### 3.7-preview
- Add:
	- TabControl style
	- properties and methods to NavigationButton.
	- ability for Window to handle the mouse XButtons events.
	- BrowseToParent property to (I)ExplorerControl(BrowsableObjectInfoViewModel) classes and interface.
	- button for browsing to the current path's parent to the ExplorerControl.
	- new types.
- The process error panel is displayed when a process does not succeed, even if the error was a global process error.
- Bug fixed:
	- in ShellObjectInfo\<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>.GetParent().
	- in ProcessDelegateTypes\<T, TParam>.ProcessEventDelegates.GetNotContainedActionException method: the argument name that was passed to the exception constructor was those of the actual method and not the value of that argument.
	- in IProcess data template binding.
	- bug fixes in Window

### 3.6-preview

- Add:
	- WinCopies.Console project.
	- Loading process status.
	- new types.
- IProcessFactoryProcessInfo: add GetUserConfirmationText() method.
- IProcessFactory: add Recycling property.
- Re-design process options and error types.
- Update Deletion to add recycling option.
- Process base class:
	- The property event delegate of the Process base class is called when the Status property is updated.
	- The GetEnumerable method is now defined as: System.Collections.Generic.IEnumerable<TItemsOut> GetEnumerable(in TItemsOut path).
	- The Convert method parameter does not have the 'in' modifier anymore.
	- Add protected methods.
	- Dispose method is not virtual anymore. Override DisposeManaged and the new DisposeUnmanaged methods instead.
	- Some dispose operations are performed by the new method DisposeUnmanaged.
	- Add deconstructor, which call Dispose method.
- (Default)DestinationProcess: constructor is now protected.
- ShellObjectInfo:
	- Update processes handling.
	- Bug fixes.
- Update Process factories definitions.
- Add 'Empty Recycle Bin' feature.
- Update Shell ProcessHelper class CanRun method.
- Move part of IO types to the new WinCopies.IO.Shell package and, for some types among them, also to the WinCopies.IO.Shell.* namespace.
- BrowsableObjectInfoFactory: the class and its GetBrowsableObjectInfo(string path) method are is now abstract.
- ExplorerControlBrowsableObjectInfoViewModel:
	- is now abstract.
	- add GetBrowsableObjectInfoViewModelConverter property and add parameter in constructor for the property set-up.
	- OnGoCommandExecuted is not virtual anymore.
	- From static methods has been removed.
- WinCopies.GUI.IO.Process.Process: bug fixed when disposing.
- ExplorerControl: update TextBox.
- Remove ArchiveCompressionProcessParametersControlTemplate resource from WinCopies.GUI.IO

### 3.5-preview

- TextBox.OnUpperCommandCanExecute is now TextBox.OnCaseCommandCanExecute.
- Replace ProcessWindow by ProcessManagerControl and add default view models for this control.
- ShellObjectInfo: some protected subclasses are now in the WinCopies.IO namespace.
- Add recursiveEnumerationOrder parameter to all recursive enumeration methods.
- IRecursiveEnumerable\<T>: GetEnumerator() return type is RecursiveEnumeratorAbstract\<T>.
- Re-design path types.
- Bug fixed when disposing ExplorerControlBrowsableObjectInfoViewModel.
- Add new types.
- Processes:
	- Optimize process collections.
	- Re-design process factories.
	- ProcessInterfaceModelTypes\<TItemsIn, TItemsOut, TError, TAction>.IProcess\<TParam, TProcessEventDelegates>.Actions has the following return type: IReadOnlyDictionary\<string, ICommand\<IProcessErrorItem\<TItemsOut, TError, TAction>>>.
	- WinCopies.IO.Process:
		- AbstractionProcessCollection\<TSource, TDestination> has the new property HasItems.
		- ProcessTypes\<T>.ProcessErrorTypes\<TError, TAction>: TAction generic type parameter removed.
		- ProcessTypes\<TPath, TError, TAction>:
			- TAction generic type parameter removed.
			- Remove protected constructors and add new public constructor.
		- ProcessErrorFactory\<T, TAction> and IProcessErrorItem/Factory:
			- TAction generic type parameter removed.
			- This class is not abstract anymore.
		- IProcessErrorItem: add Item property.
		- ProcessError: add fields.
	- Object model:
		- WinCopies.IO.Process:
			- initial path collection passed to constructor must be writable.
			- add new abstract and virtual methods.
			- LoadPaths and Start are now virtual.
		- Copy process can move file system items to another file system directory.
		- Deletion process added.
		- WinCopies.GUI.IO.Process.Process: add null check in constructor.

### 3.4-preview

- Additions:
	- New commands to TextBox context menu.
	- New DataTemplates.
	- New types.
- NavigationButton uses IHistoryCollection.
- Bug fixed in IconUtil.
- ButtonAlignmentToHorizontalAlignmentConverter inherits from AlwaysConvertibleTwoWayConverter\<HorizontalAlignment, object, System.Windows.HorizontalAlignment>. The value cannot be null.
- Update properties of ProcessControl, ExplorerControl, ExplorerControlListViewItemContent, IExplorerControlBrowsableObjectInfoViewModel, IBrowsableObjectInfoViewModel and WinCopies.GUI.IO.Process.IProcess.
- BrowsableObjectInfoViewModel's and WinCopies.GUI.IO.Process: update properties and add methods.
- Item sorting bug fixed in BrowsableObjectInfoViewModel.
- BrowsableObjectInfo.RegisterAllSelectors is now called RegisterDefaultSelectors.
- Update process linked lists and collections and ProcessStatus.
- Add ActionToDelegateCommand2Converter static resource to WinCopies.GUI.IO merged dictionaries.
- Update DotNetAssemblyInfo, IBrowsableObjectInfo, WinCopies.IO.BrowsableObjectInfo, generic version of FileSystemObjectInfo, RegistryItemInfo and IDotNetAssemblyInfo.
- Type parameters of the generic version of IArchiveItemInfoProvider, IShellObjectInfo and IFileSystemObjectInfo have the out attribute.
- Fixed memory leak in DotNetAttributeInfo, DotNetNamespaceInfo, DotNetParameterInfo, DotNetTypeInfo, DotNetMemberInfo and FileSystemObjectComparer.
- Update Process API.
- BrowsableObjectInfoProperties.BrowsableObjectInfo is now InnerObject.
- Bug fixed in WinCopies.IO.Extensions.GetPath and in Path.RenameDuplicate.

### 3.3.0-preview

- Add new types.
- Add ContentDecoratorStyle dependency property to DialogWindow.
- Update IO API to have better Process integration.
- Remove image source converters. These types have been move to the WinCopies.Util.Desktop package.

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