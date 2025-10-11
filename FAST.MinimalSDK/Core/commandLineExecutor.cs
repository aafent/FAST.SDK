using System.Diagnostics;

namespace FAST.Core
{
    /// <summary>
    /// Executes command line programs and batch files
    /// </summary>
    public class commandLineExecutor
    {
        /// <summary>
        /// The shell program to use, e.g. cmd.exe or powershell.exe
        /// </summary>
        public string shellProgramName = "cmd.exe";

        /// <summary>
        /// The arguments to pass to the shell program, {command} and {arguments} will be replaced
        /// </summary>
        public string shellProgramArguments = "/c {command} {arguments}";

        /// <summary>
        /// The program to execute, e.g. cmd.exe or powershell.exe
        /// </summary>
        public string argumentsToExecute = "/c {command}";

        /// <summary>
        /// The command to execute, e.g. a batch file name
        /// </summary>
        public string commandToExecute = string.Empty;

        /// <summary>
        /// The arguments to pass to the command
        /// </summary>
        public StreamReader stdOUT = null;

        /// <summary>
        /// The standard error stream of the executed command
        /// </summary>
        public StreamReader stdERR = null;

        /// <summary>
        /// The standard input stream of the executed command
        /// </summary>
        public StreamWriter stdIN = null;

        /// <summary>
        /// The exit code of the executed command
        /// </summary>
        public int exitCode;

        ProcessStartInfo processInfo;
        Process process;

        /// <summary>
        /// Executes a batch file and waits for it to finish
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandArguments"></param>
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

        /// <summary>
        /// Reads the standard error stream to the end and returns it as a string
        /// </summary>
        /// <returns></returns>
        public string stdERRToString()
        {
            if (stdERR == null) return string.Empty;
            return stdERR.ReadToEnd();
        }

        /// <summary>
        /// Reads the standard output stream to the end and returns it as a string
        /// </summary>
        /// <returns></returns>
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
