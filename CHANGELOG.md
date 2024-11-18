# Changelog

All notable changes to this project will be documented in this file.

## [24.11.00] - 2024-11-18

### Bug Fixes

- Ensure Chart and stats window update when a new portfolio is loaded
- Move UiStyles into ui project to reduce viewModel dependency on windows.
- Remove dependency on System.Windows.Data for datagrid views.
- Remove view model dependency on System.Windows.Controls
- Update FinancialStructures for tweaks to statistic calculations.
- Add stats output to body of email
- Remove windows dependencies from FPD.Logic completely
- Ensure can send email with no attachements
- Use Microsoft logger where possible, and remove creation of new lists
- Make MailSender an injectable service.
- Update submodule
- Correct wrong scrolling behaviour in stats creator window
- Update for new yahoo cookie

### Features

- Expose calculating currency statistics.
- Add aspect logging into console commands.

### Testing

- Fix broken unit tests

## [24.10.00-20241002T1424] - 2024-10-02

### Bug Fixes

- Move code into src and test folders.

### Refactor

- Update to use collection lists.

## [24.09.00-20240911T0003] - 2024-09-11

### Bug Fixes

- Remove obsolete consoleInstance class from console commands
- Update FinancialStructures for downloading improvements
- Improve downloading and date formatting.

## [24.09.00-20240905T0010] - 2024-09-05

### Bug Fixes

- Default to not calculate MDD and Drawdown for stats view.
- Update submodules to latest develop versions.
- Fixes to downloading to ensure it works for yahoo.
- Update downloaders for non uk instruments.

### Features

- Add new statistics into Bank Account output options.
- Add scroll bars into stats creator page.
- Provide options in Charts window for duration and frequency of valuations.
- Improve Stats generation apis.

### Testing

- Fix broken config tests.

## [24.06.00] - 2024-06-10

### Bug Fixes

- Move exception handling subscription to code behind.
- Move IsLightTheme checking to ViewModel.
- Move UiStyles creation outside of MainWindowViewModel.
- Migrate Reporter instantiation into DI container.
- Store config location in IConfiguration to avoid repeated use of filepath.
- Tidy code in MainWindowViewModel.
- Miscellaneous code tidies.
- Split app class into constituent parts.
- Only update UI trades and values when difference occurs.
- Alter stats window update restricting.
- Restrict update event firing when timelist data is unchanged.
- Add delays to stats window tests to ensure they pass.
- Address code style in stats window tests.
- Ensure StatsUserWindow does not react when not visible.
- Ensure start display converts values into correct currency.
- Update common library to ensure downloading prices works.

### Features

- Use IConfiguration for command line arguments.
- Place window startup in code behind.
- Add basic hosting for the main window.
- Add MainWindowViewModel to DI container and resolve using service collection.
- Move styles and ViewModelFactory creation oustide of MainViewModel.
- Move Configuration loading out of MainWindowViewModel.
- Improve performance of stats window.

### Miscellaneous Tasks

- Update build process repo to fix single file and version issues.

## [24.03.00] - 2024-03-17

### Bug Fixes

- Update build runners to include all test projects.
- Update Publish properties to fix font missing errors.
- Update Style bridge name to ensure style is inherited correctly.
- Ensure gridsplitter can alter location.

## [24.02.00] - 2024-02-23

### Bug Fixes

- Tweaks to chart viewing to reduce regeneration of stats.
- Update financial structures to remove dodgy download tests.
- Add logging to mail sender class.
- Trimming options
- Prepend author name to namespace.
- Update common location and console namespace.
- Code style in importCommand.
- Update common for missing using statements.
- Improve default stats filepath in download command.
- Updates to common and Financial structures for reporting and stats defaults.
- Ensure that TradeCosts can be edited.
- Ensure that IRR returns correct value when stock no longer has any shares.
- Ensure console logging using friendly string.

### Build Changes

- Enable parsing of version tag in build process.
- Enable reporting of test results.
- Remove default package creation.
- Update build to ensure publish to version folder.
- Add ability to record code coverage in test projects.
- Change checks reporting.
- Ensure FPDConsole builds with partial trimming.

### Features

- Add ability to email from console once stats update performed.
- Enable loading and saving of binary portfolio files.

### Miscellaneous Tasks

- Make common test packages part of directory.build.props file.
- Tidy target framework properties.
- Misc fixes for build process.
- Update common location.
- Update Common project namespaces.
- Rearrange solution layout.
- Move FinancialStructures into src folder.
- Fix broken tests from namespace changes.
- Tidy csproj files.
- Update naming of dependent dlls to include author.
- Update FPD.Logic.Tests namespaces.
- Update FinancialStructures namespace to append Author.

### Refactor

- Update FPD.UI and FPD.Logic namespaces to append author.
- Address code style comments in FPD.UI.

### Testing

- Add unit tests for console command validation.
- Update config tests for namespace changes.

## [23.12.01] - 2023-12-27

### Build Changes

- Add changelog gen to actions.
- Update build process to properly deal with version and release notes updates.

### Documentation

- Add default git-cliff notes config.
- Add custom commit start messages for release notes.

<!-- generated by git-cliff -->
