using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GnuConvert.Services.Settings;

namespace GnuConvert.Services.Storage
{
    public sealed class SettingsStore
    {
        public AppSettings LoadOrCreateDefault()
        {
            Directory.CreateDirectory(AppPaths.Root);

            if (File.Exists(AppPaths.SettingsFile))
            {
                try
                {
                    return JsonFileStore.Load<AppSettings>(AppPaths.SettingsFile);
                }
                catch
                {
                    // Ha hibás a fájl, újra létrehozzuk
                }
            }

            var defaults = AppSettings.CreateDefault();
            Save(defaults);
            return defaults;
        }

        public void Save(AppSettings settings)
        {
            JsonFileStore.SaveAtomic(AppPaths.SettingsFile, settings);
        }
    }
  

}
