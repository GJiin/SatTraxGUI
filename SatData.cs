using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SatTraxGUI
{
    public class SatData
    {
        // Satellite RF data
        // Typically there is no publicly available data for LEO/MEO.
        // Two parts...
        //  1- try to get public .. for Stationary.
        //  2 - allow user to enter data (for all types)

        // and persist. as .\DATADIRECTORY\SatData.txt

        public List<SatDataFields> SatDataList;

        public static void LoadSatData(SatelliteCatalog satcat)
        {
            Directory.CreateDirectory(Form1.DATA_DIRECTORY); // create if not already exists

            const string path = Form1.DATA_DIRECTORY + "SatelliteData.csv";
            var lines = new List<string>();
            if (File.Exists(path))
            {
                lines = File.ReadAllLines(path).ToList();
            }

            foreach (var line in lines)
            {
                if (line.StartsWith("#")) continue;

                var items = line.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                // look in the main sat cat. if exists, amend it with this data else add a new entry
                var satId = int.Parse(items[0]);
                var satName = items[1];

                if (!satcat.SatCatDict.ContainsKey(satId)) // norad number
                {
                    var satInfo = new SatInfo
                    {
                        NoradNumber = satId,
                        SatelliteName = satName
                    };

                    // add
                    satcat.SatCatDict.Add(satId, satInfo);
                }

                satcat.SatCatDict[satId].sdf.SatId = satId;
                satcat.SatCatDict[satId].sdf.SatName = items[1];
                satcat.SatCatDict[satId].sdf.StartTimeAzEl = items[2];
                satcat.SatCatDict[satId].sdf.EndTimeAzEl = items[3];
                satcat.SatCatDict[satId].sdf.TrackType = items[4];
                satcat.SatCatDict[satId].sdf.TrackingFrequency = items[5];
                satcat.SatCatDict[satId].sdf.Polarity = items[6];
                satcat.SatCatDict[satId].sdf.ProgramAutoTrack = items[7];
                satcat.SatCatDict[satId].sdf.SearchType = items[8];
                satcat.SatCatDict[satId].sdf.SampleRate = items[9];
                satcat.SatCatDict[satId].sdf.MaxPathDeviation = items[10];
                satcat.SatCatDict[satId].sdf.Threshold = items[11];
                satcat.SatCatDict[satId].sdf.BeamWidth = items[12];
                satcat.SatCatDict[satId].sdf.CRC = items[13];
            }
        }

        #region SatData fields

        public struct SatDataFields
        {
            public string SatName;
            public int SatId;
            public string StartTimeAzEl;
            public string EndTimeAzEl;
            public string TrackType;
            public string TrackingFrequency;
            public string Polarity;
            public string ProgramAutoTrack;
            public string SearchType;
            public string SampleRate;
            public string MaxPathDeviation;
            public string Threshold;
            public string BeamWidth;
            public string CRC;
        }

        #endregion fields
    }
}