namespace Juniper
{
    public static class Retry
    {
        public static async Task<T> Times<T>(int attempts, Func<int, Task<T>> action, Func<T, bool> validate)
        {
            var result = default(T);
            var errors = new List<Exception>();
            for (int i = attempts; i > 0 && !validate(result); --i)
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

            if (!validate(result))
            {
                throw new AggregateException("Failed to retrieve a valide result", errors);
            }

            return result;
        }

        public static T Times<T>(int attempts, Func<int, T> action, Func<T, bool> validate)
        {
            var result = default(T);
            var errors = new List<Exception>();
            for (int i = attempts; i > 0 && !validate(result); --i)
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

            if (!validate(result))
            {
                throw new AggregateException("Failed to retrieve a valide result", errors);
            }

            return result;
        }

        public static Task<T> Times<T>(int attempts, Func<Task<T>> action, Func<T, bool> validate) =>
            Times(attempts, (_) => action(), validate);

        public static T Times<T>(int attempts, Func<T> action, Func<T, bool> validate) =>
            Times(attempts, (_) => action(), validate);
    }
}
