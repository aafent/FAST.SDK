namespace FAST.Core
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class toDoAttribute : Attribute
    {
        public string note { get; set; }

        public toDoAttribute()
        {
        }
        public toDoAttribute(string note):this()
        {
            this.note = note;
        }
    }
}