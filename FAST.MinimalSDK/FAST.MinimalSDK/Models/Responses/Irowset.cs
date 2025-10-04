namespace FAST.Core.Models
{
    public interface Irowset<T> : IcommonInRowData
    {
        List<T> data { get; set; }
    }

   
}
