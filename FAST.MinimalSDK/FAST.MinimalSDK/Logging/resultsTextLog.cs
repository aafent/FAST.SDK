namespace FAST.Logging
{
    public class resultsTextLog
    {
        public class entry
        {
            public levels level   { get; set; }
            public string message { get; set; }

            public entry()
            {
            }
            public entry(levels level, string message):this()
            {
                this.level = level;
                this.message = message;
            }
            public entry(string message):this()
            {
                this.level = levels.info;
                this.message = message;
            }
        }

        public List<entry> messages { get; private set; }

        public bool hasMessages
        {
            get
            {
                return messages.Count > 0;
            }
        }

        public resultsTextLog()
        {
            messages = new List<entry>();
        }

        public void add(string message)
        {
            messages.Add( new entry(message) );
        }
        public void add(levels level, string message)
        {
            messages.Add( new entry(level, message) );
        }

        public void clear()
        {
            messages.Clear();
        }

        public delegate void whatToTry();
        public void tryTo(whatToTry codeToTry, string messageOnSuccess, string messageOnError )
        {
            try
            {
                codeToTry();
                if (!string.IsNullOrEmpty(messageOnError)) this.add(levels.info, messageOnSuccess);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(messageOnError)) messageOnError = "{0}";
                messageOnError = messageOnError.Replace("{exception}", "{0}");
                this.add(levels.error, string.Format(messageOnError, ex));
            }
        }



        public static resultsTextLog empty()
        {
            resultsTextLog empty = new resultsTextLog();
            return empty;
        }
    }
}
