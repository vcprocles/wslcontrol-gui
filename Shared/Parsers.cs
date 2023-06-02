using System.Collections.Generic;

namespace wslcontrol_gui
{
    class RespondParser
    {
        public static char GetLastLineSymbol(string text, int lineNumber)
        //splits text into lines, cleaning up newlines and carriage returns
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            try
            { return lines[lineNumber][^1]; }
            catch
            { return '0'; }
        }
        public static List<Distro> ParseDistroList(string text)
        //WSL distro list parser
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
        public static List<OnlineDistro> ParseOnlineDistroList(string text)
        //availible distributions for initial setup list parser
        {
            List<OnlineDistro> dl = new();
            string[] lines = text.Replace("\r", "").Split('\n');
            int lineNumber = 1;
            bool skipForNow = true;
            foreach (string line in lines)
            {
                string[] fields = line.Split(' ');
                if (skipForNow)
                {
                    //does this work across locales?
                    if (fields[0]=="NAME")
                        skipForNow = false;
                    continue;
                }
                bool PDefault = false;
                int PNumber = lineNumber++;
                if (line.Length == 0) continue;
                string PName = "";
                if (fields[0] == "*") //clean up default star
                {
                    PDefault = true;
                    fields[0] = "";
                    PName= fields[1];//internal name
                    fields[1] = "";
                }
                else
                {
                    PName= fields[2];//2?
                    fields[2] = "";
                }
                string PFriendlyName = "";
                foreach (string field in fields)
                {
                    if ((field.Length == 0)) continue;
                    PFriendlyName += field+" ";
                }
                dl.Add(new OnlineDistro() { Name = PName, FriendlyName = PFriendlyName, Number = PNumber, Default = PDefault });
            }
            return dl;
        }
        public static List<OnlineDistro> ParseOnlineDistroList2(string text)
        //availible distributions for online install list parser
        {
            List<OnlineDistro> dl = new();
            string[] lines = text.Replace("\r", "").Split('\n');
            int lineNumber = 1;
            bool skipForNow = true;
            foreach (string line in lines)
            {
                string[] fields = line.Split(' ');
                if (skipForNow)
                {
                    //does this work across locales?
                    if (fields[0] == "NAME")
                        skipForNow = false;
                    continue;
                }
                int PNumber = lineNumber++;
                if (line.Length == 0) continue;
                string PName = fields[0];
                fields[0] = "";
                string PFriendlyName = "";
                foreach (string field in fields)
                {
                    if ((field.Length == 0)) continue;
                    PFriendlyName += field + " ";
                }
                dl.Add(new OnlineDistro() { Name = PName, FriendlyName = PFriendlyName, Number = PNumber});
            }
            return dl;
        }
    }
}