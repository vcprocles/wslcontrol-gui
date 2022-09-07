using PeanutButter.INI;
using System;
using System.Threading;
using System.Windows.Documents;

class IniParseWrap
{
    protected PeanutButter.INI.INIFile? parser;
    protected string path;
    //public IniParseWrapGlobal()//constructor
    //{
    //    //string homeFolder = Environment.SpecialFolder.UserProfile.ToString();
    //    //string pathToWSLConfig = homeFolder;
    //}
    protected void SetParameter(string section, string key, string value)
    {
        parser.SetValue(section, key, value);
    }
    private void RemovePrivateUnusedParameters()//TODO figure out how to clean out the unused keys
                                                //not critical,though
    {
        foreach (var a in parser["wsl2"].Keys)
        {
            if (a is "") parser["wsl2"].Remove(a);
        }
    }
    protected string ReadParameter(string section, string key)
    {
        if (parser.HasSection(section) && parser.HasSetting(section, key))
        {
            return parser.GetValue(section, key);
        }
        else return null;
    }
    public void WriteOut()
    {
        RemovePrivateUnusedParameters();
        parser.Persist(path);
    }
}
class IniParseWrapGlobal : IniParseWrap
{
    const string section = "wsl2";
    public IniParseWrapGlobal()//constructor
    {
        string homeFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        string pathToWSLConfig = homeFolder +"\\.wslconfig";
        path = pathToWSLConfig;
        parser=new INIFile(pathToWSLConfig);
    }
    
    public void SetParameter(string key, string value)
    {
        SetParameter(section, key, value);
    }
    public void SetParameter (string key, bool value)
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
    
    public string ReadParameter(string key) => ReadParameter(section, key);

}
class IniParseWrapSpecific : IniParseWrap //might need to move this to the different executable, because elevation is needed
{
    OsVer os = new();
    private bool manuallyStartWSL = false;
    public IniParseWrapSpecific(Distro distro)
    {
        string distroName = distro.Name;
        string initial;
        if (os.build < 22000)
        {
            initial = "\\\\wsl$\\";
            manuallyStartWSL=true;
        }
        else initial = "\\\\wsl.localhost\\";
        string fullPath=initial+distroName+"\\";
    }
    public void SetParameterMountOptions(string key, string value)
    {
        const string section = "automount";
        value = "\"" + value + "\"";
        SetParameter(section, key, value);
    }
}