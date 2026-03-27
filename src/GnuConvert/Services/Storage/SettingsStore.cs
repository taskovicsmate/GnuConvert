using GnuConvert.ExceptionHandling;
using GnuConvert.Services.Settings;
using System.IO;

namespace GnuConvert.Services.Storage
{
    public sealed class SettingsStore
    {
        public AppSettings LoadOrCreateDefault()
        {

            try
            {
                Directory.CreateDirectory(AppPaths.Root);
            }
            catch (IOException ex)
            {
                throw new PersistenceException(
                    "SETTINGS_DIR_CREATE_FAILED",
                    $"Sikerertelen a beállítások könyvtárának a létrehozása: {AppPaths.Root}",
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PersistenceException(
                    "SETTINGS_DIR_ACCESS_DENIED",
                    $"Hozzáférés megtagadva a beállításokhoz: {AppPaths.Root}",
                    ex);
            }

            if (File.Exists(AppPaths.SettingsFile))
            {
                return JsonFileStore.Load<AppSettings>(AppPaths.SettingsFile);

            }
            else { 
            
                var defaults = AppSettings.CreateDefault();
                Save(defaults);
                return defaults;
            }

        }

        public void Save(AppSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "A beállítások nem lehetnek null értékűek.");
            }
            JsonFileStore.SaveAtomic(AppPaths.SettingsFile, settings);
        }
    }
  

}
