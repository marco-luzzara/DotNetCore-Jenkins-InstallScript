using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace InstallScript
{
    public class CommandExecutor
    {
        public CommandExecutor()
        {
        }

        virtual internal string GetDefaultShell()
        {
            string shell = "";
            if (Program.currentOS == OSPlatform.Windows)
                shell = "cmd.exe";
            else if (Program.currentOS == OSPlatform.OSX || Program.currentOS == OSPlatform.Linux)
                shell = "/bin/bash";

            return shell;
        }

        virtual internal string GetParameterToRunningCommand()
        {
            string parameters = "";
            if (Program.currentOS == OSPlatform.Windows)
                parameters = "/c";
            else if (Program.currentOS == OSPlatform.OSX || Program.currentOS == OSPlatform.Linux)
                parameters = "-c";

            return parameters;
        }

        virtual public void ExecuteCommandOnDefaultShell(string command, bool logging = true)
        {
            if (logging)
                Console.WriteLine($"Start -- Executing command {command}");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = this.GetDefaultShell();
            startInfo.Arguments = $"\"{this.GetParameterToRunningCommand()} {command}\"";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;

            using (var process = Process.Start(startInfo))
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }

                process.WaitForExit();
            }

            if (logging)
                Console.WriteLine($"End -- Executing command {command}");
        }
    }
}
