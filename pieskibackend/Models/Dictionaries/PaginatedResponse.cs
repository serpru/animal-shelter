namespace pieskibackend.Models.Dictionaries;

public class PaginatedResponse<T>
{
    public int Page { get; set; }
    public int Size { get; set; }
    public int TotalElements { get; set; }
    public T Data { get; set; }
}
