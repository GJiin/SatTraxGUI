///////////////////////////////////////////////////////////////
// Form1.cs - SatTrax
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using SatTraxGUI.Properties;
using Zeptomoby.OrbitTools;
using Zeptomoby.OrbitTools.Pro;
using Zeptomoby.OrbitTools.Track;

namespace SatTraxGUI
{
    public partial class Form1 : Form
    {
        public enum TrackTYPE
        {
            Standard,
            Overhead,
            Tilt
        }

        private const string pathSites = DATA_DIRECTORY + @"Sites.txt";

        public static TrackTYPE TrackType = TrackTYPE.Standard;

        public SiteInfo CurrentSite { get; set; }

        public Form1()
        {
            InitilizePropertyDefaults();

            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            InitComplete = false;

            Cursor.Current = Cursors.WaitCursor;

            btnBuildScript.Enabled = false;
            btnPause.Enabled = false;
            btnRun.Enabled = false;
            btnRun.Tag = "Run";

            tabControl1.Enabled = false;
            tabControl2.Enabled = false;


            InterThreadMessaging.Instance.PropertyChanged += OnInterThreadMessagingPropertyChanged;
            InterThreadMessaging.Instance.CollectionChanged += OnInterThreadMessagingCollectionChanged;

            // Fill cache with Master Sat Cat (from "https://celestrak.com/pub/satcat.txt");
            InitializeSatelliteInfo();

            InitializeSites();

            InitilizeFavoriteSats();

            // initial epoch and interval

            dateTimePicker_UTC_Start.Value = new DateTime(2015, 3, 16, 0, 0, 0);
            dateTimePicker_UTC_End.Value = dateTimePicker_UTC_Start.Value.AddDays(1);

            dateTimePicker_LOCAL_Start.Value = dateTimePicker_UTC_Start.Value.ToLocalTime();
            dateTimePicker_LOCAL_End.Value = dateTimePicker_UTC_End.Value.ToLocalTime();

            //dateTimePicker_UTC_Start.Value = DateTime.UtcNow;
            //dateTimePicker_UTC_End.Value = dateTimePicker_UTC_Start.Value.AddHours(1);

            //dateTimePicker_LOCAL_Start.Value = dateTimePicker_UTC_Start.Value.ToLocalTime();
            //dateTimePicker_LOCAL_End.Value = dateTimePicker_UTC_End.Value.ToLocalTime();

            // initial script filename
            EventScriptFilename.Text = DATA_DIRECTORY + @"EventScript.txt";

            CLI_IP = ipAddressControl1.Text;
            CLI_PORT = tbMXPPORT.Text;
            CLI_MASK = tbMXPMASK.Text;
            CurrentSite.IP = CLI_IP;
            CurrentSite.PORT = CLI_PORT;
            CurrentSite.MASK = CLI_MASK;

            LeadtimeMs = 500;
            tbLeadTime_ms.Text = LeadtimeMs.ToString();

            DelaySec = double.Parse(EventInterval.Text);

            {
                // use default
                var noradNum = 32951;
                var satName = "GALAXY 18"; // (G-18) //25994 - TERRA");
                var defaultFav = string.Format("{0} - {1}", noradNum, satName);

                SetFavorite(defaultFav);
                SetSatInfo(noradNum);
                SetSatData(noradNum);
                SetTleInfo(satName);
            }

            Console_WriteLine("Initialization Complete.");

            tabControl1.Enabled = true;
            tabControl2.Enabled = true;

            Cursor.Current = Cursors.Default;

            InitComplete = true;
        }

        private void InitializeSatelliteInfo()
        {
            if ( LoadSatcat() )
            {
                SatData.LoadSatData(SatCat); // load satellite RF data

                LoadTLE();
            }
            GenerateScriptButton.Enabled = false;
        }

        private void InitilizeFavoriteSats()
        {
            Console_WriteLine("Initializing Favorites Info...");

            Fav_ListBox1.Items.Clear();

            UpdateFavorites("The Sun");

            UpdateFavorites(28154); // AMC-10 (GE-10)

            UpdateFavorites(32951); // GALAXY 18 (123W)

            UpdateFavorites(25994); // TERRA

            UpdateFavorites(27424); // AQUA

            UpdateFavorites(37849); // SUOMI NPP

            UpdateFavorites(37214); // FENGYUN 3B
        }

        private bool LoadSatcat()
        {
            Console_WriteLine("Initializing Satellite Catalog Info...");
            SetProgressText("Initializing Satellite Catalog Info");

            SatCat = new SatelliteCatalog();

            while ( ! SatelliteCatalog.SatCatLoaded && ! SatelliteCatalog.SatCatFailLoad )
            {
                Application.DoEvents();
                Thread.Sleep(1);
                Thread.Yield();
            }

            listBoxSatelliteName.DataSource = SatelliteCatalog.FormatedSatNames;

            SetProgressText("");

            return ! SatelliteCatalog.SatCatFailLoad;
        }

        private void LoadTLE()
        {
            Console_WriteLine("Loading Satellite TLE...");

            var bwWorker1 = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            bwWorker1.DoWork += SatCat.LoadTleCatalog;
            bwWorker1.ProgressChanged += SatCat.LoadSat_ProgressChanged;
            bwWorker1.RunWorkerCompleted += SatCat.LoadTle_RunWorkerCompleted;
            bwWorker1.RunWorkerAsync();

            while ( ! SatelliteCatalog.TleLoaded )
            {
                Thread.Sleep(1);
                Application.DoEvents();
                Thread.Yield();
            }

            if ( SatelliteCatalog.TleFailLoad )
            {
                Console_WriteLine("Failed to load TLE Catalog.");
            }
        }

        private void SetTleInfo(string satName)
        {
            var tle = SatCat.TleDict.Values.FirstOrDefault(t => t.Name == satName);
            SetTleInfo(tle);
        }

        private void SetTleInfo(Tle tle)
        {
            if ( tle == null )
                return;

            TLE_Line1.Text = tle.Line1;
            TLE_Line2.Text = tle.Line2;

            // See http://celestrak.com/columns/v04n03/

            // Line 1
            //Field	Column	Description

            //1.1	01	Line Number of Element Data

            //tle.Name;
            TLE_Line1_SatelliteNumber.Text = tle.NoradNumber;
            TLE_Line1_Classification.Text = tle.Classification;
            TLE_Line1_LaunchYear.Text = tle.LaunchYear;
            TLE_Line1_LaunchNum.Text = tle.LaunchNum;
            TLE_Line1_LaunchPiece.Text = tle.LaunchPiece;
            TLE_Line1_EpochYear.Text = tle.EpochYear;
            TLE_Line1_EpochDayPart.Text = tle.EpochDayPart;
            var TLE_Line1_Epoch = tle.Epoch;
            var TLE_Line1_EpochJulian = tle.EpochJulian.ToTime().ToString(CultureInfo.InvariantCulture);
            TLE_Line1_MeanMotionFirstDt.Text = tle.MeanMotion;
            TLE_Line1_MeanMotionSecondDt.Text = tle.MeanMotionDt2;
            TLE_Line1_BSTAR.Text = tle.BStarDrag;
            TLE_Line1_EphemerisType.Text = tle.EphemerisType;
            TLE_Line1_ElementSetNumber.Text = tle.ElSetNum;
            TLE_Line1_Checksum.Text = tle.Line1Checksum;

            // Line1 2
            TLE_Line2_SatelliteNumber.Text = tle.SatNumber;
            TLE_Line2_Inclination.Text = tle.Inclination;
            TLE_Line2_Ascension.Text = tle.RAAscendingNode;
            TLE_Line2_Eccentricity.Text = tle.Eccentricity;
            TLE_Line2_PerigeeArg.Text = tle.ArgPerigee;
            TLE_Line2_MeanAnomaly.Text = tle.MeanAnomaly;
            TLE_Line2_MeanMotion.Text = tle.MeanMotion;
            TLE_Line2_RevolutionNum.Text = tle.OrbitAtEpoch;
            TLE_Line2_Checksum.Text = tle.Line2Checksum;

            labelEpochInfo.Text = TLE_Line1_Epoch;

            Console_WriteLine("Updated TLE Info for {0} ({1})", tle.Name, tle.NoradNumber);
        }

        private void ResetTleInfo()
        {
            TLE_Line1.Text = @"*** TLE DATA UNAVAILABLE ***";
            TLE_Line2.Text = string.Empty;

            TLE_Line1_SatelliteNumber.Text = string.Empty;
            TLE_Line1_Classification.Text = string.Empty;
            TLE_Line1_LaunchYear.Text = string.Empty;
            TLE_Line1_LaunchNum.Text = string.Empty;
            TLE_Line1_LaunchPiece.Text = string.Empty;
            TLE_Line1_EpochYear.Text = string.Empty;
            TLE_Line1_EpochDayPart.Text = string.Empty;
            var TLE_Line1_Epoch = string.Empty;
            var TLE_Line1_EpochJulian = string.Empty;
            TLE_Line1_MeanMotionFirstDt.Text = string.Empty;
            TLE_Line1_MeanMotionSecondDt.Text = string.Empty;
            TLE_Line1_BSTAR.Text = string.Empty;
            TLE_Line1_EphemerisType.Text = string.Empty;
            TLE_Line1_ElementSetNumber.Text = string.Empty;
            TLE_Line1_Checksum.Text = string.Empty;

            TLE_Line2_SatelliteNumber.Text = string.Empty;
            TLE_Line2_Inclination.Text = string.Empty;
            TLE_Line2_Ascension.Text = string.Empty;
            TLE_Line2_Eccentricity.Text = string.Empty;
            TLE_Line2_PerigeeArg.Text = string.Empty;
            TLE_Line2_MeanAnomaly.Text = string.Empty;
            TLE_Line2_MeanMotion.Text = string.Empty;
            TLE_Line2_RevolutionNum.Text = string.Empty;
            TLE_Line2_Checksum.Text = string.Empty;

            labelEpochInfo.Text = string.Empty;
        }

        private void SetFavorite(string text)
        {
            var idx = listBoxSatelliteName.FindString(text);
            listBoxSatelliteName.SelectedIndex = idx;
            idx = Fav_ListBox1.FindString(text);
            Fav_ListBox1.SelectedIndex = idx;
        }

        private void UpdateFavorites < T >(T item)
        {
            Directory.CreateDirectory(DATA_DIRECTORY); // create if not already exists

            const string path = DATA_DIRECTORY + "FAVORITES.TXT";
            var items = new List < string >();
            if ( File.Exists(path) )
                items = File.ReadAllLines(path).ToList();

            string info;

            if ( item is int )
            {
                SatInfo satInfo;
                if ( ! SatCat.SatCatDict.TryGetValue((int)Convert.ChangeType(item, typeof ( T )), out satInfo) )
                    return;

                info = string.Format("{0} - {1}", satInfo.NoradNumber, satInfo.SatelliteName);
            }
            else if ( item is string )
            {
                info = string.Format("{0} - {1}", "00000", item);
            }
            else
            {
                return;
            }

            if ( ! items.Contains(info) )
            {
                ( (IList)items ).Add(info);
            }

            Fav_ListBox1.Items.Clear();
            foreach ( var o in items )
            {
                Fav_ListBox1.Items.Add(o);
            }
            File.WriteAllLines(path, Enumerable.ToArray(items));
        }

        private int GetSatInfo()
        {
            var selectedText = listBoxSatelliteName.Text;
            Console_WriteLine("Getting Sat Info for {0}", selectedText);

            var noradNum = Convert.ToInt32(selectedText.Substring(0, selectedText.IndexOf("- ")));
            if ( ! SatCat.SatCatDict.ContainsKey(noradNum) )
            {
                var msg1 = string.Format("*** Failed to find {0} in Satellite Catalog. ***", selectedText);
                Console_WriteLine(msg1);
                ResetTleInfo();
            }

            var satInfo = SatCat.SatCatDict[noradNum];
            SetSatInfo(satInfo);

            if ( ! SatCat.TleDict.ContainsKey(noradNum) )
            {
                Console_WriteLine(string.Format("*** Failed to find {0} in NORAD TLE Catalog ***", selectedText));
                Console_WriteLine("Attempting to locate in CELESTRAK database. Please stand by...");

                // try  CELESTRAK
                var tle = SatelliteCatalog.GetTLE(noradNum);
                if ( tle[0] != "No TLE found" )
                {
                    for ( var i = 0; i < tle.Length; i++ )
                    {
                        tle[i] = tle[i].Trim();
                    }
                    var newtle = new Tle(tle[0], tle[1], tle[2]);

                    SatCat.TleDict.Add(noradNum, newtle);
                }

                if ( ! SatCat.TleDict.ContainsKey(noradNum) )
                {
                    Console_WriteLine(string.Format("*** Failed to find {0} in CELESTRAK TLE Catalog. ***", selectedText));
                    ResetTleInfo();
                    return -1;
                }
            }

            return noradNum;
        }

        private void SetSatInfo(int noradNum)
        {
            SatInfo satInfo;
            if ( ! SatCat.SatCatDict.TryGetValue(noradNum, out satInfo) ) return;
            SetSatInfo(satInfo);
        }

        private void SetSatInfo(SatInfo item)
        {
            SatelliteInfo_Designator.Text = item.IntlDesc;
            SatelliteInfo_NoradNumber.Text = item.NoradNumber.ToString();
            SatelliteInfo_MultipleNameFlag.Text = item.MultipleNameFlag;
            SatelliteInfo_PayloadFlag.Text = item.PayloadFlag;
            SatelliteInfo_OperationalStatusCode.Text = item.OperationalStatusCode;
            SatelliteInfo_SourceOrOwnership.Text = item.SourceOrOwnership;
            SatelliteInfo_LaunchDate.Text = item.LaunchDate;
            SatelliteInfo_LaunchSite.Text = item.LaunchSite;
            SatelliteInfo_DecayDate.Text = item.DecayDate;
            SatelliteInfo_OrbitalPeriod.Text = item.OrbitalPeriod.ToString(CultureInfo.InvariantCulture);
            SatelliteInfo_Inclination_degrees.Text = item.Inclination.ToString(CultureInfo.InvariantCulture);
            SatelliteInfo_Apogee_Altitude_kilometers.Text = item.ApogeeAltitude.ToString().Trim();
            SatelliteInfo_Perigee_Altitude_kilometers.Text = item.PerigeeAltitude.ToString().Trim();
            SatelliteInfo_Radar_Cross_Section_meters2.Text = item.RadarCrossSection.ToString(CultureInfo.InvariantCulture);
            SatelliteInfo_Orbital_Status_Code.Text = item.OrbitalStatusCode;

            var name = string.Format("{0} - {1}", item.NoradNumber, item.SatelliteName);
            listBoxSatelliteName.Text = name;

            TLE_SatelliteName.Text = item.SatelliteName;

            Console_WriteLine("Updated Satellite Info");
        }

        private void ShowPasses(string satName)
        {
            boxPasses.Items.Clear();

            var satSelectedFavorite = satName;

            if ( string.IsNullOrEmpty(satSelectedFavorite.Trim()) )
            {
                return;
            }

            const int start = 0;
            var end = satSelectedFavorite.IndexOf(" - ");
            var strNoradNum = satSelectedFavorite.Substring(start, end - start);
            var noradNum = int.Parse(strNoradNum);

            // Now create a site object. Site objects represent a location on the surface of the earth.
            var siteName = tbLocationName.Text;
            var siteLat = double.Parse(LocationLatitude.Text);
            var siteLon = double.Parse(LocationLongitude.Text);
            var siteEl = double.Parse(LocationElevation.Text);
            var site = new Site(siteLat, siteLon, siteEl, new Wgs84(), siteName); // 0.00 N, 100.00 W, 0 km altitude

            // utcT0 and utcT1 are used to determine the time window over which
            // to search for satellite passes.

            boxPasses.Items.Clear();
            Console_WriteLine("Generating satellite pass predictions...");

            var t1 = dateTimePicker_UTC_Start.Value;
            var t2 = dateTimePicker_UTC_End.Value;

            var passList = new List < PassInfo >();

            if ( ! satSelectedFavorite.EndsWith("00000 - The Sun") )
            {
                var satInfo = SatCat.SatCatDict[noradNum];
                if ( ! SatCat.TleDict.ContainsKey(noradNum) )
                {
                    ResetTleInfo();
                    var msg1 = string.Format("*** Failed to find {0} in CELESTRAK TLE Catalog. ***", satName);
                    Console_WriteLine(msg1);
                    ResetTleInfo();
                    return;
                }

                var tle = SatCat.TleDict[noradNum];

                SetTleInfo(tle);
                SetSatInfo(satInfo);

                passList = Pass.Predict(tle, site, t1, t2);
            }
            else
            {
                // The Sun
                // one pass for each day in time frame..
                var x = ( t2 - t1 ).Days + 1;
                var t = t1;
                for ( var i = 1; i <= x; i++ )
                {
                    passList.Add(new PassInfo(t.ToString(), t.ToLocalTime().ToString()));
                    t = t1.AddDays(i);
                }
            }

            foreach ( var pass in passList )
            {
                if ( radioButton1.Checked )
                    boxPasses.Items.Add(pass.UTC);
                else
                    boxPasses.Items.Add(pass.Local);
            }
        }

        private void ShowScript(IEnumerable < string > script)
        {
            StopAction = false;

            listEventScript.Items.Clear();

            var processTheSun = false;

            foreach ( var line in script )
            {
                var l0 = line;
                if ( line.StartsWith("#") )
                {
                    if ( line.Contains("Sat:The Sun") )
                    {
                        processTheSun = true;
                    }
                }
                else
                {
                    string[] l = null;
                    var evt = "";
                    var t = "";
                    double az;
                    double el;
                    var cl = 0.0;

                    l = line.Split(',');

                    if ( TrackType == TrackTYPE.Standard
                         || processTheSun
                         || l.Length == 3 )
                    {
                        t = l[0];
                        az = double.Parse(l[1]);
                        el = double.Parse(l[2]);
                    }
                    else
                    {
                        l = line.Split(',', ' ');
                        evt = l[0];
                        t = l[1];
                        az = double.Parse(l[2]);
                        el = double.Parse(l[3]);
                        if ( l.Count() > 3 )
                            cl = double.Parse(l[4]);
                    }
                    l0 = string.Format("T:{0} {1} AZ:{2,3:F12} EL:{3,3:F12} CL:{4,3:F12}", evt, t, az, el, cl);
                }

                Event_Writeline(l0);
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                    Thread.Yield();
                }

                if ( StopAction )
                {
                    Event_Writeline("===============================  ABORTED - END OF DATA  ===============================");
                    StopAction = false;
                    return;
                }
            }
        }

        private void InitializeSites()
        {
            Console_WriteLine("Initializing Site Info...");

            if ( File.Exists(pathSites) )
            {
                SitesLoad();
            }
            else
            {
                const double degLat = 38.006603;
                const double degLon = -122.043495;
                const double kmAlt = 0.0; // 0.0131064; // 43 ft
                Sites.Add(new SiteInfo(new Site(degLat, degLon, kmAlt, new Wgs84(), "Seatel Concord FRANK_I"), "10.1.1.100", "2003", "255.255.255.0"));
            }

            PopulateSiteFields();

            CurrentSite = Sites[0];
        }

        private Site GetSite()
        {
            var siteName = tbLocationName.Text;
            if ( string.IsNullOrEmpty(siteName) )
                return new Site(0, 0, 0, new Wgs84(), "Unknown");

            var siteLat = double.Parse(LocationLatitude.Text);
            var siteLon = double.Parse(LocationLongitude.Text);
            var siteEl = double.Parse(LocationElevation.Text);
            var site = new Site(siteLat, siteLon, siteEl, new Wgs84(), siteName); // 0.00 N, 100.00 W, 0 km altitude
            return site;
        }

        private void SitesLoad()
        {
            if ( File.Exists(pathSites) )
            {
                Sites.Clear();
                var sites = File.ReadAllLines(pathSites);
                foreach ( var site in sites )
                {
                    try
                    {
                        var fields = site.Split(',');
                        var name = fields.Length > 0 ? fields[0].Replace('"', ' ').Trim() : "Unknown";
                        var lla = fields.Length > 1 ? fields[1].Trim().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries) : new[] {"0", "0", "0"};
                        double degLat;
                        double.TryParse(lla[0].Substring(0, lla[0].Length - 1), out degLat); // remove N/s
                        if ( lla[0].Contains('S') ) degLat = -degLat;
                        double degLon;
                        double.TryParse(lla[1].Substring(0, lla[1].Length - 1), out degLon); // remove W/E
                        if ( lla[1].Contains('W') ) degLon = -degLon;
                        double kmAlt;
                        double.TryParse(lla[2].Substring(0, lla[2].Length - 2), out kmAlt); // remove 'km'
                        kmAlt *= .001; // meters to km
                        var ip = fields[9];
                        var port = fields[10];
                        var mask = fields[11];

                        Console_WriteLine("Loading site: {0} @ {1}:{2}", name, ip, port);

                        Sites.Add(new SiteInfo(new Site(degLat, degLon, kmAlt, new Wgs84(), name), ip, port, mask));

                        PopulateSiteFields();
                    }
                    catch ( Exception ex )
                    {
                        Console_WriteLine("error parsing site. {0}", ex.Message);
                    }
                }
            }
        }

        private void SitesSave()
        {
            var sites = new List < string >();
            foreach ( var siteInfo in Sites.Select(s => string.Format("{0},{1},{2},{3}", s, s.IP, s.PORT, s.MASK)) )
            {
                Console_WriteLine("Site info: {0}", siteInfo);
                sites.Add(siteInfo);
            }

            File.WriteAllLines(pathSites, sites);
        }

        private void ShowPasses()
        {
            string satName;
            if ( Fav_ListBox1.SelectedIndex != -1 )
            {
                satName = Fav_ListBox1.Text;
                ShowPasses(satName);
                return;
            }

            if ( listBoxSatelliteName.SelectedItem == null ) return;

            satName = listBoxSatelliteName.SelectedItem.ToString();
            var temp1 = satName.Substring(0, satName.IndexOf(" "));
            var temp2 = satName.Substring(satName.IndexOf(" - ") + 3);
            var temp3 = temp2 + " (" + temp1 + ")";

            ShowPasses(temp3);
        }

        private void GenSchedule()
        {
            var satName = listBoxSatelliteName.SelectedItem != null ? listBoxSatelliteName.SelectedItem.ToString() : listBoxSatelliteName.Text;

            if ( Fav_ListBox1.SelectedIndex != -1 )
            {
                satName = Fav_ListBox1.Text;
            }
            else
            {
                var temp1 = satName.Substring(0, satName.IndexOf(" "));
                var temp2 = satName.Substring(satName.IndexOf(" - ") + 3);
                var temp3 = temp2 + " (" + temp1 + ")";
                satName = temp3;
            }

            var AOS = new DateTime();
            var LOS = new DateTime();

            if ( satName.EndsWith("The Sun") )
            {
                AOS = dateTimePicker_UTC_Start.Value;
                LOS = dateTimePicker_UTC_End.Value;
            }
            else
            {
                var pass = boxPasses.Text;
                if ( ! string.IsNullOrEmpty(pass) )
                {
                    // SelectedItem = "2015-09-14 21:20:34 PDT AOS [AZ 099] 21:25:01 PDT MAX [AZ 059 EL  6.5] 21:29:27 PDT LOS [AZ 018]"}

                    var strT1 = string.Empty;
                    var strT1A = string.Empty;
                    var strT2A = string.Empty;

                    try
                    {
                        // AOS
                        const int startT1 = 0;
                        var endT1 = pass.IndexOf("AOS");
                        var endT1A = pass.IndexOf(" ");
                        strT1 = pass.Substring(startT1, endT1 - 1);
                        strT1A = pass.Substring(startT1, endT1A);
                        string strT1B;
                        if ( radioButton1.Checked )
                        {
                            strT1B = pass.Substring(endT1A + 1, pass.IndexOf("UTC") - 3 - endT1A + 1);
                            if ( strT1B == "--:--:--" )
                            {
                            }
                        }
                        else
                        {
                            strT1B = pass.Substring(endT1A + 1, pass.IndexOf("LST") - 3 - endT1A + 1);
                            if ( strT1B == "--:--:--" )
                            {
                            }
                        }
                        AOS = DateTime.Parse(strT1A + " " + strT1B);
                    }
                    catch ( Exception ex )
                    {
                        Console_WriteLine("'{0}' {1}", strT1, ex.Message);
                        return;
                    }

                    try
                    {
                        // MAX EL
                        var endT2 = pass.LastIndexOf("LST") + 3;
                        if ( radioButton1.Checked )
                        {
                            endT2 = pass.LastIndexOf("UTC") + 3;
                        }

                        var x = pass.Substring(0, endT2);
                        var startT2 = x.LastIndexOf("]") + 1;
                        var strT2 = pass.Substring(startT2, endT2 - startT2);
                        strT2A = strT1A + " " + strT2;
                        var strT2B = strT2.Substring(0, strT2.LastIndexOf(" ")).Trim();
                        if ( radioButton1.Checked )
                        {
                            if ( strT2B == "--:--:--" )
                            {
                            }
                        }
                        else
                        {
                            if ( strT2B == "--:--:--" )
                            {
                            }
                        }
                    }
                    catch ( Exception ex )
                    {
                        Console_WriteLine("'{0}' {1}", strT2A, ex.Message);
                        return;
                    }

                    try
                    {
                        // SelectedItem = "2015-09-14 21:20:34 PDT AOS [AZ 099] 21:25:01 PDT MAX [AZ 059 EL  6.5] 21:29:27 PDT LOS [AZ 018]"}

                        // LOS
                        var endT2 = pass.LastIndexOf("LOS");
                        if ( radioButton1.Checked )
                        {
                            endT2 = pass.LastIndexOf("UTC");
                        }

                        var x = pass.Substring(0, endT2);
                        var startT2 = x.LastIndexOf("]") + 2;
                        var strT2 = pass.Substring(startT2, endT2 - startT2);
                        strT2A = strT1A + " " + strT2;
                        var strT2B = strT2.Substring(0, strT2.LastIndexOf(" ")).Trim();
                        if ( radioButton1.Checked )
                        {
                            if ( strT2B == "--:--:--" )
                            {
                            }
                        }
                        else
                        {
                            if ( strT2B == "--:--:--" )
                            {
                            }
                        }
                        LOS = DateTime.Parse(strT2A);
                    }
                    catch ( Exception ex )
                    {
                        Console_WriteLine("'{0}' {1}", strT2A, ex.Message);
                        return;
                    }
                }
            }

            PredictOrbit(AOS, LOS, Fav_ListBox1.Text.EndsWith("The Sun"));
        }

        private IEnumerable < string > BuildHeaderInfo(ScriptInfo scriptInfo)
        {
            var satName = scriptInfo.satName;
            var noradNum = scriptInfo.noradNum;
            var t1 = scriptInfo.utcT0;
            var t2 = scriptInfo.utcT1;
            var nEventInterval = scriptInfo.nEventIntervalSec;
            var site = scriptInfo.site;
            var passinfo = scriptInfo.passinfo;
            var duration = ( scriptInfo.utcT1 - scriptInfo.utcT0 ).Seconds;

            var ll = new List < string >
            {
                string.Format("# Sat:{0}, Site:{1}, Track Type:{2}, Lat:{3}, Lon:{4}", satName, tbLocationName.Text, CurrentSite.TrackType, LocationLatitude.Text, LocationLongitude.Text),
                string.Format("# Events:{0}, Period:{1}, Pass Length:{2} Pass Info:{3}", ListScript.Count(), nEventInterval, duration, passinfo)
            };

            SatInfo satInfo;
            if ( ! SatCat.SatCatDict.TryGetValue(noradNum, out satInfo) ) return ll;

            tbStartTimeAzEl.Text = satInfo.sdf.StartTimeAzEl;
            tbStopTimeAzEl.Text = satInfo.sdf.EndTimeAzEl;

            tbThreshold.Text = satInfo.sdf.Threshold;
            tbMaxPathDev.Text = satInfo.sdf.MaxPathDeviation;
            tbSearchType.Text = satInfo.sdf.SearchType;
            tbTrackingFreq.Text = satInfo.sdf.TrackingFrequency;

            tbProgAutoTrack.Text = satInfo.sdf.ProgramAutoTrack;
            tbSampleRate.Text = satInfo.sdf.SampleRate;
            tbPolarity.Text = satInfo.sdf.Polarity;
            tbTrackType.Text = satInfo.sdf.TrackType;

            ll.Add(string.Format("# StartTimeAzEl:{0}, EndTimeAzEl:{1}, TrackType:{2}, TrackingFrequency:{3}, " +
                                 "Polarity:{4}, ProgramAutoTrack:{5}, SearchType:{6}, SampleRate:{7}, MaxPathDeviation:{8}, Threshold:{9}, BeamWidth:{10}, CRC:{11}",
                satInfo.sdf.StartTimeAzEl, satInfo.sdf.EndTimeAzEl, satInfo.sdf.TrackType, satInfo.sdf.TrackingFrequency, satInfo.sdf.Polarity, satInfo.sdf.ProgramAutoTrack,
                satInfo.sdf.SearchType, satInfo.sdf.SampleRate, satInfo.sdf.MaxPathDeviation, satInfo.sdf.Threshold, satInfo.sdf.BeamWidth, satInfo.sdf.CRC));

            return ll;
        }

        private List < Command > ConvertScriptToCommands(List < string > list)
        {
            if ( list.Count < 1 )
            {
                Console_WriteLine("ERROR: No Command list");
                return new List < Command >();
            }

            var includeCl = list[list.Count - 3].Split(',').Length > 3;
            Regex r;
            r = includeCl
                ? new Regex(@"^.* (?<TIME_UTC>[^,]*),(?<AZ>[-\d\.]+),(?<EL>[-\d\.]+),(?<CL>[-\d\.]+)$", RegexOptions.None)
                : new Regex(@"^(?<TIME_UTC>[^(]*)\((?<TIME_LOCAL>[^)]*)\),(?<AZ>[-\d\.]+),(?<EL>[-\d\.]+)$", RegexOptions.None);

            var cmds = ( from scriptLine in list
                where ! scriptLine.StartsWith("#")
                let m = r.Match(scriptLine)
                where m.Success
                let timeUTC = r.Match(scriptLine).Result("${TIME_UTC}")
                let timeLOCAL = r.Match(scriptLine).Result("${TIME_LOCAL}")
                let az = r.Match(scriptLine).Result("${AZ}")
                let el = r.Match(scriptLine).Result("${EL}")
                let cl = r.Match(scriptLine).Result("${CL}")
                let resultAz = string.Format("SET ANTENNA AZ_TARGET {0}", az)
                let resultEl = string.Format("SET ANTENNA EL_TARGET {0}", el)
                let resultCl = string.Format("SET ANTENNA CL_TARGET {0}", includeCl ? cl : "0.0")
                select new Command
                {
                    When = DateTime.ParseExact(timeUTC, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    TextCmdAz = resultAz,
                    TextCmdEl = resultEl,
                    TextCmdCl = resultCl
                } ).ToList();

            btnRun.Enabled = true;

            return cmds;
        }

        private void SetSatData()
        {
            var selectedText = listBoxSatelliteName.Text;
            Console_WriteLine("Getting Sat Info for {0}", selectedText);

            var noradNum = Convert.ToInt32(selectedText.Substring(0, selectedText.IndexOf("- ")));
            SetSatData(noradNum);
        }

        private void SetSatData(int noradNum)
        {
            SatInfo satInfo;
            if ( ! SatCat.SatCatDict.TryGetValue(noradNum, out satInfo) ) return;
            SetSatData(satInfo);
        }

        private void SetSatData(SatInfo item)
        {
            tbStartTimeAzEl.Text = item.sdf.StartTimeAzEl;
            tbStopTimeAzEl.Text = item.sdf.EndTimeAzEl;

            tbThreshold.Text = item.sdf.Threshold;
            tbMaxPathDev.Text = item.sdf.MaxPathDeviation;
            tbSearchType.Text = item.sdf.SearchType;
            tbTrackingFreq.Text = item.sdf.TrackingFrequency;

            tbProgAutoTrack.Text = item.sdf.ProgramAutoTrack;
            tbSampleRate.Text = item.sdf.SampleRate;
            tbPolarity.Text = item.sdf.Polarity;
            tbTrackType.Text = item.sdf.TrackType;
        }

        private void SetAntennaType(TrackTYPE typ)
        {
            TrackType = typ;
            CurrentSite.TrackType = typ;
        }

        private bool Connect()
        {
            if ( Io != null )
                return true;

            Io = new IO(this);

            if ( ! Io.ConnectAndLoginCLI(CurrentSite.IP, CurrentSite.PORT) )
            {
                Io = null;
                return false;
            }

            // CLI version
            Io.Write("CLI", "VERSION SYSTEM DEBUG ALL");
            Thread.Sleep(2500); // CLI versions debug all takes a bit more time
            var versionCli = Io.WaitResponseCLI();
            if ( versionCli == null )
                versionCli = "Unable to login.";

            Console_WriteLine("\nCLI Version:\n{0}", versionCli);
            Console_WriteLine("----------------------------------------");

            return true;
        }

        private void SendCommandTriplet(Command cmd)
        {
            Console_WriteLine("Send: {0}({1}):{2}", cmd.When, cmd.When.ToLocalTime(), cmd.TextCmdAz);
            Send(cmd.TextCmdAz);

            Console_WriteLine("Send: {0}({1}):{2}", cmd.When, cmd.When.ToLocalTime(), cmd.TextCmdEl);
            Send(cmd.TextCmdEl);

            Console_WriteLine("Send: {0}({1}):{2}", cmd.When, cmd.When.ToLocalTime(), cmd.TextCmdCl);
            Send(cmd.TextCmdCl);
        }

        private void Send(string str)
        {
            if ( ! Connect() ) // connect if not already
            {
                Console_WriteLine("\nFail To connect:\n");
                return;
            }

            Io.Write(InterfaceMode, str);

            //var sw = new Stopwatch();
            //sw.Start();
            //var response = Io.WaitResponseCLI(InterfaceMode);
            //while ( response == null && sw.ElapsedMilliseconds < 1000 )
            //{
            //    //response = "Timeout";
            //    Io.Write(InterfaceMode, "\r");
            //    Thread.Sleep(1);
            //    response = Io.WaitResponseCLI(InterfaceMode);
            //}

            //if ( ! string.IsNullOrEmpty(response) )
            //{
            //    // Console_WriteLine("Response: {0}", response);
            //}
        }

        private void ResetCommandExpired()
        {
            for ( var i = 0; i < Commands.Count; i++ )
            {
                var c = Commands[i];
                c.Expired = false;
                Commands[i] = c; // set to expired
            }
        }

        private void SetCommandExpired(int i)
        {
            var c = Commands[i];
            c.Expired = true;
            Commands[i] = c; // set to expired
        }

        private void ClearConsole_Click(object sender, EventArgs e)
        {
            ConsoleListBox.Items.Clear();
        }

        #region struct Command

        public struct Command
        {
            public bool Expired { get; set; }
            public DateTime When { get; set; }
            public string TextCmdAz { get; set; }
            public string TextCmdEl { get; set; }
            public string TextCmdCl { get; set; }
        }

        #endregion struct Command

        #region class SiteInfo

        public class SiteInfo : Site
        {
            public string IP { get; set; }
            public string PORT { get; set; }
            public string MASK { get; set; }
            public TrackTYPE TrackType { get; set; }

            public SiteInfo(Site site, string ip, string port, string mask)
                : base(site.Geo.LatitudeDeg, site.Geo.LongitudeDeg, site.Geo.Altitude, site.WgsModel, site.Name)
            {
                IP = ip;
                PORT = port;
                MASK = mask;
            }
        }

        #endregion class SiteInfo

        #region struct ScriptInfo

        private struct ScriptInfo
        {
            public string satName;
            public int noradNum;
            public DateTime utcT0;
            public DateTime utcT1;
            public double nEventIntervalSec;
            public Site site;
            public string passinfo;
        }

        #endregion struct ScriptInfo

        #region Predict Orbit

        private void PredictOrbit(DateTime t1, DateTime t2, bool predictTheSun = false)
        {
            var nEventInterval = double.Parse(EventInterval.Text);

            var noradNum = int.Parse(SatelliteInfo_NoradNumber.Text);
            var satName = listBoxSatelliteName.Text;

            if ( predictTheSun )
            {
                // haque
                noradNum = 0;
                satName = "The Sun";
            }

            var si = new ScriptInfo
            {
                satName = satName,
                noradNum = noradNum,
                utcT0 = t1,
                utcT1 = t2,
                nEventIntervalSec = nEventInterval,
                site = GetSite(),
                passinfo = boxPasses.Text
            };

            Console_WriteLine("Generating script: {0} @ {1}", satName, si.site.Name);

            var thread = new Thread(PredictOrbit_WorkerThread);
            thread.Start(si);
            thread.Join();

            btnBuildScript.Enabled = true;
        }

        private void PredictOrbit_WorkerThread(object scriptInfo)
        {
            var si = (ScriptInfo)scriptInfo;
            var satName = si.satName;
            var noradNum = si.noradNum;
            var t1 = si.utcT0;
            var t2 = si.utcT1;
            var nEventInterval = si.nEventIntervalSec;
            var site = si.site;

            var predictingTheSun = satName == "The Sun";

            //if (predictingTheSun)
            //{
            //    t1 = dateTimePicker_UTC_Start.Value;
            //    t2 = dateTimePicker_UTC_End.Value;
            //}

            //var i1 = t2.Subtract(t1).TotalSeconds;
            //if (i1 > 100)
            //{
            //    var message = string.Format("Attempt to generate {0} pass elements. Are you sure?", i1);
            //    var caption = "Possible too many data points";
            //    var result = Console.In();
            //        // Console_MessageBoxYesNo(caption, message);
            //    if (result != DialogResult.Yes)
            //    {
            //        return;
            //    }
            //}

            ListScript.Clear();
            ListScript.Insert(0, "# ===============================  START OF DATA  ===============================");

            Satellite sat = null;
            if ( ! predictingTheSun )
            {
                // Create an orbit object using the TLE object.
                if ( ! SatCat.TleDict.ContainsKey(noradNum) )
                {
                    var msg = string.Format(@"TLE data not found for {0} - {1}", satName, noradNum);
                    Console_WriteLine("{0}", msg);
                    return;
                }

                var tle = SatCat.TleDict[noradNum];
                sat = new Satellite(tle, new Wgs84());
            }

            for ( var t = t1; t <= t2; t += TimeSpan.FromSeconds(nEventInterval) )
            {
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                    Thread.Yield();
                }

                var eci = predictingTheSun ? Solar.PositionEci(new Julian(t)) : sat.PositionEci(t);

                var topoTime = site.GetLookAngle(eci);

                if ( topoTime.ElevationDeg < 0.0 || topoTime.ElevationDeg >= 180.0 )
                    if ( predictingTheSun || ShowVisibleOnly )
                        continue;

                var eciUTC = eci.Date.ToTime().ToString("O") + "Z";
                var eciLocal = eci.Date.ToTime().ToLocalTime();
                var tmp = string.Format("{0}({1}),{2},{3}", eciUTC, eciLocal, topoTime.AzimuthDeg, topoTime.ElevationDeg);
                ListScript.Add(tmp);
            }
            ListScript.Add("# ===============================  END OF DATA  ===============================");

            ListScript.InsertRange(0, BuildHeaderInfo(si));
            var script = PassData.ComputeCrossLevel(ListScript);
            if ( script != null )
            {
                ListScript = script;
                ListScript.Insert(0, "# ===============================  START OF DATA  ===============================");
                ListScript.InsertRange(0, BuildHeaderInfo(si));
                ListScript.Add("# ===============================  END OF DATA  ===============================");
            }
        }

        #endregion END Predict Orbit

        #region GUI Handlers

        private void OnInterThreadMessagingCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var messages = sender as InterThreadMessaging;
        }

        private void OnInterThreadMessagingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var messages = sender as InterThreadMessaging;

            switch ( e.PropertyName )
            {
                case "StepProgressBar1":
                    StepProgress();
                    break;

                case "RESET_StepProgressBar1":
                    ResetProgress((int)messages.data);
                    break;

                case "Enqueue":
                {
                    var msg = InterThreadMessaging.GetMessage();
                    while ( ! string.IsNullOrEmpty(msg) )
                    {
                        Console_WriteLine(msg);
                        msg = InterThreadMessaging.GetMessage();
                    }

                    break;
                }

                //case "Dequeue":
                //{
                //    foreach (var message in messages)
                //    {
                //        var x = InterThreadMessaging.GetMessage();
                //        Console_WriteLine(message);
                //    }
                //    break;
                //}

                case "AddSatName":
                    var text = messages.Message;
                    AddToSatelliteNameListBox(text);
                    break;

                default:
                    var ctrl = Controls.Find(e.PropertyName, true).FirstOrDefault();
                    if ( ctrl != null )
                        ctrl.Text = messages.Message;
                    break;
            }
        }

        internal void Console_WriteLine(string format = "", params object[] arg0)
        {
            var text = string.Format(format, arg0);

            Console.WriteLine(text);

            Invoke(
                (MethodInvoker)( () =>
                {
                    //if ( ConsoleListBox.Items.Count > 500 )
                    //{
                    //    ConsoleListBox.Items.RemoveAt(0);
                    //}

                    ConsoleListBox.Items.Add(text);

                    ConsoleListBox.SelectedIndex = ConsoleListBox.Items.Count - 1;
                    ConsoleListBox.SelectedIndex = -1;
                    ConsoleListBox.Refresh();
                } ));
        }

        internal void Event_Writeline(string format, params object[] arg0)
        {
            var text = string.Format(format, arg0);

            Invoke(
                (MethodInvoker)( () =>
                {
                    listEventScript.Items.Add(text);

                    // scroll to bottom
                    listEventScript.SelectedIndex = listEventScript.Items.Count - 1;
                    listEventScript.SelectedIndex = -1;

                    listEventScript.Refresh();
                } ));
        }

        internal DialogResult Console_MessageBoxYesNo(string caption, string format = "", params object[] arg0)
        {
            var message = string.Format(format, arg0);

            Console.WriteLine(message);

            var result = DialogResult.Yes;

            if ( InvokeRequired )
            {
                Invoke(
                    (MethodInvoker)( () => { result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo); } ));
            }
            else
            {
                result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
            }

            return result;
        }

        internal void AddToSatelliteNameListBox(string text)
        {
            Invoke(
                (MethodInvoker)( () => { listBoxSatelliteName.Items.Add(text); } ));
        }

        internal void SetRunButtonBackground(Bitmap image)
        {
            Invoke(
                (MethodInvoker)( () => { btnRun.BackgroundImage = image; } ));
        }

        private delegate void StepProgressCallback();

        private delegate void ResetProgressCallback();

        internal void SetProgressText(string text)
        {
            Invoke(
                (MethodInvoker)( () => { progressBar1.Text = text; } ));
        }

        internal void UpdateProgress(int value)
        {
            Invoke(
                (MethodInvoker)( () => { progressBar1.Value = value; } ));
        }

        public void StepProgress()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if ( progressBar1.InvokeRequired )
            {
                Invoke(new StepProgressCallback(StepProgress));
            }
            else
            {
                progressBar1.PerformStep();
            }
        }

        public void ResetProgress(int maximum)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if ( progressBar1.InvokeRequired )
            {
                Invoke(new ResetProgressCallback(delegate { ResetProgress(maximum); }));
            }
            else
            {
                var maxSize = maximum;
                progressBar1.Maximum = maxSize;
                progressBar1.Step = 1; // progressBar1.Maximum / 100;
                progressBar1.Text = "";
                progressBar1.Visible = true;
                progressBar1.Minimum = 0;
                progressBar1.Value = 0;
                //progressBar1.MarqueeAnimationSpeed = 100;
                progressBar1.Style = ProgressBarStyle.Continuous;

                StepProgress();
            }
        }

        private void PopulateSiteFields(int index = 0)
        {
            listLocationName.Items.Clear();
            foreach ( var site in Sites )
            {
                listLocationName.Items.Add(site.Name);
            }

            if ( index < 0 )
                index = listLocationName.Items.Count - 1;
            listLocationName.SelectedIndex = index;
            tbLocationName.Text = listLocationName.Text;

            foreach ( var site in Sites.Where(site => site.Name.Equals(tbLocationName.Text)) )
            {
                LocationLatitude.Text = site.Geo.LatitudeDeg.ToString(CultureInfo.InvariantCulture);
                LocationLongitude.Text = site.Geo.LongitudeDeg.ToString(CultureInfo.InvariantCulture);
                LocationElevation.Text = site.Geo.Altitude.ToString(CultureInfo.InvariantCulture);

                ipAddressControl1.Text = CLI_IP = site.IP;
                tbMXPPORT.Text = CLI_PORT = site.PORT;
                tbMXPMASK.Text = CLI_MASK = site.MASK;
            }
        }

        private void tbStartTimeAzEl_Leave(object sender, EventArgs e)
        {
            SetSatData();
        }

        private void tbStopTimeAzEl_Leave(object sender, EventArgs e)
        {
            SetSatData();
        }

        private void tbTrackType_Leave(object sender, EventArgs e)
        {
            SetSatData();
        }

        private void tbTrackingFreq_Leave(object sender, EventArgs e)
        {
            SetSatData();
        }

        private void tbPolarity_Leave(object sender, EventArgs e)
        {
            SetSatData();
        }

        private void tbProgAutoTrack_Leave(object sender, EventArgs e)
        {
            SetSatData();
        }

        private void tbSearchType_Leave(object sender, EventArgs e)
        {
            SetSatData();
        }

        private void tbSampleRate_Leave(object sender, EventArgs e)
        {
            SetSatData();
        }

        private void tbMaxPathDev_Leave(object sender, EventArgs e)
        {
            SetSatData();
        }

        private void tbThreshold_Leave(object sender, EventArgs e)
        {
            SetSatData();
        }

        private void tbStartTimeAzEl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
                SetSatData();
        }

        private void tbStopTimeAzEl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
                SetSatData();
        }

        private void tbTrackType_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
                SetSatData();
        }

        private void tbTrackingFreq_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
                SetSatData();
        }

        private void tbPolarity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
                SetSatData();
        }

        private void tbProgAutoTrack_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
                SetSatData();
        }

        private void tbSearchType_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
                SetSatData();
        }

        private void tbSampleRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
                SetSatData();
        }

        private void tbMaxPathDev_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
                SetSatData();
        }

        private void tbThreshold_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( e.KeyChar == '\r' )
                SetSatData();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            SetAntennaType(TrackTYPE.Standard);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            SetAntennaType(TrackTYPE.Overhead);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            SetAntennaType(TrackTYPE.Tilt);
        }

        private bool SetAntennaTypeCanted;

        private void cbCanted_CheckedChanged(object sender, EventArgs e)
        {
            SetAntennaTypeCanted = cbCanted.Checked;
        }

        private void EventInterval_Leave(object sender, EventArgs e)
        {
            StopAction = true;
        }

        private void chkIgnoreStartTimer_CheckedChanged(object sender, EventArgs e)
        {
            IgnoreStartTimer = chkIgnoreStartTimer.Checked;
        }

        private void chkIgnoreIntervalTimer_CheckedChanged(object sender, EventArgs e)
        {
            IgnoreIntervalTimer = chkIgnoreIntervalTimer.Checked;
        }

        private void EventScriptSave_Click(object sender, EventArgs e)
        {
            if ( ListScript.Count < 1 )
            {
                Console_WriteLine("Build script first ...");
                return;
            }

            EventScriptFilename.Text = DATA_DIRECTORY + @"EventScript.csv";
            Console_WriteLine("Saving scripts as {0}", EventScriptFilename.Text);
            var l = new List<string>(ListScript);
            if (ListScript[0].StartsWith("#"))
            {
                l.RemoveRange(0, 4);
            }
            l.Insert(0, "Event Time, degAz, degEl, degCl");
            File.WriteAllLines(EventScriptFilename.Text, l);

        }

        private void btnRemoveFav_Click(object sender, EventArgs e)
        {
            for ( var index = 0; index < Fav_ListBox1.SelectedItems.Count; index++ )
            {
                Fav_ListBox1.Items.RemoveAt(index);
            }
        }

        private void dateTimePicker_UTC_Start_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker_UTC_Start.Value = dateTimePicker_LOCAL_Start.Value.ToUniversalTime();
        }

        private void dateTimePicker_UTC_End_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker_UTC_End.Value = dateTimePicker_LOCAL_End.Value.ToUniversalTime();
        }

        private void dateTimePicker_LOCAL_Start_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker_LOCAL_Start.Value = dateTimePicker_UTC_Start.Value.ToLocalTime();
        }

        private void dateTimePicker_LOCAL_End_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker_LOCAL_End.Value = dateTimePicker_UTC_End.Value.ToLocalTime();
        }

        private void cbVisibleOnly_CheckedChanged(object sender, EventArgs e)
        {
            ShowVisibleOnly = cbVisibleOnly.Checked;
        }

        private void ipAddressControl1_Leave(object sender, EventArgs e)
        {
            CLI_IP = ipAddressControl1.Text;
            CLI_PORT = tbMXPPORT.Text;
            CLI_MASK = tbMXPMASK.Text;

            CurrentSite.IP = CLI_IP;
            CurrentSite.PORT = CLI_PORT;
            CurrentSite.MASK = CLI_MASK;
        }

        private void txtWindowSize_TextChanged(object sender, EventArgs e)
        {
            WindowSize = int.Parse(txtWindowSize.Text);
        }

        private void txtWindowSize_Leave(object sender, EventArgs e)
        {
            WindowSize = int.Parse(txtWindowSize.Text);
        }

        private void txtWindowSize_Validated(object sender, EventArgs e)
        {
            WindowSize = int.Parse(txtWindowSize.Text);
        }

        private void txtWindowSize_Validating(object sender, CancelEventArgs e)
        {
            WindowSize = int.Parse(txtWindowSize.Text);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            ShowPasses();
        }

        private void SetupAntenna()
        {
            // Send Antenna Setup commands
            var setupCommands = new List < string >
            {
                "SET ANTENNA TRACKING OFF",
                "SET SYSTEM PRIMARY_REFLECTOR DISHSCAN MODE OFF",
                "SET SYSTEM TILT_MODE ON"
                //"SET ANTENNA AZ_TARGET 0",
                //"SET ANTENNA EL_TARGET 0",
                //"SET ANTENNA CL_TARGET 0"
            };

            foreach ( var setupCommand in setupCommands )
            {
                Send(setupCommand);
                Thread.Sleep(100);
            }

            // Send antenna to AOS
            Console_WriteLine("Sending antenna to initial AOS location");
            var cmd = Commands[0];
            SendCommandTriplet(cmd);
        }

        private void Run()
        {
            btnPause.Enabled = true;
            btnRun.Text = string.Empty;
            SetRunButtonBackground(Resources.RedStopButton);

            Cursor.Current = Cursors.WaitCursor;
            ResetCommandExpired();

            _run = true;
            Running = true;
            StopAction = false;

            SetupAntenna();

            for ( var i = 0; _run && Commands.Any(x => ! x.Expired); ++i )
            {
                while ( _Pause )
                {
                    Application.DoEvents();
                    Thread.Sleep(0);
                    Thread.Yield();
                }


                if ( ! _run || StopAction || ! Running )
                {
                    break;
                }

                // If all commands are expired, were done
                if ( Commands.All(x => x.Expired) )
                    break;

                StepProgress();

                var cmd = Commands[i];
                var now = DateTime.UtcNow.AddSeconds(-WindowSize);
                if ( ! IgnoreStartTimer )
                {
                    if ( cmd.When < now )
                    {
                        SetCommandExpired(i);

                        var az = cmd.TextCmdAz.Substring(cmd.TextCmdAz.LastIndexOf(' '));
                        var el = cmd.TextCmdEl.Substring(cmd.TextCmdEl.LastIndexOf(' '));
                        var cl = cmd.TextCmdCl.Substring(cmd.TextCmdCl.LastIndexOf(' '));

                        var msg = string.Format("Skip expired T:{0} {1}UTC ({2}LOCAL) AZ:{3} EL:{4} CL:{5}",
                            i, cmd.When.ToUniversalTime(), cmd.When, az, el, cl);

                        InterThreadMessaging.Instance.Message = msg;

                        continue;
                    }
                }

                SendCommandTriplet(cmd);

                SetCommandExpired(i);

                if ( IgnoreIntervalTimer ) continue;

                var msTillNextCommand = Commands[i + 1].When - cmd.When;
                if ( msTillNextCommand.TotalMilliseconds > 2 * 1000 )
                {
                    var text = string.Format("Next command in {0} seconds...", msTillNextCommand.TotalSeconds);
                    InterThreadMessaging.Instance.Message = text;
                }


                for ( var j = 0; j < msTillNextCommand.TotalMilliseconds; j++ )
                {
                    if ( ! _run || StopAction || ! Running )
                    {
                        break;
                    }

                    Application.DoEvents();
                    Thread.Sleep(1);
                    Thread.Yield();
                }
            }

            InterThreadMessaging.Instance.Message = "GO complete. All commands sent.";

            btnRun.Text = string.Empty;
            SetRunButtonBackground(Resources.GreenPlayButton);
            Cursor.Current = Cursors.Default;

            _run = false;
            Running = false;

            InterThreadMessaging.Instance.ResetProgress = 0;

            {
                Application.DoEvents();
                Thread.Sleep(0);
                Thread.Yield();
            }
        }

        private void LeadTime_ms_TextChanged(object sender, EventArgs e)
        {
            LeadtimeMs = int.Parse(( (TextBox)sender ).Text);
        }

        private void listboxSatellite_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( InitComplete )
            {
                var selectedSat = listBoxSatelliteName.SelectedItem.ToString();
                listBoxSatelliteName.FindString(selectedSat);
                listBoxSatelliteName.Text = selectedSat;

                // Set the satellite info and TLE
                if ( Sites.Count <= 0 ) return;

                var satName = ( (ComboBox)sender ).Text;
                ShowPasses(satName);
            }
        }

        private void boxSatellite_Name_Leave(object sender, EventArgs e)
        {
            var idx = listBoxSatelliteName.FindString(listBoxSatelliteName.Text);
            if ( idx == -1 )
            {
                // add
                SatelliteCatalog.FormatedSatNames.Insert(0, listBoxSatelliteName.Text);
                listBoxSatelliteName.SelectedIndex = listBoxSatelliteName.FindString(listBoxSatelliteName.Text);
            }
            else
            {
                listBoxSatelliteName.SelectedIndex = idx;
            }

            // Set the satellite info and TLE
            btnGetSatInfo_Click(sender, e);
        }

        private void btnGetSatInfo_Click(object sender, EventArgs e)
        {
            if ( string.IsNullOrEmpty(listBoxSatelliteName.Text) )
                return;

            GenerateScriptButton.Enabled = false;

            var noradNum = GetSatInfo();

            if ( noradNum > 0 && SatCat.TleDict.ContainsKey(noradNum) )
            {
                SetTleInfo(SatCat.TleDict[noradNum]);
                GenerateScriptButton.Enabled = true;
            }
            else
            {
                ResetTleInfo();
            }
        }

        private void listBoxFavorites_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( ! InitComplete )
                return;

            listEventScript.Items.Clear();

            ShowPasses(Fav_ListBox1.Text);
            InterThreadMessaging.Instance.Message = "Pass selection required";
            btnBuildScript.Enabled = false;
        }

        private void listBoxPasses_SelectedIndexChanged(object sender, EventArgs e)
        {
            listEventScript.Items.Clear();
            btnBuildScript.Enabled = true;
        }

        private void boxPasses_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            listEventScript.Items.Clear();
            btnBuildScript_Click(sender, e);
        }

        private void listLocationName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            foreach ( var site in Sites.Where(site => site.Name == comboBox.Text) )
            {
                LocationLatitude.Text = site.Geo.LatitudeDeg.ToString(CultureInfo.InvariantCulture);
                LocationLongitude.Text = site.Geo.LongitudeDeg.ToString(CultureInfo.InvariantCulture);
                LocationElevation.Text = site.Geo.Altitude.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void BTNSiteNew_Click(object sender, EventArgs e)
        {
            // Clear info and populate with defaults

            SitesLoad();

            Sites.Add(new SiteInfo(new Site(0.0, 0.0, 0.0, new Wgs84(), "Site Name"), "0.0.0.0", "0", "255.255.255.0"));

            PopulateSiteFields(-1);
        }

        private void btnSiteSave_Click(object sender, EventArgs e)
        {
            SitesSave();
        }

        private void BTNSiteRemove_Click(object sender, EventArgs e)
        {
            var x = listLocationName.SelectedItem as string;

            foreach ( var idx in from site in Sites where site.Name == x select Sites.IndexOf(site) )
            {
                Sites.RemoveAt(idx);
                break;
            }

            PopulateSiteFields();
        }

        private void btnBuildScript_Click(object sender, EventArgs e)
        {
            if ( btnBuildScript.Text == "Build Script" )
            {
                btnPause.Enabled = true;

                btnBuildScript.Text = "Stop Build";
                btnBuildScript.BackgroundImage = Resources.RedStopButton;

                StopAction = true;

                GenSchedule();

                Commands = ConvertScriptToCommands(ListScript);

                ShowScript(ListScript);

                btnBuildScript.Text = "Build Script";
                btnBuildScript.BackgroundImage = Resources.BlueBuildButton;

                btnBuildScript.Enabled = true;
            }
            else
            {
                StopAction = true;
            }
        }

        private readonly bool _Pause = false;

        private void btnPause_Click(object sender, EventArgs e)
        {
            if ( ! _Pause )
            {
//                SetRunButtonBackground(Resources.GreenPlayButton);
            }

            //  _Pause = !_Pause;
        }

        public bool Running { get; private set; }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if ( ! Running )
            {
                _run = false;

                if ( Commands == null || Commands.Count < 1 )
                {
                    return;
                }

                if ( ! SatelliteCatalog.SatCatLoaded )
                {
                    Console_WriteLine("Satellite Catalog not loaded");
                    return;
                }

                ThreadRun = new Thread(Run);
                _run = true;

                btnRun.Text = string.Empty;

                progressBar1.Maximum = Commands.Count;
                progressBar1.Value = 0;
                progressBar1.Step = 1;


                ThreadRun.Start();

                while ( _run )
                {
                    Application.DoEvents();
                    Thread.Sleep(0);
                    Thread.Yield();
                }
            }
            else
            {
                StopAction = true;
                Running = false;
            }
        }

        #endregion GUI Handlers

        #region Fields/Properties

        public const string DATA_DIRECTORY = @".\data\";

        public List < string > ListScript { get; set; }
        public List < SiteInfo > Sites { get; set; }
        private bool _run;
        public static IO Io { get; private set; }
        public List < Command > Commands { get; private set; }
        public double DelaySec { get; private set; }
        public bool IgnoreStartTimer { get; private set; }
        public bool IgnoreIntervalTimer { get; private set; }
        public Thread ThreadRun { get; private set; }
        public SatelliteCatalog SatCat { get; set; }
        public bool InitComplete { get; private set; }
        public bool StopAction { get; private set; }
        private static string CLI_IP { get; set; }
        private static string CLI_PORT { get; set; }
        private static string CLI_MASK { get; set; }
        public static string InterfaceMode { get; set; }
        public static int LeadtimeMs { get; set; }
        public int WindowSize { get; set; }
        public bool ShowVisibleOnly { get; set; }

        private void InitilizePropertyDefaults()
        {
            ListScript = new List < string >();

            Sites = new List < SiteInfo >();

            CLI_IP = "69.42.29.222"; // Frank I

            CLI_PORT = "2003";

            CLI_MASK = "255.255.255.0";

            InterfaceMode = "CLI";

            LeadtimeMs = 500; // 1/2 sec lead time

            WindowSize = 60;

            ShowVisibleOnly = true;
        }

        #endregion Fields/Properties
    }
}