namespace Juniper.Google.Maps
{
    public struct PhoneNumber
    {
        public static explicit operator string(PhoneNumber value)
        {
            return value.ToString();
        }

        public static explicit operator PhoneNumber(string number)
        {
            return new PhoneNumber(number);
        }

        private readonly string number;

        public PhoneNumber(string number)
        {
            this.number = number;
        }

        public override string ToString()
        {
            return number;
        }

        public override int GetHashCode()
        {
            return number.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is PhoneNumber p && p.number == number;
        }

        public static bool operator ==(PhoneNumber left, PhoneNumber right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PhoneNumber left, PhoneNumber right)
        {
            return !(left == right);
        }
    }
}