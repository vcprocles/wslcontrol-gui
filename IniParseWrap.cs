using PeanutButter.INI;
using System;
using System.Threading;

class IniParseWrap
{
    protected PeanutButter.INI.INIFile? parser;
    //public IniParseWrapGlobal()//constructor
    //{
    //    //string homeFolder = Environment.SpecialFolder.UserProfile.ToString();
    //    //string pathToWSLConfig = homeFolder;
    //}

}
class IniParseWrapGlobal : IniParseWrap
{
    public IniParseWrapGlobal()//constructor
    {
        string homeFolder = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        string pathToWSLConfig = homeFolder +"\\.wslconfig";
        parser=new INIFile(pathToWSLConfig);
    }
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
}