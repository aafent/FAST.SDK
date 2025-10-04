namespace FAST.Data
{
    /// <summary>
    /// Interface to implement the Producer Consumer Hub
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IproducerConsumerHub<T>
    {
        Task<T> consumeAsync();
        Task produceAsync(T data);
        string tag { get; set; }
    }

    //
    // based on the follwing idea  
    // more:    https://www.c-sharpcorner.com/article/producer-consumer-pattern-in-c-sharp/#:~:text=The%20Producer%20Consumer%20pattern%20is,this%20is%20a%20disconnected%20pattern.
    //          https://stackoverflow.com/questions/23854102/using-async-await-with-datareader-without-middle-buffers
    //          https://dotnetfiddle.net/8gQVt2
    //

    /// <summary>
    /// Producer & Consumer hub an implementation of Producer Consumer Pattern 
    /// The Producer Consumer pattern is where a producer generates some messages or data as we may call it and 
    /// various consumers can read that data and work on it. The main advantage of this pattern is that the 
    /// producer and consumer are not causally linked in any way. Hence, we can say this is a disconnected pattern.
    /// 
    /// Acts as the intermediate hub between the producer of an asynchronous data provider and the consumer
    /// </summary>
    /// <typeparam name="TData">Type of data the producer sending to the consumer</typeparam>
    public class producerConsumerHub<TData> : IproducerConsumerHub<TData>
    {
        TaskCompletionSource<Empty> _consumer = new TaskCompletionSource<Empty>();
        TaskCompletionSource<TData> _producer = new TaskCompletionSource<TData>();

        public string tag { get; set; } = string.Empty;

        // TODO: make thread-safe
        public async Task produceAsync(TData data)
        {
            _producer.SetResult(data);
            await _consumer.Task;
            _consumer = new TaskCompletionSource<Empty>();
        }

        public async Task<TData> consumeAsync()
        {
            var data = await _producer.Task;
            _producer = new TaskCompletionSource<TData>();
            _consumer.SetResult(Empty.Value);
            return data;
        }

        struct Empty { public static readonly Empty Value = default(Empty); }
    }

}
