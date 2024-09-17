using System.ComponentModel.DataAnnotations;


namespace Common.Validators
{
    public class NonZeroAttribute<T> : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is null || value is T intValue && EqualityComparer<T>.Default.Equals(intValue, default))
            {
                return false;
            }
            return true;
        }
    }
}
