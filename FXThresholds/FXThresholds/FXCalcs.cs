using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FXThresholds
{
    public class FXCalcs
    {
        public const int iTREND_UP = 1;
        public const int iTREND_DOWN = 0;

        public const int iTRUE = 1;
        public const int iFALSE = 0;

        public class Trend
        {
            public int newTrend { get; set; }
            public int trendOvershoot {get; set;}
            public int trendReversal { get; set; }
        }

        public class Threshold
        {
            public double newLowThreshold { get; set; }
            public double newHighThreshold {get; set;}
        }

        public class MarketStepReturn
        {
            public double percent { get; set; }
            public int newTrend { get; set; }
            public double newLowThreshold { get; set; }
            public double newHighThreshold { get; set; }
            public int trendOvershoot { get; set; }
            public int trendReversal { get; set; }
            public string eventText { get; set; }
        }

        public class MarketStepInput
        {
            public double mid { get; set; }
            public double percent { get; set; }
            public int curTrend { get; set; }
            public double curLowThreshold { get; set; }
            public double curHighThreshold { get; set; }
        }

        public static MarketStepReturn MarketStep(MarketStepInput input)
        {
            Trend calcTrend = new Trend();
            calcTrend = TrendCalc(input.mid, input.curTrend, input.curLowThreshold, input.curHighThreshold);

            Threshold calcThreshold = new Threshold();
            calcThreshold = ThresholdCalc(calcTrend, input.mid, input.percent, input.curLowThreshold, input.curHighThreshold);

            string calcEventText = EventText(calcTrend, calcThreshold);

            MarketStepReturn outValue = new MarketStepReturn() 
            {
                percent = input.percent,
                newTrend = calcTrend.newTrend,
                newLowThreshold = calcThreshold.newLowThreshold,
                newHighThreshold = calcThreshold.newHighThreshold,
                trendOvershoot = calcTrend.trendOvershoot,
                trendReversal = calcTrend.trendReversal,
                eventText = calcEventText
            };
            return outValue;
        }

        public static Trend TrendCalc(double mid, int curTrend, double lowThres, double highThres)
        {
            Trend retTrend = new Trend
            {
                trendOvershoot = iFALSE,
                trendReversal = iFALSE
            };
            if( (mid > highThres) && (curTrend == iTREND_UP) ) 
            {
                retTrend.trendOvershoot = iTRUE;
                retTrend.newTrend = curTrend;
            }
            else if ((mid > highThres) && (curTrend == iTREND_DOWN))
            {
                retTrend.trendReversal = iTRUE;
                retTrend.newTrend = iTREND_UP;
            }
            else if ((mid < lowThres) && (curTrend == iTREND_DOWN))
            {
                retTrend.trendOvershoot = iTRUE;
                retTrend.newTrend = curTrend;
            }
            else if ((mid < lowThres) && (curTrend == iTREND_UP))
            {
                retTrend.trendReversal = iTRUE;
                retTrend.newTrend = iTREND_DOWN;
            }
            else
            {
                retTrend.newTrend = curTrend;
            }
            return retTrend;
        }


        public static Threshold ThresholdCalc(Trend uNewTrend, double curMid, double curPercent, double curLowThreshold, double curHighThreshold)
        {
            Threshold retThreshold = new Threshold();

            if (NoTrendEvent(uNewTrend))
            {
                if (uNewTrend.newTrend == iTREND_UP)
                {
                    retThreshold.newHighThreshold = curHighThreshold;
                    retThreshold.newLowThreshold = Math.Max(curLowThreshold, curMid * (1 - curPercent));
                }
                else  // this is newtrend == trend_down
                {
                    retThreshold.newLowThreshold = curLowThreshold;
                    retThreshold.newHighThreshold = Math.Min(curHighThreshold, curMid * (1 + curPercent));
                }
            }
            else
            {
                retThreshold.newLowThreshold = curMid * (1 - curPercent);
                retThreshold.newHighThreshold = curMid * (1 + curPercent);
            }
            return retThreshold;
        }

        public static bool NoTrendEvent(Trend inTrend)
        {
            return (inTrend.trendOvershoot == iFALSE && inTrend.trendReversal == iFALSE);
        }

        public static string EventText(Trend uTrend, Threshold uThresholds)
        {
            string retString = "";

            if (NoTrendEvent(uTrend))
            {
                retString = "NO EVENT";
            }
            else if (uTrend.trendOvershoot == iTRUE)
            {
                if (uTrend.newTrend == iTREND_UP)
                {
                    retString = "TREND: UP OVERSHOOT NLOW: " + uThresholds.newLowThreshold + " NHIGH: " + uThresholds.newHighThreshold;
                }
                else
                {
                    retString = "TREND: DOWN OVERSHOOT NLOW: " + uThresholds.newLowThreshold + " NHIGH: " + uThresholds.newHighThreshold;
                }

            }
            else if (uTrend.trendReversal == iTRUE)
            {
                if (uTrend.newTrend == iTREND_UP)
                {
                    retString = "REVERSAL NEW TREND: UP OVERSHOOT NLOW: " + uThresholds.newLowThreshold + " NHIGH: " + uThresholds.newHighThreshold;
                }
                else
                {
                    retString = "REVERSAL NEW TREND: DOWN OVERSHOOT NLOW: " + uThresholds.newLowThreshold + " NHIGH: " + uThresholds.newHighThreshold;
                }
            }
            else
            {
                retString = "ERROR INVALID STATE";
            }
            return retString;
        }

    }
}
