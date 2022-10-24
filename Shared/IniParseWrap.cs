using PeanutButter.INI;
using System;

namespace wslcontrol_gui
{
    class IniParseWrap
    {
        protected PeanutButter.INI.INIFile parser = new("");
        protected string path = "";
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
        public void SetParameter(string key, bool value)
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
    class IniParseWrapSpecific : IniParseWrap
    {
        OsInfo os = new();
        private string distroName;
        public IniParseWrapSpecific(Distro distro)
        {
            distroName = distro.Name;
            string initial;
            if (os.build < 22000)
            {
                initial = "\\\\wsl$\\";
                //manuallyStartWSL=true;
            }
            else initial = "\\\\wsl.localhost\\";
            string fullPath = initial + distroName + "\\etc\\wsl.conf";
            //parser = new INIFile(fullPath);
            parser = new INIFile("a.conf");
        }
        public void SetParameterMountOptions(string key, string value)
        {
            const string section = "automount";
            value = "\"" + value + "\"";
            SetParameter(section, key, value);
        }
        public void GetConfig() => WSLInterface.PassCommand(distroName, "-- perl .companion.pl");
        public void SetConfig()
        {
            WSLInterface.PassCommand(distroName, "-- sudo perl .companion.pl -i");
        }
    }
}