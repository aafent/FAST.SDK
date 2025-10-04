namespace FAST.Core.Models
{
    public abstract class errorItem : IErrorItem
    {
        private errorTypes _type = errorTypes.information;
        private string _description = "";

        public errorTypes type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
    }
}
