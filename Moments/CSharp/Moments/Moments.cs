using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Qutes.Moments
{
    /// <summary>
    /// SYMMYS - Last version of this code available at http://symmys.com/node/136
    /// Project summary statistics to arbitrary horizons under i.i.d. assumption
    /// see Meucci, A. (2010) "Annualization and General Projection of Skewness, Kurtosis and All Summary Statistics"
    /// GARP Risk Professional, August, pp. 52-54 
    /// </summary>
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
            if (K > N)
                return 0;
            for (d = 1; d <= K; ++d)
            {
                r *= N--;
                r /= d;
            }
            return r;
        }

        /// <summary>
        /// Transforms central moments into raw moments (first central moment defined as expectation).
        /// </summary>
        /// <param name="mu">The mu.</param>
        /// <returns></returns>
        public static double[] Central2Raw(double[] mu)
        {
            var N = mu.Length;
            var mu_ = mu.Clone() as double[];

            for (int n = 2; n <= N; ++n)
            {
                mu_[n-1] = Math.Pow(-1,n + 1) * Math.Pow(mu[0], n);
                for (int k = 1; k<n; ++k)
                    mu_[n-1] += NChooseK(n, k) * Math.Pow(-1, n - k + 1) * mu_[k-1] * Math.Pow(mu_[0] ,n - k);
            
                mu_[n-1] += mu[n-1];
            }

            return mu_;
        }

        /// <summary>
        /// Replicate Matlab native central moment computation.
        /// See https://fr.mathworks.com/help/stats/moment.html
        /// </summary>
        /// <param name="X">The x.</param>
        /// <param name="k">The k.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Transforms cumulants into raw moments.
        /// </summary>
        /// <param name="ka">The ka.</param>
        /// <returns></returns>
        public static double[] Cumul2Raw(double[] ka)
        {
            var N = ka.Length;
            var mu_ = ka.Clone() as double[];

            for (int n = 1; n <= N; ++n)
            {
                mu_[n-1] = ka[n-1];
                for (int k = 1; k<n; ++k)
                  mu_[n-1] += NChooseK(n - 1, k - 1) * ka[k-1] * mu_[n - k - 1];
            }

            return mu_;
        }




        /// <summary>
        /// Transforms raw moments into central moments(first central moment defined as expectation).
        /// </summary>
        /// <param name="mu_">The mu.</param>
        /// <returns></returns>
        public static double[] Raw2Central(double[] mu_)
        {
            var N = mu_.Length;
            var mu = mu_.Clone() as double[];

            for (int n = 2; n <= N; ++n)
            {
                mu[n-1] = Math.Pow(-mu_[0], n);
                for (int k = 1; k<n; ++k)
                    mu[n-1] += NChooseK(n, k) * Math.Pow(-mu_[0], n - k) * mu_[k-1];
                mu[n-1] += mu_[n-1];
            }

            return mu;
        }

        /// <summary>
        /// Transforms raw moments into cumulants.
        /// </summary>
        /// <param name="mu_">The mu.</param>
        /// <returns></returns>
        public static double[] Raw2Cumul(double[] mu_)
        {
            var N = mu_.Length;
            var ka = mu_.Clone() as double[];

            for (int n = 1; n <= N; ++n)
            {
                ka[n-1] = mu_[n-1];
                for (int k = 1; k<n; ++k)
                    ka[n-1] -= NChooseK(n - 1, k - 1) * ka[k-1] * mu_[n - k - 1];
            }


            return ka;
        }

        public static double[] ComputeStandardizedStatistics(double[] Mu)
        {
            var Ga = Mu.Clone() as double[];
            Ga[1] = Math.Sqrt(Mu[1]);
            for(int n = 3; n<=Mu.Length; ++n)
                Ga[n-1] = Mu[n-1] / Math.Pow(Ga[1],n);

            return Ga;
        }





    }
}
