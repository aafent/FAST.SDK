namespace FAST.Core.Models
{
    public interface IRow<T> : IcommonInRowData
    {
        T data { get; set; }
    }

}
