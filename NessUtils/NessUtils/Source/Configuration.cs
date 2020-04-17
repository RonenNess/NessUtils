
namespace Ness.Utils
{
    /// <summary>
    /// Wrap app config.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Get a config value from AppConfig.
        /// </summary>
        public static string GetConfig(string key, string defaultVal = null, bool mandatory = true)
        {
            var ret = System.Configuration.ConfigurationManager.AppSettings[key] ?? defaultVal;
            if (mandatory && string.IsNullOrEmpty(ret))
            {
                throw new System.Configuration.ConfigurationErrorsException("Missing mandatory configuration key: '" + key + "'");
            }
            return ret;
        }

        /// <summary>
        /// Get a list of values.
        /// </summary>
        public static string[] GetList(string key, string defaultVal = null, bool mandatory = true, char delimiter = ',')
        {
            var ret = GetConfig(key, defaultVal, mandatory);
            var values = ret.Split(delimiter);
            for (int i = 0; i < values.Length; ++i)
            {
                values[i] = values[i].Trim();
            }
            return values;
        }

        /// <summary>
        /// Get a folder config value from AppConfig (this will validate folder exists).
        /// </summary>
        public static string GetFolder(string key, string defaultVal = null, bool mandatory = true, bool folderMustExist = true)
        {
            // get folder name
            var ret = GetConfig(key, defaultVal, mandatory);

            // check if exist (note: this also validate path is valid, that's why we do it anyway)
            bool exist = false;
            try
            {
                exist = System.IO.Directory.Exists(ret);
            }
            catch
            {
                throw new System.Configuration.ConfigurationErrorsException("Invalid folder path: '" + ret + "'");
            }

            // check if exists
            if (folderMustExist && !exist)
            {
                throw new System.Configuration.ConfigurationErrorsException("Missing mandatory folder (read from config): '" + ret + "'");
            }

            // return folder
            return ret;
        }
    }
}
