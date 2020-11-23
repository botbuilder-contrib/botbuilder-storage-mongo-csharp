
namespace StateManagementBot
{
    public class Marklar<T> 
    {
        public Marklar(T value )
        {
            Value = value;
        }
        public T Value { get; set; }
    }


}
