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
            // Bertrand's postulate: n > 3 => there exists a prime number p, such that n < p < 2*n - 2
            // => if number * 2 > 3 => there exists a prime number p', such that number * 2 < p' < number * 4 - 2
            // ofc, number * 4 - 2 < int.MaxValue
            if (number < int.MaxValue / 4 + 0.5)
            {
                number *= 2;
                // first, check the computed primes
                for (int i = 0; i < primes.Length; i++)
                {
                    if (primes[i] > number)
                    {
                        return primes[i];
                    }
                }
                // if that doesn't work, calculate
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

        public static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};
    }
}
