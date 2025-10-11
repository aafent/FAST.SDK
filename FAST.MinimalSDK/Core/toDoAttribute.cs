namespace FAST.Core
{

    /// <summary>
    /// Attribute to mark code that needs to be done
    /// </summary>
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class toDoAttribute : Attribute
    {
        /// <summary>
        /// Note about what needs to be done
        /// </summary>
        public string note { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public toDoAttribute()
        {
        }
        /// <summary>
        /// Constructor with note
        /// </summary>
        /// <param name="note"></param>
        public toDoAttribute(string note):this()
        {
            this.note = note;
        }
    }
}