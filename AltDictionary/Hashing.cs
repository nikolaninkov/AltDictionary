namespace Alt
{
    public static class Hashing
    {
        public static int Hash<T>(T key, int bucketCount)
        {
            if (key != null && bucketCount > 0)
            {
                try
                {
                    var hashCode = key.GetHashCode() % bucketCount;
                    return hashCode >= 0 ? hashCode : hashCode + bucketCount;
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
            if (number < 2)
            {
                return 3;
            }
            // Bertrand's postulate: n > 3 => there exists a prime number p, such that n < p < 2*n - 2
            // => if number * 2 > 3 => there exists a prime number p', such that number * 2 < p' < number * 4 - 2
            // ofc, number * 4 - 2 < int.MaxValue
            if (number < int.MaxValue / 4 + 0.5)
            {
                // the postulate guarantees that the loop will stop
                for (int i = number * 2 + 1; ; i++)
                {
                    if (IsPrime(i))
                    {
                        return i;
                    }
                }
            }
            return number;
        }

        public static bool IsPrime(int number)
        {
            if (number == 2 || number == 3)
            {
                return true;
            }
            if (number < 2 || number % 2 == 0 || number % 3 == 0)
            {
                return false;
            }
            int s = (int)Math.Floor(Math.Sqrt(number));
            // if p is prime => p = 6*n + 1 || p = 6*n - 1
            for (int i = 5; i <= s; i += 6)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            for (int i = 7; i <= s; i += 6)
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
