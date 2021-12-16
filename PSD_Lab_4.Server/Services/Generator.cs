using System;
using System.Collections.Generic;
using System.Numerics;

namespace PSD_Lab_4.Server.Services
{
    public static class Generator
    {
        public static int GenerateRandomQ(int lowerBound, int higherBound)
        {
            var random = new Random();

            int Q = random.Next(lowerBound, higherBound);

            while (!IsPrime(Q))
            {
                Q = random.Next(lowerBound, higherBound);
            }

            return Q;
        }

        public static int GenerateA(BigInteger Q)
        {
            BigInteger F = Q - 1;

            List<BigInteger> factors = GetPrimeFactors(F);

            factors.Sort();

            for (BigInteger i = 2; i < Q; i++)
            {
                bool isRoot = true;

                foreach (var factor in factors)
                {
                    if (BigInteger.ModPow(i, factor, Q) == 1)
                    {
                        isRoot = false;
                        break;
                    }
                }
                
                if (isRoot)
                {
                    return (int)i;
                }
            }

            return 0;
        }

        public static bool IsPrime(int n)
        {
            var isPrime = true;
            var sqrt = Math.Sqrt(n);

            for (var i = 2; i <= sqrt; i++)
            {
                if ((n % i) == 0)
                {
                    isPrime = false;
                }
            }

            return isPrime;
        }

        public static List<BigInteger> GetPrimeFactors(BigInteger F)
        {
            var factors = new List<BigInteger>();

            for (BigInteger i = 2; F > 1; i++)
            {
                if (F % i != 0)
                {
                    continue;
                }

                factors.Add(F / i);

                while (F % i == 0)
                {
                    F /= i;
                }
            }

            return factors;
        }
    }
}
