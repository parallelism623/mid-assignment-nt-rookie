namespace MIDASM.Domain.Abstract;

public interface IEntity<T>
{
    public T Id { get; set; }
}
