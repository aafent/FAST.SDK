namespace FAST.Core.Models
{
    public class errorsContainer<T> where T : IErrorItem 
    {
        public T this[int index]    // Indexer declaration  
        {
            get
            {
                return items[index];
            }

            set
            {
                items[index] = value;
            }
        }  

        public List<T> items = new List<T>();

        public void add(T item)
        {
            items.Add(item);
        }
        public void add(errorTypes type, string description)
        {
            T item = default(T);
            item.type = type;
            item.description = description;
            items.Add( item );
        }

        public void clear()
        {
            items.Clear();
        }

        public virtual void copyItemTo(T source, T destination)
        {
            destination.description = source.description;
            destination.type = source.type;
        }

        public errorsContainer<T> copyTo(errorsContainer<T> destination)
        {
            foreach (var item in items)
            {
                T destinationItem = default(T);
                this.copyItemTo(item, destinationItem);
                destination.add(destinationItem);
            }

            return this;
        }

        public bool hasError
        {
            get
            {
                return items.Any(i => i.type == errorTypes.error);
            }
        }
        public bool hasWarning
        {
            get
            {
                return items.Any(i => i.type == errorTypes.warning);
            }
        }
        public bool hasInformation
        {
            get
            {
                return items.Any(i => i.type == errorTypes.information);
            }
        }
        public bool success
        {
            get
            {
                return !hasError;
            }
        }
        public bool sucessWithoutWarnings
        {
            get
            {
                return !(hasError || hasWarning);
            }
        }
        public bool fail
        {
            get
            {
                return !success;
            }
        }
        public bool failOrWarnings
        {
            get
            {
                return fail || hasWarning;
            }
        }
    }


    public class errorsContainer : errorsContainer<errorItem>
    {
    }


}
