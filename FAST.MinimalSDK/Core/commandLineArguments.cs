namespace FAST.Core
{
    public class commandLineArguments
    {
        private bool bypassArg0 = true;
        private List<string> args;
        public List<string>  commandLineArgumentsList
        {
            get
            {
                return args;
            }
        }
        public variablesContainer values = new();

        public commandLineArguments(string[] args, bool bypassArgument0=true )
        {
            this.args = args.ToList();
            this.bypassArg0 = bypassArgument0;
            values.clearVariables();
            parse();
        }

        private void expandArguments()
        {
            List<string> files = new List<string>();
            foreach (var arg in commandLineArgumentsList)
            {
                if (arg.StartsWith("@"))
                {
                    string fileName = arg.Substring(1); // after the @ character. Eg. @c:\temp\args @.\arg2.param 
                    if (File.Exists(fileName))
                    {
                        files.Add(fileName);
                    }
                }
            }

            foreach (var file in files)
            {
                string key = "@" + file;
                commandLineArgumentsList.Remove(key);
            }
        }

        private void parse()
        {
            int count = -1; // start from 0
            foreach (var arg in commandLineArgumentsList)
            {
                count++;
                if (bypassArg0 && count == 0) continue;

                string key = "";
                string value = "";

                var p1=arg.IndexOf('=');
                if (p1 < 0) p1 = arg.IndexOf(':');
                if (p1 >= 0)
                {
                    key = arg.Substring(0, p1);
                    value = arg.Substring(p1 + 1);
                }
                else
                {
                    key = arg;
                    value = "";
                }

                if (string.IsNullOrEmpty(key)) continue;



                if (!string.IsNullOrEmpty(value)) 
                {
                    values.set(key, value);
                } else
                {
                    values.set(key, "");
                }
            }
        }



    }
}
