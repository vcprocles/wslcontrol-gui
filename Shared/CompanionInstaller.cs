using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace wslcontrol_gui.Shared
{
    partial class CompanionInstaller
    {
        public static bool CheckCompanionInstallation(string distro)
        {
            //here i can run perl oneliner to check file existance
            if (WSLInterface.RunCustomCommandNoWindowNoUnicode(distro, "test -f ~/.companion.pl && echo \"true\"")=="") return false; //hacky but should work
            else
                return true;
        }
        public static int CheckCompanionVersion(string distro)
        {
            var firstLine = WSLInterface.RunCustomCommandNoWindowNoUnicode(distro, "perl ~/.companion.pl -v").Split("\n")[0];
            if (firstLine == "read mode")
                return 0;
            else
                return int.Parse(firstLine);
        }
        public static async void InstallCompanion(string distro)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string script;
            const string resourceName = "wslcontrol_gui.linux_companion.companion.pl";
            using (Stream stream =assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                script = reader.ReadToEnd();
            }
            string initial = "\\\\wsl.localhost\\";
            WSLInterface wsli = new();
            string username = wsli.GetDefaultUser(distro);
            username = username.Replace("\n", "");
            string fullPath = initial + distro + "\\home\\" + username + "\\.companion.pl";
            await File.WriteAllTextAsync(fullPath, script);
            //here i copy the script from the resource to the WSL's home folder
        }
    }
}
