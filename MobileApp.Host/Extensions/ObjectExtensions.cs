namespace MobileApp.Host.Extensions;

public static class ObjectExtensions
{
    public static T Set<T>(this T obj, Action<T> action)
    {
        action(obj);
        return obj;
    }

    public static bool IsOfType<T>(this object value)
    {
        if (value == null)
        {
            return false;
        }

        return value.GetType() == typeof(T);
    }
}