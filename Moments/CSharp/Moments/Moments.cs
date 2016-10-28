using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Qutes.
    Moments
{
    public static class qMoments
    {
        public static long NChooseK(long N, long K)
        {
            // This function gets the total number of unique combinations based upon N and K.
            // N is the total number of items.
            // K is the size of the group.
            // Total number of unique combinations = N! / ( K! (N - K)! ).
            // This function is less efficient, but is more likely to not overflow when N and K are large.
            // Taken from:  http://blog.plover.com/math/choose.html
            //
            long r = 1;
            long d;
            if (K > N) return 0;
            for (d = 1; d <= K; d++)
            {
                r *= N--;
                r /= d;
            }
            return r;
        }

        public static double[] Central2Raw(double[] mu)
        {
            var N = mu.Length;
            var mu_ = mu.Clone() as double[];

            for (int n = 2; n <= N; ++n)
            {
                mu_[n-1] = Math.Pow(-1,n + 1) * Math.Pow(mu[0], n);
                for (int k = 1; k<n; ++k)
                    mu_[n-1] += NChooseK(n, k) * Math.Pow(-1, n - k + 1) * mu_[k] * Math.Pow(mu_[0] ,n - k);
            
                mu_[n-1] += mu[n];
            }

            return mu_;
        }

        public static double Moment(double[] X, int k)
        {
            var avg = X.Average();
            var size = X.Length;
            var ms = from x in X
                select Math.Pow(x - avg, k)/size;

            var mk = ms.Sum();

            return mk;

        }

        public static Tuple<double[], double[]> SummStats(double[] X, int N)
        {
            //compute central moments
            var mu = Enumerable.Repeat(0.0, N).ToArray();
            mu[0] = X.Average();


            for (int n = 2; n <= N; ++n)
                mu[n-1] = Moment(X, n);


            //compute standardized statistics
            var ga = mu.Clone() as double[];
            ga[1] = Math.Sqrt(mu[1]);
            for (int n = 3; n <= N; ++n)
                ga[n-1] = mu[n-1] / Math.Pow(ga[1],n);

            return new Tuple<double[], double[]>(mu, ga);
        }

        public static 




    }
}
