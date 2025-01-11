namespace del.shared.DTOs;
public class MessageWrapper<T>
{
    public int Id { get; set; }
    public int RetryCount { get; set; }
    public T Message { get; set; }
}