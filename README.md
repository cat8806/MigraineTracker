# MigraineTracker

A lightweight .NET MAUI app for logging migraine episodes and daily factors.
<img width="473" alt="image" src="https://github.com/user-attachments/assets/d7bbb38a-bb07-44a6-ae8f-8dc2e68dbc96" />


## Features
- Add migraine episodes (start/end, severity, triggers, notes)
- Log meals, water, supplements, sleep
- View and delete entries in the **Timeline** page
- Quick trend reports (more coming)
- Manual export/import backups from the **Settings** tab (Android export lets you choose the destination folder)

## Project Structure

- **Models/** – Data models such as `MigraineEntry`, `MealEntry`, `SleepEntry`, and more  
- **Data/** – `MigraineTrackerDbContext` sets up the SQLite database  
- **ViewModels/** – MVVM logic for pages (e.g., `MainPageViewModel`, `AddSupplementViewModel`)  
- **Views/** – XAML pages that make up the UI (Add pages, Timeline, Reports, etc.)  
- **Resources/** – App icons, images, and fonts used throughout the UI  


## Getting Started
1. Install **Visual Studio 2022** with the **.NET MAUI** workload.
2. Clone this repository and open `MigraineTracker.sln` in Visual Studio.
3. Select an Android or iOS emulator/device from the toolbar.
4. Use Visual Studio's **Run** or **Debug** command to build and launch the app.

## Compatibility

The app has been tested on a **3120 × 1440** display (e.g., *Galaxy S25 Ultra*).  
If your device’s screen size differs, simply create an Android emulator with the same resolution in **Visual Studio 2022 → Android Device Manager** and give it a quick spin.

## Permissions

Android builds require the `WRITE_EXTERNAL_STORAGE` and `READ_EXTERNAL_STORAGE` permissions for the backup import/export feature.
During export you will be prompted with Android's folder picker to choose where the backup file is saved.

## Release Builds

Android release builds enable IL trimming which can remove Entity Framework Core metadata and break the backup import. The `MigraineTracker.csproj` file disables trimming for Android release builds so that database operations work correctly.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
