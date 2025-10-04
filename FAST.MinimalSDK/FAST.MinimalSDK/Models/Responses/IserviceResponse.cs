namespace FAST.Core.Models
{
    public interface IserviceResponse : IerrorCarrier, IdataFoundQuestion
    {
        void copyFrom(IserviceResponse source);
        void copyFrom<T>(errorsContainer<T> source) where T : IErrorItem;
    }

}
