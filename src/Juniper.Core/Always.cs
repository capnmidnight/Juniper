namespace Juniper
{
    public static class Always
    {
        public static bool True()
        {
            return true;
        }

        public static bool False()
        {
            return false;
        }

        public static bool Not(bool value)
        {
            return !value;
        }

        public static T Identity<T>(T value)
        {
            return value;
        }
    }
}
