using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using System.Threading;

namespace wslcontrol_gui
{
    public class ConsoleInterface
    {
        protected virtual string RunCommand(string command, string parameters)
        {
            ProcessStartInfo processInfo;
            Process process;
            command=command.Replace("System32", "Sysnative");//dirty hack to run shit from 32-bit environment
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
        protected virtual string RunCommandNoUnicode(string command, string parameters)
        {
            ProcessStartInfo processInfo;
            Process process;
            command = command.Replace("System32", "Sysnative");//dirty hack to run shit from 32-bit environment
            processInfo = new ProcessStartInfo(command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = parameters,
                RedirectStandardOutput = true,
                //StandardOutputEncoding = System.Text.Encoding.Unicode//not important here, apparently
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
            command = command.Replace("System32", "Sysnative");//dirty hack to run shit from 32-bit environment
            processInfo = new ProcessStartInfo(command)
            {
                CreateNoWindow = false,
                UseShellExecute = true,
                WorkingDirectory = "C:\\",
                Arguments = parameters
            };
            Process.Start(processInfo);
        }
        protected static void RunCommandNoWindow(string command, string parameters)
        {
            ProcessStartInfo processInfo;
            command = command.Replace("System32", "Sysnative");//dirty hack to run shit from 32-bit environment
            processInfo = new ProcessStartInfo(command)
            {
                CreateNoWindow = true,
                UseShellExecute = true,
                //WorkingDirectory = "C:\\",
                Arguments = parameters
            };
            Process.Start(processInfo);
        }
    }
    public class WSLInterface : ConsoleInterface
    {
        private string PassToWSL(string parameters)
        {
            return RunCommand("C:\\Windows\\System32\\wsl.exe", parameters);
        }
        private string PassToWSLNoUnicode(string parameters)
        {
            return RunCommandNoUnicode("C:\\Windows\\System32\\wsl.exe", parameters);
        }
        public int GetCurrentDefaultWSLVersion()
        {
            char ver = RespondParser.GetLastLineSymbol(PassToWSL("--status"), 1);
            int iver = int.Parse(ver.ToString());
            return iver;
        }
        public void ShutdownWSL()//Shuts down the subsystem
        {
            PassToWSL("--shutdown");
        }
        public void TerminateDistro(string distro)//Stops the specific distro
        {
            PassToWSL("--terminate " + distro);
        }
        public void SetDefaultVersion(int ver)//Sets the default WSL version
        {
            PassToWSL("--set-default-version " + ver);
        }
        public List<Distro> GetDistros()//parses distribution list for main menu
        {
            return RespondParser.ParseDistroList(PassToWSL("-l -v"));
        }
        public List<OnlineDistro> GetOnlineDistros()//parses available distribution list for online install window
        {
            return RespondParser.ParseOnlineDistroList(PassToWSL("-l -o"));
        }
        public void UnregisterDistro(string distro)//remove distro
        {
            PassToWSL("--unregister " + distro);
        }
        public static void RunCustomCommand(string distro, string command)//run custom command in a separate window
        {
            RunCommandInWindow("C:\\Windows\\System32\\wsl.exe", "-d " + distro + " -- bash -l -c \"" + command + "\" && bash");
        }
        public static void RunCustomCommandNoWindow(string distro, string command)//run custom command without a separate window. Used internally.
        {
            RunCommandNoWindow("C:\\Windows\\System32\\wsl.exe", "-d " + distro + " -- bash -l -c \"" + command + "\"");
        }
        public static void OpenDistro(string distro)//Opens the selected distro
        {
            RunCommandInWindow("C:\\Windows\\System32\\wsl.exe", "-d " + distro);
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
            PassToWSL("--install -d " + distroName);
        }
        public void InitializeWSLFirstStart()//TODO: use it for smth
        {
            PassToWSL("--install");
        }
        public string GetDefaultUser(string distroName)
        {
            return PassToWSLNoUnicode("-d " + distroName + " -- whoami");
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