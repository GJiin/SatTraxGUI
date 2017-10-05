///////////////////////////////////////////////////////////////
// SatelliteCatalog.cs - SatTrax
// Perform scheduling and execution of satellite tracking.
// The program obtains a catalog of satellites and TLE data for a set of satellites.
// It predicts and produces a pass ephemeris script and connects to an MXP with a command set

// Version .01
// Author Michael Ketcham
//
// 01/27/2016 - Initial
//
// Copyright Sea Tel @ 2016 doing business as Cobham SATCOM
//////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Zeptomoby.OrbitTools;

// interesting URLS
// http://www.amsat.org/amsat/ftp/docs/spacetrk.pdf - Spacetrack Report No. 3
// http://nssdc.gsfc.nasa.gov/nmc/spacecraftDisplay.do?id=1966-027A - NASA MAster Catalog by id = International Designator (e.g. 1966-027A is "Explorer 7")
// http://nssdc.gsfc.nasa.gov/nmc/SpacecraftQuery.jsp - NASA Spacecraft Query
// https://celestrak.com/cgi-bin/TLE.pl?CATNR=00022 - CELESTRAK TLE lookup by NORAD catalog number (e.g. 00022 is "Explorer 7")
// https://en.wikipedia.org/wiki/Two-line_element_set
// http://celestrak.com/columns/v04n03/ - TLE Format
// https://en.wikipedia.org/wiki/Orbital_elements
// https://en.wikipedia.org/wiki/Epoch_(reference_date)
// https://en.wikipedia.org/wiki/Euler_angles
// http://www.amsat.org/amsat/keps/kepmodel.html - Keplerian Elements Tutorial
// http://ssd.jpl.nasa.gov/?horizons_doc

namespace SatTraxGUI
{
    using TLEDict = Dictionary<int, Tle>; // Key = Norad Num, Value = SatInfo
    using SatDict = Dictionary<int, SatInfo>; // Key = Norad Num, Value = SatInfo

    [Serializable]
    public class SatelliteCatalog
    {
        internal static SatDict _satCatDict = new SatDict();

        internal static List<string> FormatedSatNames = new List<string>();
        [NonSerialized] public string[] _listSats;

        internal TLEDict _tleDict = new TLEDict();

        /// pub/satcat.txt - Raw (CELESTRAK ) SATCAT Data - satcat.txt
        private string UrlCelestrakSatcat
        {
            //get { return "https://celestrak.com/pub/satcat.txt"; }
            get { return "http://www.celestrak.com/pub/satcat.txt"; }
        }

        internal TLEDict TleDict
        {
            get { return _tleDict; }
            set { }
        }

        public SatDict SatCatDict
        {
            get { return _satCatDict; }
            set { _satCatDict = value; }
        }

        internal static bool SatCatLoaded { get; set; }
        internal static bool TleLoaded { get; set; }
        internal static bool SatCatFailLoad { get; set; }
        internal static bool TleFailLoad { get; set; }

        static SatelliteCatalog()
        {
            var bwWorker1 = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            var satCat = new SatelliteCatalog();
            bwWorker1.DoWork += satCat.LoadSatInfoCatalog;
            bwWorker1.ProgressChanged += satCat.LoadSat_ProgressChanged;
            bwWorker1.RunWorkerCompleted += satCat.LoadSat_RunWorkerCompleted;
            bwWorker1.RunWorkerAsync();
        }

        internal void LoadSat_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            InterThreadMessaging.Instance.Progress = e.ProgressPercentage;
        }

        /// <summary>
        ///     SatCartf load complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void LoadSat_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                InterThreadMessaging.Instance.ProgressText = "Canceled!";
                SatCatFailLoad = true;
            }

            else if (e.Error != null)
            {
                InterThreadMessaging.Instance.ProgressText = "Error: " + e.Error.Message;
                SatCatFailLoad = true;
            }

            else
            {
                InterThreadMessaging.Instance.ProgressText = "Done!";
                SatCatFailLoad = false;
            }

            SatCatLoaded = true;
        }

        /// <summary>
        ///     TLE load complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void LoadTle_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                InterThreadMessaging.Instance.ProgressText = "Canceled!";
                TleFailLoad = true;
            }

            else if (e.Error != null)
            {
                InterThreadMessaging.Instance.ProgressText = "Error: " + e.Error.Message;
                TleFailLoad = true;
            }

            else
            {
                InterThreadMessaging.Instance.ProgressText = "Done!";
                TleFailLoad = false;
            }

            {
                Thread.Sleep(1);
                Application.DoEvents();
                Thread.Yield();
            }

            TleLoaded = true;
        }

        /// <summary>
        ///     Load the Master Sat Info catalog from the Celestrak site
        /// </summary>
        /// <returns></returns>
        public void LoadSatInfoCatalog(object sender, DoWorkEventArgs e)
        {
            SatCatFailLoad = false;

            new SatDict();

            if (SatCatLoaded)
            {
                InterThreadMessaging.Instance.Message = "Satellite Catalog already loaded. Using memory cache...";
                //Console.WriteLine("Sat info already loaded. Using memory cache...");
                e.Cancel = true;
                SatCatFailLoad = true;
                return;
            }

            Directory.CreateDirectory(Form1.DATA_DIRECTORY); // create if not already exists
            const string path = Form1.DATA_DIRECTORY + "SATCAT.TXT";
            //const string pathBin = Form1.DATA_DIRECTORY + "SATCAT.BIN";

            //if (File.Exists(pathBin))
            //{
            //    SatCatDict = DeSerializeSatCat(pathBin);
            //}
            //else
            //{
            if (!File.Exists(path))
            {
                InterThreadMessaging.Instance.Message = "Downloading Satellite Catalog. Takes about 45 seconds. Please stand by...";
                // GetFileViaHttp(UrlCelestrakSatcat, path);
                GetFileViaHttp_TEST(UrlCelestrakSatcat, path);
            }
            if (!File.Exists(path))
            {
                InterThreadMessaging.Instance.Message = "failed to download Satellite catalog from " + UrlCelestrakSatcat;
                return;
            }

            InterThreadMessaging.Instance.Message = "Sat info already downloaded. Using file cache...";
            _listSats = File.ReadAllLines(path);

            var worker = sender as BackgroundWorker;

            InterThreadMessaging.Instance.ResetProgress = _listSats.Length; // setup progress bar
            InterThreadMessaging.Instance.Message = "Satellite Catalog downloaded. Preparing...";

            foreach (var satInfo in _listSats.Select(line => new SatInfo(line)))
            {
                worker.ReportProgress(_listSats.Length * 10);

                {
                    Application.DoEvents();
                    Thread.Sleep(0);
                    Thread.Yield();
                }

                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    worker.CancelAsync();
                    SatCatFailLoad = true;
                    return;
                }

                if (!SatCatDict.ContainsKey(satInfo.NoradNumber))
                {
                    SatCatDict.Add(satInfo.NoradNumber, satInfo);
                }
            }

            // create a datasource for the listbox
            foreach (var name in SatCatDict.Select(satInfo => string.Format("{0} - {1}", satInfo.Value.NoradNumber, satInfo.Value.SatelliteName)))
            {
                FormatedSatNames.Add(name);
            }


            // serialize (msk - takes to long...)
            //    SerializeSatCat(pathBin, SatCatDict);
            //}

            SatCatLoaded = true;

            InterThreadMessaging.Instance.ResetProgress = 0;
        }

        private static void SerializeSatCat(string path, SatDict satcatdict)
        {
            try
            {
                Stream stream = File.Open(path, FileMode.Create);
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, satcatdict);
                stream.Close();
            }
            catch (Exception e)
            {
                InterThreadMessaging.Instance.Message = "error serializing SatCat " + e.Message;
                throw;
            }
        }

        private static SatDict DeSerializeSatCat(string path)
        {
            try
            {
                Stream stream = File.Open(path, FileMode.Open);
                var formatter = new BinaryFormatter();
                var satcatdict = (SatDict) formatter.Deserialize(stream);
                stream.Close();
                return satcatdict;
            }
            catch (Exception e)
            {
                InterThreadMessaging.Instance.Message = "error de-serializing SatCat " + e.Message;
                throw;
            }
        }

        private static void SerializeTLE(string path, TLEDict tleDict)
        {
            try
            {
                Stream stream = File.Open(path, FileMode.Create);
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, tleDict);
                stream.Close();
            }
            catch (Exception e)
            {
                InterThreadMessaging.Instance.Message = "error serializing TLE " + e.Message;
                throw;
            }
        }

        private static TLEDict DeSerializeTLE(string path)
        {
            try
            {
                Stream stream = File.Open(path, FileMode.Open);
                var formatter = new BinaryFormatter();
                var tleDict = (TLEDict) formatter.Deserialize(stream);
                stream.Close();
                return tleDict;
            }
            catch (Exception e)
            {
                InterThreadMessaging.Instance.Message = "error de-serializing TLE " + e.Message;
                throw;
            }
        }

        /// <summary>
        ///     Load the master TLE data catalog from the Space-Track site
        /// </summary>
        /// <returns></returns>
        public void LoadTleCatalog(object sender, DoWorkEventArgs e)
        {
            string[] tleLines;

            if (TleLoaded)
            {
                InterThreadMessaging.Instance.Message = "TLE info already loaded. Using memory cache...";
//                TleLoaded = true;
                e.Cancel = true;
                return;
            }

            var worker = sender as BackgroundWorker;

            const string path = Form1.DATA_DIRECTORY + "TLE.TXT";
            //const string pathBin = Form1.DATA_DIRECTORY + "TLE.BIN";

            //if (File.Exists(pathBin))
            //{
            //    TleDict = DeSerializeTLE(pathBin);
            //}
            //else
            //{
            if (!File.Exists(path))
            {
                InterThreadMessaging.Instance.Message = "Downloading TLE to file cache. Please standby...";

                var dtstart = DateTime.Now - TimeSpan.FromDays(10); //  get past 30 days
                var dtend = DateTime.Now;

                var line = GetSpaceTrack(dtstart, dtend);

                InterThreadMessaging.Instance.Message = "TLE Download complete. Processing...";

                tleLines = line.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

                // Write the contents to file.
                File.WriteAllLines(path, tleLines, Encoding.UTF8);
            }
            else
            {
                InterThreadMessaging.Instance.Message = "TLE already loaded. Using file cache...";
                tleLines = File.ReadAllLines(path);
                //TleLoaded = true;
            }

            InterThreadMessaging.Instance.ResetProgress = tleLines.Length;

            for (var i = 0; i < tleLines.Length; ++i)
            {
                worker.ReportProgress(i * 10);

                {
                    Application.DoEvents();
                    Thread.Sleep(0);
                    Thread.Yield();
                }

                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    worker.CancelAsync();
//                    TleLoaded = true;
                    return;
                }

                var name = tleLines[i].Substring(2);
                var line1 = tleLines[++i];
                var line2 = tleLines[++i];
                var tle = new Tle(name, line1, line2);
                var noradNumber = int.Parse(tle.NoradNumber);
                TleDict[noradNumber] = tle;
            }

            //    SerializeTLE(pathBin, TleDict);
            //}

            InterThreadMessaging.Instance.ResetProgress = 0;

//#if DEBUG
//            {
//                var keys = new List<string>();
//                var values = new List<string>();
//                foreach (var pair in TleDict)
//                {
//                    keys.Add(pair.Key.ToString());
//                    values.Add(pair.Value.NoradNumber + " " + pair.Value.Name);
//                }
//                File.WriteAllLines(Form1.DATA_DIRECTORY + "TLEDICT_KEYS.txt", keys, Encoding.UTF8);
//                File.WriteAllLines(Form1.DATA_DIRECTORY + "TLEDICT_VALUES.txt", values.ToArray(), Encoding.UTF8);
//            }
//#endif
//            InterThreadMessaging.Instance.ResetProgress = 0;

            //          TleLoaded = true;
        }


        public static string[] GetTLE(string NORAD_Catalog_Number)
        {
            return GetTLE(int.Parse(NORAD_Catalog_Number));
        }

        public static string[] GetTLE(int NORAD_Catalog_Number)
        {
            // Get the TLE

            //https://celestrak.com/cgi-bin/TLE.pl?CATNR=00022

            var baseURL = @"https://celestrak.com/cgi-bin/TLE.pl?CATNR=";
            var url = baseURL + string.Format("{0:0000}", NORAD_Catalog_Number);
            var result = GetDataViaHttp(url);
            var str = Encoding.UTF8.GetString(result);

            // strip out TLE
            var startPRE = str.IndexOf(@"<PRE>");
            var stopPRE = str.IndexOf(@"</PRE>");
            var strTLE = str.Substring(startPRE + 6, stopPRE - startPRE - 6);

            InterThreadMessaging.Instance.Message = "TLE: " + strTLE;

            return strTLE.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
        }

        //    Private Sub W_DownloadProgressChanged(ByVal sender As Object, ByVal e As _
        //Net.DownloadProgressChangedEventArgs) Handles W.DownloadProgressChanged

        private static int GetwebFileSize(string url)
        {
            var req = WebRequest.Create(url);
            req.Method = "HEAD";
            using (var resp = req.GetResponse())
            {
                int contentLength;
                if (int.TryParse(resp.Headers.Get("Content-Length"), out contentLength))
                {
                    return contentLength;
                }
            }
            return -1;
        }

        /// <summary>
        ///     download binary file:
        /// </summary>
        /// <param name="url"></param>
        /// <param name="uriBase"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static byte[] GetFileViaHttp(string uriBase, string path)
        {
            using (var client = new WebClient())
            {
                try
                {
                    var TLEdownloadComplete = false;
                    var raw = new byte[0];

                    // set progress bar size
                    var contentLength = GetwebFileSize(uriBase);
                    InterThreadMessaging.Instance.ResetProgress = 100; //contentLength;

                    client.DownloadProgressChanged += (sender, args) => { InterThreadMessaging.Instance.Progress = 150; };

                    client.DownloadFileCompleted += (sender, args) =>
                    {
                        InterThreadMessaging.Instance.Message = "Satellite Catalog Download Complete";
                        TLEdownloadComplete = true;
                    };

                    var uri = new Uri(uriBase);


                    if (!Directory.Exists(Form1.DATA_DIRECTORY))
                        Directory.CreateDirectory(Form1.DATA_DIRECTORY);

                    client.DownloadFileAsync(uri, path);

                    while (!TLEdownloadComplete)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                        Thread.Yield();
                    }
                    return raw;
                }
                catch (WebException ex)
                {
                    var response = (HttpWebResponse) ex.Response;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        InterThreadMessaging.Instance.Message = ex.Message;
                        return new byte[0];
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    InterThreadMessaging.Instance.Message = ex.Message;
                    return new byte[0];
                }
            }
        }

        private static byte[] GetFileViaHttp_TEST(string uriBase, string path)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFileTask(uriBase, path);
                }
                catch (Exception)
                {
                }
            }
            return new byte[0];
        }


        /// <summary>
        /// </summary>
        /// <param name="uriBase"></param>
        /// <returns></returns>
        private static byte[] GetDataViaHttp(string uriBase)
        {
            using (var client = new WebClient())
            {
                try
                {
                    var downloadComplete = false;
                    var raw = new byte[0];

                    client.DownloadProgressChanged += (sender, args) => { InterThreadMessaging.Instance.Progress = 1; };
                    client.DownloadDataCompleted += (sender, args) =>
                    {
                        InterThreadMessaging.Instance.Message = "Satellite TLE Download Complete";
                        raw = args.Result;
                        downloadComplete = true;
                    };

                    client.DownloadDataAsync(new Uri(uriBase));

                    while (!downloadComplete)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                        Thread.Yield();
                    }
                    return raw;
                }
                catch (WebException ex)
                {
                    var response = (HttpWebResponse) ex.Response;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine(ex.Message);
                        //Form1.Console_WriteLine(ex.Message);
                        return new byte[0];
                    }
                    throw;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return new byte[0];
                }
            }
        }

        // Get the TLEs based of an array of NORAD CAT IDs, start date, and end date
        //        public string GetSpaceTrack(string[] norad, DateTime dtstart, DateTime dtend)
        public string GetSpaceTrack(DateTime dtstart, DateTime dtend)
        {
            const string uriBase = "https://www.space-track.org";
            const string requestController = "/basicspacedata";
            const string requestAction = "/query";

            // URL to retrieve all the latest tle's for the provided NORAD CAT IDs for the provided Dates
            // string predicateValues   = "/class/tle_latest/ORDINAL/1/NORAD_CAT_ID/" + string.Join(",", norad) + "/orderby/NORAD_CAT_ID%20ASC/format/tle";

            // URL to retrieve all the latest 3le's for the provided NORAD CAT IDs for the provided Dates
            //string predicateValues = "/class/tle/EPOCH/" + dtstart.ToString("yyyy-MM-dd--") + dtend.ToString("yyyy-MM-dd") + "/NORAD_CAT_ID/" + string.Join(",", norad) + "/orderby/NORAD_CAT_ID%20ASC/format/3le";
            var predicateValues = "/class/tle/EPOCH/" + dtstart.ToString("yyyy-MM-dd--") + dtend.ToString("yyyy-MM-dd") + "/orderby/NORAD_CAT_ID%20ASC/format/3le";
            var request = uriBase + requestController + requestAction + predicateValues;
            request = @"https://www.space-track.org/basicspacedata/query/class/tle_latest/ORDINAL/1/EPOCH/%3Enow-365/format/3le";

            // Create new WebClient object to communicate with the service
            using (var client = new SpaceTrack.WebClientEx())
            {
                // Store the user authentication information
                var data = new NameValueCollection {{"identity", "msketcham@gmail.com"}, {"password", "Passw0rdPassw0rd"}};

                // Generate the URL for the API Query and return the response
                var response2 = client.UploadValues(uriBase + "/auth/login", data);
                var response4 = client.DownloadData(request);
                return Encoding.Default.GetString(response4);
            }
        }
    }

    public class SpaceTrack
    {
        // Get the TLEs based of an array of NORAD CAT IDs, start date, and end date
        public string GetSpaceTrack(string[] norad, DateTime dtstart, DateTime dtend)
        {
            const string uriBase = "https://www.space-track.org";
            const string requestController = "/basicspacedata";
            const string requestAction = "/query";

            // URL to retrieve all the latest tle's for the provided NORAD CAT IDs for the provided Dates
            //string predicateValues   = "/class/tle_latest/ORDINAL/1/NORAD_CAT_ID/" + string.Join(",", norad) + "/orderby/NORAD_CAT_ID%20ASC/format/tle";
            // URL to retrieve all the latest 3le's for the provided NORAD CAT IDs for the provided Dates

            var predicateValues = "/class/tle/EPOCH/" + dtstart.ToString("yyyy-MM-dd--") + dtend.ToString("yyyy-MM-dd") + "/NORAD_CAT_ID/" + string.Join(",", norad)
                                  + "/orderby/NORAD_CAT_ID%20ASC/format/3le";

            var request = uriBase + requestController + requestAction + predicateValues;

            // Create new WebClient object to communicate with the service
            using (var client = new WebClientEx())
            {
                // Store the user authentication information
                var data = new NameValueCollection {{"identity", "msketcham@gmail.com"}, {"password", "Passw0rdPassw0rd"}};

                // Generate the URL for the API Query and return the response
                var response2 = client.UploadValues(uriBase + "/auth/login", data);
                var response4 = client.DownloadData(request);

                return Encoding.Default.GetString(response4);
            }
        } // END GetSpaceTrack()

        #region Nested type: WebClientEx

        public class WebClientEx : WebClient
        {
            // Create the container to hold all Cookie objects
            private readonly CookieContainer _cookieContainer = new CookieContainer();
            // Override the WebRequest method so we can store the cookie
            // container as an attribute of the Web Request object
            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);

                if (request is HttpWebRequest)
                {
                    ( request as HttpWebRequest ).CookieContainer = _cookieContainer;
                }

                return request;
            }
        }

        #endregion
    }
}