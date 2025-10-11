namespace FAST.Core
{
    [toDo("Remote from future releases")]
    [Obsolete("Do not used it. Implement code to do the same")]
    public interface ImergeFrom<T>
    {
        void mergeFrom(T dest);
    }
}