using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace wslcontrol_gui
{
    public class ConsoleInterface
    {
        protected virtual string RunCommand(string command, string parameters)
        {
            ProcessStartInfo processInfo;
            Process process;
            processInfo = new ProcessStartInfo(command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = parameters,
                RedirectStandardOutput = true,
                StandardOutputEncoding = System.Text.Encoding.Unicode//important
            };
            process = Process.Start(processInfo)!;
            if (process == null) return "0";
            process.WaitForExit();
            string outputString = process.StandardOutput.ReadToEnd();
            return outputString;
        }
        protected static void RunCommandInWindow(string command, string parameters)
        {
            ProcessStartInfo processInfo;
            processInfo = new ProcessStartInfo(command)
            {
                CreateNoWindow = false,
                UseShellExecute = true,
                WorkingDirectory = "C:\\",
                Arguments = parameters
            };
            Process.Start(processInfo);
        }
        protected static void RunCommandInWindowAndWait(string command, string parameters)
        {
            ProcessStartInfo processInfo;
            Process process;
            processInfo = new ProcessStartInfo(command)
            {
                CreateNoWindow = false,
                UseShellExecute = true,
                Arguments = parameters,
            };
            process = Process.Start(processInfo)!;
            if (process == null) throw new ArgumentNullException();
            process.WaitForExit();
        }
    }
    public class WSLInterface : ConsoleInterface
    {
        protected override string RunCommand(string command, string parameters)
        {
            return base.RunCommand("C:\\Windows\\System32\\" + command, parameters);
        }
        private string PassToWSL(string parameters)
        {
            return RunCommand("wsl.exe", parameters);
        }
        private static void RunWSLInWindow(string parameters) => RunCommandInWindow("C:\\Windows\\System32\\wsl.exe", parameters);
        private static void RunWSLInWindowAndWait(string parameters) => RunCommandInWindowAndWait("C:\\Windows\\System32\\wsl.exe", parameters);

        public int GetCurrentDefaultWSLVersion()
        {
            char ver = RespondParser.GetLastLineSymbol(PassToWSL("--status"), 1);
            int iver = int.Parse(ver.ToString());
            return iver;
        }
        public void ShutdownWSL()
        {
            PassToWSL("--shutdown");
        }
        public void TerminateDistro(string distro)
        {
            PassToWSL("--terminate " + distro);
        }
        public void SetDefaultVersion(int ver)
        {
            PassToWSL("--set-default-version " + ver);
        }
        public List<Distro> GetDistros()
        {
            return RespondParser.ParseDistroList(PassToWSL("-l -v"));
        }
        public List<OnlineDistro> GetOnlineDistros()
        {
            return RespondParser.ParseOnlineDistroList(PassToWSL("-l -o"));
        }
        public void UnregisterDistro(string distro)
        {
            PassToWSL("--unregister " + distro);
        }
        public static void RunCustomCommand(string distro, string command)
        {
            RunWSLInWindow("-d " + distro + " -- bash -l -c \"" + command + "\" && bash");
        }
        public static void PassCommand(string distro, string command)
        {
            RunWSLInWindowAndWait("-d " + distro + " -- " + command);
        }
        public static void OpenDistro(string distro)
        {
            RunWSLInWindow("-d " + distro);
        }
        public void SetDefaultDistro(string distro)
        {
            PassToWSL("-s " + distro);
        }
        public void ImportDistro(string distroName, string importFilePath, string distroInstallPath, DistType type, int WSLVersion)
        {
            if (type == DistType.vhdx)
            {
                PassToWSL("--import " + distroName + " " + distroInstallPath + " " + importFilePath + " --version " + WSLVersion.ToString() + " " + "--vhd");
            }
            else if (type == DistType.tar)
            {
                PassToWSL("--import " + distroName + " " + distroInstallPath + " " + importFilePath + " --version " + WSLVersion.ToString());
            }
        }
        public void ExportDistro(string distroName, string exportPath, DistType type)
        {
            if (type == DistType.vhdx)
            {
                PassToWSL("--export " + distroName + " " + exportPath + " --vhd");
            }
            else if (type == DistType.tar)
            {
                PassToWSL("--export " + distroName + " " + exportPath);
            }
        }
        public void InstallOnlineDistro(string distroName)
        {
            PassToWSL("--install " + distroName);
        }
        public void InitializeWSLFirstStart()
        {
            PassToWSL("--install");
        }
    }
    public enum DistType
    {
        vhdx,
        tar
    }

    public class Distro
    {
        public Distro()
        {
            this.Number = 0;
            this.Name = "noname";
            this.State = "nostate";
            this.Version = 0;
            this.Default = false;
        }
        public int Number { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public int Version { get; set; }
        public bool Default { get; set; }
        public override string ToString()//override text output
        {
            if (this.Default) return this.Number.ToString() + ". " + this.Name + ", WSL" + this.Version + ", " + this.State + ", Set as default";
            return this.Number.ToString() + ". " + this.Name + ", WSL" + this.Version + ", " + this.State;
        }
    }
    public class OnlineDistro : Distro
    {
        public OnlineDistro()
        {
            this.Number = 0;
            this.Name = "noname";
            this.State = "unneeded";
            this.Version = 0;
            this.Default = false;
            this.FriendlyName = "No Name";
        }
        public string FriendlyName { get; set; }
        public override string ToString() //override text output
        {
            return this.Number.ToString() + ". " + this.FriendlyName + " (" + this.Name + ")";
        }
    }
}