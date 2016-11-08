using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Qutes.Moments;

namespace Test
{
    class Program
    {
        struct Moments
        {
            public double StdDev;
            public double Skewness;
            public double Kurtosis;
        }

        static void Main(string[] args)
        {
            double[] spotPrices = null;
            using (var textReader= File.OpenText("SPX.csv"))
            {
                var csv = new CsvReader(textReader);
                spotPrices = csv.GetRecords<double>().ToArray();
            }

            var dailyReturns = new double[spotPrices.Length - 1];
            var monthlyReturns = new double[spotPrices.Length - 21];
            var annualReturns = new double[spotPrices.Length - 252];

            for (int i = 1; i < spotPrices.Length; ++i)
                dailyReturns[i - 1] = Math.Log(spotPrices[i]/spotPrices[i - 1]);

            for (int i = 21; i < spotPrices.Length; ++i)
                monthlyReturns[i - 21] = Math.Log(spotPrices[i] / spotPrices[i - 21]);

            for (int i = 252; i < spotPrices.Length; ++i)
                annualReturns[i - 252] = Math.Log(spotPrices[i] / spotPrices[i - 252]);

            var nMoments = 4;
            //Projection horizon
            var kDailyAnnual = 252;
            var kDailyMonthly = 21;
            var kMonthlyAnnual = 12;

            //Compute Moments
            var dailyMoms = qMoments.SummStats(dailyReturns, nMoments);
            var monthlyMoms = qMoments.SummStats(monthlyReturns, nMoments);
            var annualMoms = qMoments.SummStats(annualReturns, nMoments);


            //compute single - period non - central moments
            var mu_daily = qMoments.Central2Raw(dailyMoms.Item1);
            var mu_monthly = qMoments.Central2Raw(monthlyMoms.Item1);

            //compute single - period cumulants
            var ka_daily = qMoments.Raw2Cumul(mu_daily);
            var ka_monthly = qMoments.Raw2Cumul(mu_monthly);

            //compute multi - period cumulants
            var Ka_dailyMonthly = ka_daily.Select(x=>x*kDailyMonthly).ToArray();
            var Ka_dailyAnnual = ka_daily.Select(x=>x* kDailyAnnual).ToArray();
            var Ka_monthlyAnnual = ka_monthly.Select(x=> x*kMonthlyAnnual).ToArray();

            //compute multi - period non - central moments
            var Mu_DailyMonthly = qMoments.Cumul2Raw(Ka_dailyMonthly);
            var Mu_DailyAnnual = qMoments.Cumul2Raw(Ka_dailyAnnual);
            var Mu_MonthlyAnnual = qMoments.Cumul2Raw(Ka_monthlyAnnual);

            //compute multi - period central moments
            var MuDailyMonthly = qMoments.Raw2Central(Mu_DailyMonthly);
            var MuDailyAnnual = qMoments.Raw2Central(Mu_DailyAnnual);
            var MuMonthlyAnnual = qMoments.Raw2Central(Mu_MonthlyAnnual);

            var standardisedDailyMonthly = qMoments.ComputeStandardizedStatistics(MuDailyMonthly);
            var standardisedDailyAnnual = qMoments.ComputeStandardizedStatistics(MuDailyAnnual);
            var standardisedMonthlyAnnual = qMoments.ComputeStandardizedStatistics(MuMonthlyAnnual);


        }
    }
}
