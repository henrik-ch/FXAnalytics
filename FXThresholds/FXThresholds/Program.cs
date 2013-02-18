using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace FXThresholds
{
    class Program
    {
        static void Main(string[] args)
        {

            var fxUpdates = new List<FXUpdate>();
            readfxCsv(fxUpdates);


            var percent = 0.0025;
            var curTrend = FXCalcs.iTREND_UP;
            var curLowThres = 1.23;
            var curHighThres = 1.25;
            var curMid = fxUpdates[0].ratemid;

            var fxResults = new List<FXCalcs.MarketStepReturn>();
            var myInput = new FXCalcs.MarketStepInput()
            {
                mid = curMid,
                percent = percent,
                curTrend = curTrend,
                curLowThreshold = curLowThres,
                curHighThreshold = curHighThres,
            };

            var myOutput = FXCalcs.MarketStep(myInput);
            fxResults.Add(myOutput);

            //from second list item to the end.
            for (int updateCounter = 1; updateCounter < fxUpdates.Count; updateCounter += 1)
            {
                myInput = new FXCalcs.MarketStepInput()
                {
                    mid = fxUpdates[updateCounter].ratemid,
                    percent = fxResults[updateCounter -1].percent,
                    curTrend = fxResults[updateCounter -1].newTrend,
                    curLowThreshold = fxResults[updateCounter -1].newLowThreshold,
                    curHighThreshold = fxResults[updateCounter - 1].newHighThreshold
                };
                myOutput = FXCalcs.MarketStep(myInput);
                fxResults.Add(myOutput);
            }


            var json = JsonConvert.SerializeObject(fxResults, Formatting.Indented);
            File.WriteAllText("fxresults.json", json);

            System.Console.WriteLine("Stopping here...");

        }

        static void readfxCsv(List<FXUpdate> inList)
        {
            char[] delimiters = new char[] { ',' };
            using (StreamReader reader = new StreamReader("C:\\Users\\henrik\\Dropbox\\R\\FXThresholds\\FXThresholds\\FXThresholds\\FXVals.csv"))
            {
                string header = reader.ReadLine();
                string[] headerParts = header.Split(delimiters);
                Console.WriteLine(string.Join(" ", headerParts));
                while (true)
                {
                    string line = reader.ReadLine();

                    if (line == null)
                    {
                        break;
                    }
                    string[] parts = line.Split(delimiters);
                    //Console.WriteLine(string.Join(" ", parts));
                    var updateToAdd = new FXUpdate();
                    updateToAdd.timestamp = double.Parse(parts[0]);
                    updateToAdd.ratemid = double.Parse(parts[1]);
                    inList.Add(updateToAdd);
                }
            }
        }

    }
}
