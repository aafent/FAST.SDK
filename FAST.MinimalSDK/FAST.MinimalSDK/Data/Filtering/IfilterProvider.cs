namespace FAST.Data
{
    public interface IfilterProvider<T> where T: IfilterSyntax
    {
        T syntax{get; set;}
    }
}
