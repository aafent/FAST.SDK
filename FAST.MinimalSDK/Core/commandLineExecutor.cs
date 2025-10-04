using System.Diagnostics;

namespace FAST.Core
{
    public class commandLineExecutor
    {
        public string shellProgramName = "cmd.exe";
        public string shellProgramArguments = "/c {command} {arguments}";

        public string argumentsToExecute = "/c {command}";
        public string commandToExecute = string.Empty;

        public StreamReader stdOUT = null;
        public StreamReader stdERR = null;
        public StreamWriter stdIN = null; 

        public int exitCode;

        ProcessStartInfo processInfo;
        Process process;


        public void executeBatchfileWithWait(string command, string commandArguments)
        {
            string argumentsForTheShell = shellProgramArguments.Replace("{command}", "{0}").Replace("{arguments}", "{1}");
            processInfo = new ProcessStartInfo(shellProgramName, string.Format(argumentsForTheShell, command, commandArguments));
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;

            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            stdOUT = process.StandardOutput;
            stdERR = process.StandardError;

            exitCode = process.ExitCode;

            process.Close();
        }


        public string stdERRToString()
        {
            if (stdERR == null) return string.Empty;
            return stdERR.ReadToEnd();
        }
        public string stdOUTToString()
        {
            if (stdOUT == null) return string.Empty;
            return stdOUT.ReadToEnd();
        }


        //private void executeCommandWithWait(string command)
        //{
        //    string arguments = argumentsToExecute.Replace("{command}", "{0}");
        //    processInfo = new ProcessStartInfo(programToExecute, string.Format(arguments,command) );
        //    processInfo.CreateNoWindow = true;
        //    processInfo.UseShellExecute = false;

        //    // *** Redirect the output ***
        //    processInfo.RedirectStandardError = true;
        //    processInfo.RedirectStandardOutput = true;

        //    process = Process.Start(processInfo);
        //    process.WaitForExit();

        //    // *** Read the streams ***
        //    // Warning: This approach can lead to deadlocks, see Edit #2
        //    string output = process.StandardOutput.ReadToEnd();
        //    string error = process.StandardError.ReadToEnd();

        //    exitCode = process.ExitCode;

        //    Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
        //    Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
        //    Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
        //    process.Close();
        //}
    }
}
