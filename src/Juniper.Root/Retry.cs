namespace Juniper;

public static class Retry
{
    public static async Task<T> Times<T>(int attempts, Func<int, Task<T>> action, Func<T, bool> validate, Func<T, string> generateErrorMessage) where T : class
    {
        var result = default(T);
        var errors = new List<Exception>();
        for (int i = attempts; i > 0 && (result is null || !validate(result)); --i)
        {
            try
            {
                result = await action(attempts - i);
            }
            catch (Exception exp)
            {
                errors.Add(exp);
            }
        }

        if (result is null || !validate(result))
        {
            if (result != default(T))
            {
                throw new Exception(generateErrorMessage(result));
            }
            else
            {
                throw new AggregateException("Failed to retrieve a valide result", errors);
            }
        }

        return result;
    }

    public static T Times<T>(int attempts, Func<int, T> action, Func<T, bool> validate, Func<T, string> generateErrorMessage) where T : class
    {
        var result = default(T);
        var errors = new List<Exception>();
        for (int i = attempts; i > 0 && (result is null || !validate(result)); --i)
        {
            try
            {
                result = action(attempts - i);
            }
            catch (Exception exp)
            {
                errors.Add(exp);
            }
        }

        if (result is null || !validate(result))
        {
            throw new AggregateException("Failed to retrieve a valide result", errors);
        }

        return result;
    }

    public static Task<T> Times<T>(int attempts, Func<Task<T>> action, Func<T, bool> validate, Func<T, string> generateErrorMessage) where T : class =>
        Times(attempts, (_) => action(), validate, generateErrorMessage);

    public static T Times<T>(int attempts, Func<T> action, Func<T, bool> validate, Func<T, string> generateErrorMessage) where T : class =>
        Times(attempts, (_) => action(), validate, generateErrorMessage);
}
