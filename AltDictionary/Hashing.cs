namespace Alt
{
    public static class Hashing
    {
        public static int Hash(object? key, int bucketCount)
        {
            if (key != null && bucketCount > 0)
            {
                try
                {
                    var hashCode = key.GetHashCode();
                    return hashCode % bucketCount;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public static int GetBucketCount(int number)
        {
            if (number < 0)
            {
                return number;
            }
            if (number == 0)
            {
                number++;
            }
            if (number < int.MaxValue / 2)
            {
                number *= 2;
            }
            for (int i = number + 1; i <= int.MaxValue; i++)
            {
                if (IsPrime(i))
                {
                    return i;
                }
            }
            return number;
        }

        public static bool IsPrime(int number)
        {
            if (number < 2)
            {
                return false;
            }
            double s = Math.Sqrt(number);
            for (int i = 2; i <= s; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
