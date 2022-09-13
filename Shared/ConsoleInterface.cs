using System.Collections.Generic;
using System.Diagnostics;
namespace wslcontrol_gui
{
    class RespondParser
    {
        public static char GetLastLineSymbol(string text, int lineNumber)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            try
            { return lines[lineNumber][^1]; }
            catch
            { return '0'; }
        }
        public static List<Distro> ParseDistroList(string text)
        {
            List<Distro> dl = new();
            string[] lines = text.Replace("\r", "").Split('\n');
            int lineNumber = 0;
            foreach (string line in lines)
            {
                int PNumber = lineNumber++;
                bool PDefault = false;
                if (line.Length == 0) continue;
                if (line.Equals(lines[0])) continue;
                if (line.StartsWith('*'))
                {
                    PDefault = true;
                }
                string[] fields = line.Split(' ');
                string[] fields2 = new string[4];
                int i = 0;
                foreach (string field in fields)
                {
                    if (field == "*") continue;
                    if (field.Length == 0) continue;
                    fields2[i++] = field;
                }
                string PName = fields2[0];
                string PState = fields2[1];
                int PVersion = int.Parse(fields2[2]);
                dl.Add(new Distro() { Number = PNumber, Name = PName, State = PState, Version = PVersion, Default = PDefault });
            }
            return dl;
        }

    }
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
        public void UnregisterDistro(string distro)
        {
            PassToWSL("--unregister " + distro);
        }
        public static void RunCustomCommand(string distro, string command)
        {
            RunWSLInWindow("-d " + distro + " -- bash -l -c \"" + command + "\" && bash");
        }
        public static void OpenDistro(string distro)
        {
            RunWSLInWindow("-d " + distro);
        }
        public void SetDefaultDistro(string distro)
        {
            PassToWSL("-s " + distro);
        }
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
}