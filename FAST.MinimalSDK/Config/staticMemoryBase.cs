using FAST.Core;

namespace FAST.Config
{
    public abstract class staticMemoryBase 
    {
        protected static variablesContainer _variables = null;
        protected static variablesContainer variables
        {
            get
            {
                if (_variables == null)_variables = new variablesContainer();
                return _variables;
            }
            set
            {
                if (_variables == null)_variables = new variablesContainer();
                _variables = value;
            }
        }
    }



}
