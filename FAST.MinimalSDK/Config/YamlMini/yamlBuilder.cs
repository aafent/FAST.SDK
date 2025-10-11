using System.Text;

namespace FAST.Config
{
    public class yamlBuilder 
    {
        public class header
        {
            public string title {  get; set; } = "";
            public string library { get; set; } = "";
            public string name {  get; set; } ="";
        }

        public static yamlBuilder operator ++(yamlBuilder a)
        {
            a.nestingLevel++;
            return a;
        }   


        public StringBuilder builder=null;
        
        public string spacing { get; protected set;} =string.Empty;
        private int _nestingLevel = 0;
        public int nestingLevel
        {
            get
            {
                return _nestingLevel;
            }
            set
            {
                if ( value < 0 ) value=0;
                _nestingLevel = value;
                int chars = _nestingLevel * 3;
                if ( chars ==0 ) spacing=string.Empty;
                spacing = new string(' ', chars);
            }
        } 

        public yamlBuilder(header head)
        {
            if (head==null) head=new();
            builder = new StringBuilder();
            builder.AppendLine($"#");
            if ( !string.IsNullOrEmpty(head.title))     builder.AppendLine($"# {head.title}");
            if (!string.IsNullOrEmpty(head.library))    builder.AppendLine($"# Library: {head.library}");
            if (!string.IsNullOrEmpty(head.name))       builder.AppendLine($"# Name: {head.name}");
            builder.AppendLine($"# Last saved: {DateTime.Now}");
            builder.AppendLine($"#");
        }

        // (v) wrapper 
        public void comment(string text=null)
        {
            if ( string.IsNullOrEmpty(text) )
            {
                builder.AppendLine("#");
            }
            else
            {
                builder.AppendLine($"# {text}");
            }
        }

        public void AppendLine(string text) // direct wrapper 
        {
            builder.AppendLine(text);
        }

        public void writeLine(string text)
        {
            if (nestingLevel > 0) builder.Append(spacing);
            builder.AppendLine(text);
        }

        public void entry(string name, object value)
        {
            this.writeLine($"{name}: {value}");
        }

        public void collectionNextItem()
        {
            if (nestingLevel > 0) 
            {
                if (nestingLevel > 0) builder.Append(spacing.Substring(0,(spacing.Length-1)) );
                builder.AppendLine("-");
            } 
            else
            {
                builder.AppendLine("-");
                nestingLevel++;
            }
        }


        public int section(string name)
        {
            this.comment();
            this.writeLine($"{name}:");
            return this.nestingLevel++;
        }
        public int section(int newNestingLevel, string name)
        {
            this.nestingLevel=newNestingLevel;
            return section(name);
        }

        public void resetNesting()
        {
            this.nestingLevel = 0;
        }

        public string toYaml()
        {
            builder.AppendLine($"#");
            builder.AppendLine($"# End of yaml file");
            builder.AppendLine($"#");
            return builder.ToString();
        }

        public override string ToString()
        {
            return toYaml();
        }

    }
}
