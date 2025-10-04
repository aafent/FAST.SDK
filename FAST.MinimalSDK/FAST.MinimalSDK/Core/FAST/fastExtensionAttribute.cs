namespace FAST.Core
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class fastExtensionAttribute : Attribute
    {
        public string toDo { get; set; }
        public string status { get; set; }
        public string note { get; set; }
        public bool movedToFAST { get; set; } = false;

        public fastExtensionAttribute()
        {
        }
        public fastExtensionAttribute(string note):this()
        {
            this.note = note;
        }
        public fastExtensionAttribute(string toDo, string note) : this(note)
        {
            this.toDo = toDo;
        }

    }
}