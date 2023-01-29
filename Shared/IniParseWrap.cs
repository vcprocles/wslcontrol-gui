using PeanutButter.INI;
using System;
using System.Threading;

namespace wslcontrol_gui
{
    class IniParseWrap
    {
        protected PeanutButter.INI.INIFile parser = new("");
        public string path = "";
        public void SetParameter(string section, string key, string value)
        {
            parser.SetValue(section, key, value);
        }
        private string? ReadParameter(string section, string key)
        {
            if (parser.HasSection(section) && parser.HasSetting(section, key))
            {
                return parser.GetValue(section, key);
            }
            else return null;
        }
        public void WriteOut()
        {
            if (path.Length != 0) parser.Persist(path);
        }
        public string ReadParameterString(string section, string key, out bool err)
        {
            err = false;
            string? a = ReadParameter(section, key);
            if (a != null) return a;
            err = true;
            return "Something has gone wrong";
        }

        public bool ReadParameterBoolean(string section, string key, out bool err)
        {
            if (bool.TryParse(ReadParameterString(section, key, out _), out bool a))//in case of an error in ReadParameterString it's going to output "Something has gone wrong", which can't be parsed
            {
                err = false;
                return a;
            }
            else
            {
                err = true;
                return false;
            }
        }
        public void SetParameter(string section,string key, bool value)
        {
            if (value)
            {
                SetParameter(section, key, "true");
            }
            else
            {
                SetParameter(section, key, "false");
            }
        }
    }
    class IniParseWrapGlobal : IniParseWrap
    {
        const string section = "wsl2";
        public IniParseWrapGlobal()
        {
            string homeFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            string pathToWSLConfig = homeFolder + "\\.wslconfig";
            path = pathToWSLConfig;
            parser = new INIFile(pathToWSLConfig);
        }

        public void SetParameter(string key, string value)
        {
            SetParameter(section, key, value);
        }
        public void SetParameter(string key, bool value) => base.SetParameter(section, key, value);
    }
    class IniParseWrapSpecific : IniParseWrap
    {
        private string distroName;
        WSLInterface wsli = new();
        string username = "root";
        public IniParseWrapSpecific(Distro distro)
        {
            //GetConfig();
            distroName = distro.Name;
            username=wsli.GetDefaultUser(distroName);
            username = username.Replace("\n", "");
            string initial;
            initial = "\\\\wsl.localhost\\";
            string fullPath = initial + distroName + "\\home\\"+username+"\\wsl.conf";
            path=fullPath;
            parser.WrapValueInQuotes = false;
            parser = new INIFile(fullPath);
        }
        public void SetParameterMountOptions(string key, string value)
        {
            const string section = "automount";
            value = "\"" + value + "\"";
            SetParameter(section, key, value);
        }
        public static void GetConfig(string dNameStatic)
        {
            WSLInterface.RunCustomCommandNoWindow(dNameStatic, "cd ~;perl ~/.companion.pl");
        }
        public void SetConfig()
        {
            WSLInterface.RunCustomCommand(distroName, "cd ~;sudo perl ~/.companion.pl -i;exit");
        }
        public void ResetConfig()
        {
            WSLInterface.RunCustomCommand(distroName, "cd ~;sudo perl ~/.companion.pl -r;exit");
        }
    }
}