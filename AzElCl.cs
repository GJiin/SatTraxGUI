///////////////////////////////////////////////////////////////
// Form1.cs - SatTrax
// Takes a list of AZ/EL and computes AZ/EL/CL
//
// The program obtains a catalog of satellites and TLE data for a set of satellites.
// It predicts and produces a pass ephemeris script and connects to an MXP with a command set

// Version .01
// Author Michael Ketcham / Pete Blaney
//
// 04/04/2016 - Initial - Overhead
// 04/05/2016 - Initial - Tilt
//
// Copyright Sea Tel @ 2016 doing business as Cobham SATCOM
//////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

internal static class MathExtentions
{
    public const double TwoPI = 2 * Math.PI;
    public const double RadsPerDegree = Math.PI / 180.0;
    public const double DegreesPerRad = 180.0 / Math.PI;
    public const double Ae = 1.0;
    public const double Au = 149597870.0; // Astronomical unit (km) (IAU 76)
    public const double MinPerDay = 1440.0; // Minutes per day (solar)
    public const double SecPerDay = 86400.0; // Seconds per day (solar)
    public const double OmegaE = 1.00273790934; // Earth rotation per solar day

    public static double DegreeToRadian(this double angle)
    {
        return Math.PI * ( angle / 180.0 );
    }

    public static double RadianToDegree(this double angle)
    {
        return angle * ( 180.0 / Math.PI );
    }

    // check range of angle in Radians
    public static double CheckRange(this double angle)
    {
        if ( angle < 0 )

            angle += TwoPI;

        else if ( angle > TwoPI )

            angle -= TwoPI;

        return angle;
    }

    public static double CheckDiff(this double angle)
    {
        if ( angle > Math.PI )

            angle -= TwoPI;

        else if ( angle < -Math.PI )

            angle += TwoPI;

        return angle;
    }
}

namespace SatTraxGUI
{
    public class PassData
    {
        public const string Version = "0.1";

        /// <summary>
        ///     1 - Calls LoadSatTrax
        ///     2 - Checks for a Max El greater that minimum (only generate CL for passes greater than 78 degrees)
        ///     3 - sets pass level antenna constant factors (TILT or OVERHEAD)
        ///     4 - Initializes "PREV" data
        ///     5 - Runs each row of the pass
        ///     6 - Creates a list using new AZ/EL/CL
        /// </summary>
        /// <param name="listScriptAzEl"></param>
        /// <returns></returns>
        /// START HERE
        public static List < string > ComputeCrossLevel(List < string > listScriptAzEl)
        {
            ///////////////////////////////////////////////////////////////////
            // Load data and precompute SIN/COS for each track Az/EL
            //
            // Each row will have AZ/EL in radians as well as Az/EL deltas, COS/SIN
            //
            var pass = LoadSatTrax(listScriptAzEl.ToArray());


            /////////////////////////////////////////////////////////////////////
            // don't bother if less than 5 rows or under 78 degrees max elevation
            //
            if ( pass.MaxEl < MinEL || pass.listAzelcl.Count < 1 )
            {
                return listScriptAzEl;
            }

            //Determine direction using AZ at AOS and Az at MAX el
            var motion = ( pass.rad_AzAtMaxEl - pass.listAzelcl[0].trackAz ).CheckRange();
            Direction = motion < 0 ? -1 : 1;

            /////////////////////////////////////////////////////////////////////
            // Each antenna type (overhead, tilt) has slightly different factoring data  and row zero setup
            //
            switch ( Form1.TrackType )
            {
                case Form1.TrackTYPE.Standard:

                    #region Standard

                    var std_path = @"EventScript.update.PostPredict.Standard.CSV";
                    AzElCl.WriteAzElCl(pass.listAzelcl, Form1.DATA_DIRECTORY + std_path);

                    return listScriptAzEl;

                    #endregion Standard

                    break;

                case Form1.TrackTYPE.Overhead:

                    #region Overhead

                    AzGain = 0.9;
                    ElGain = 0.9;
                    ClGain = 0.9;

                    // correct current track data using previous pedestal data...
                    pass.listAzelcl[0].pedCl = ( Math.PI / 2 - pass.MaxEl ) * Direction;
                    pass.listAzelcl[0].pedAz = pass.listAzelcl[0].trackAz;
                    pass.listAzelcl[0].pedEl = pass.listAzelcl[0].trackEl;

                    // initialize pedestal Sin and Cos (PedSinCos)
                    pass.listAzelcl[0].SetPedSinCos();

                    #endregion Overhead

                    break;

                case Form1.TrackTYPE.Tilt:

                    #region Tilt

                    // All values in radians

                    AzGain = 0;
                    ElGain = 0.7;
                    ClGain = 0.7;

                    AzElCl.State = 0;

                    // init pedestal X/Y/Z
                    pass.listAzelcl[0].pedCl = 0;
                    pass.listAzelcl[0].pedAz = pass.listAzelcl[0].trackAz;
                    pass.listAzelcl[0].pedEl = pass.listAzelcl[0].trackEl;

                    // initialize pedestal Sin and Cos
                    pass.listAzelcl[0].SetPedSinCos();

                    #endregion Tilt

                    break;

                default:
                    Console.WriteLine("Invalid Antenna Type {0}", Form1.TrackType);
                    break;
            }


            /////////////////////
            //  THE MAIN LOOP  //
            /////////////////////

            for ( var i = 0; i < pass.listAzelcl.Count - 1; i++ )
            {
                // Start with row 1, passing it the previous row (first passed row is row 0)

                pass.listAzelcl[i + 1].ProcessRow(pass.listAzelcl[i]);
            }


            //////////////////////////////////////////////////////////////////////
            // Create the new list of Az/EL/CL
            //

            #region Debug Print

            // Mostly for debugging... write everything to a file
            string path;
            switch ( Form1.TrackType )
            {
                case Form1.TrackTYPE.Overhead:
                    path = @"EventScript.update.PostPredict.Overhead.CSV";
                    break;
                case Form1.TrackTYPE.Tilt:
                    path = @"EventScript.update.PostPredict.Tilt.CSV";
                    break;
                case Form1.TrackTYPE.Standard:
                default:
                    path = @"EventScript.update.PostPredict.Standard.CSV";
                    break;
            }

            AzElCl.WriteAzElCl(pass.listAzelcl, Form1.DATA_DIRECTORY + path);

            #endregion Debug Print

            ////////////////////////////////
            // Created the list of events.
            ////////////////////////////////

            var j = 0;
            var l = pass.listAzelcl.Select(
                target => string.Format("{4} {0},{1,3:F12},{2,3:F12},{3,3:F12}",
                    target.eciDate.ToString("O"), target.pedAz.RadianToDegree(), target.pedEl.DegreeToRadian(), target.pedCl.RadianToDegree(), j++)).ToList();

            return l;
        }

        public static PassData LoadSatTrax(string path)
        {
            return LoadSatTrax(File.ReadAllLines(path));
        }

        /// <summary>
        ///     Does the actual loading of SatTrax data
        ///     1 - Load Az/EL, MAX EL - as degrees
        ///     2 - Covert to radians
        ///     3 - Compute Cos and Sin of Az and El
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static PassData LoadSatTrax(string[] lines)
        {
            var pass = new PassData(new List < AzElCl >(lines.Length));

            var sepChar = new[] {','};

            var maxEl = 0.0;
            var predictTheSun = false;
            var ELAtAOS = -1.0;
            var ELAtLOS = 0.0;

            foreach ( var line in lines ) //Parallel.ForEach(lines, line =>
            {
                if ( line.StartsWith("#") )
                {
                    //# Events:32, Period:3600, Pass Length:0 Pass Info:2016-04-06 21:36:54 UTC AOS [AZ 180] 21:43:47 UTC MAX [EL 48.00318072] 21:50:44 UTC LOS [AZ 341]

                    if ( line.StartsWith("# Sat:The Sun, ") )
                    {
                        predictTheSun = true;
                    }

                    if ( ! line.StartsWith("# Events:") ) continue;

                    // Number of events
                    var token = "Events:";
                    var start = line.IndexOf(token) + token.Length;
                    var len = line.IndexOf(',', start) - start;
                    var text = line.Substring(start, len);
                    numEvents = int.Parse(text);

                    // Event period
                    token = "Period:";
                    start = line.IndexOf(token) + token.Length;
                    len = line.IndexOf(',', start) - start;
                    text = line.Substring(start, len);
                    sec_EventPeriod = double.Parse(text);

                    // Max
                    if ( ! predictTheSun )
                    {
                        // AZ at MAX
                        token = "MAX [AZ ";
                        start = line.IndexOf(token) + token.Length;
                        len = line.IndexOf(" EL", start) - start;
                        text = line.Substring(start, len);
                        double.TryParse(text, out pass.deg_AzAtMaxEl);
                        Console.WriteLine("Az at MaxEl (degrees) {0}", pass.deg_AzAtMaxEl);
                        pass.rad_AzAtMaxEl = double.Parse(text).DegreeToRadian();

                        // MAX EL
                        token = " EL ";
                        var start1 = line.IndexOf(token, start) + token.Length;
                        len = line.IndexOf("]", start1) - start1;
                        text = line.Substring(start1, len);
                        pass.deg_MaxEl = double.Parse(text);
                        Console.WriteLine("MaxEl (degrees) {0}", pass.deg_MaxEl);
                        pass.MaxEl = double.Parse(text).DegreeToRadian();
                    }

                    continue;
                }

                var l = line.Split(sepChar); //dateTime, AZ, EL, CL

                // DateTime
                var x1 = l[0];
                var utc = x1.Substring(0, x1.IndexOf('('));
                var dt = DateTime.Parse(utc);
                var date = dt; //eciDateTime

                // CL
                var cl = l.Length < 4 ? 0 : double.Parse(l[3]).DegreeToRadian();

                // AZ
                var az = double.Parse(l[1]).DegreeToRadian();

                // EL
                var el = double.Parse(l[2]).DegreeToRadian();

                if ( predictTheSun )
                {
                    // calculate AOS/MAX/LOS for Sun
                    if ( el > maxEl ) maxEl = el; // max el
                    if ( ELAtAOS < 0.0 ) ELAtAOS = el; // first EL
                    ELAtLOS = el; // last EL
                }

                // name
                var name = string.Format("{0}Z,{1,3:F12},{2,3:F12}", dt.ToString("O"), az, el);

                var azelcl = new AzElCl(name, date, az, el, cl);
                pass.listAzelcl.Add(azelcl);
            }

            if ( predictTheSun )
            {
                pass.MaxEl = maxEl;
                pass.AOS = ELAtAOS;
                pass.LOS = ELAtLOS;
            }

            return pass;
        }

        #region Fields

        public static readonly double MinEL = 78.0.DegreeToRadian();
        public static int Direction;
        public static int numEvents;
        public static double sec_EventPeriod;
        public List < AzElCl > listAzelcl;
        public double MaxEl;
        public double deg_MaxEl; //  just used for debugging
        public double rad_AzAtMaxEl;
        public double deg_AzAtMaxEl;
        public double AOS;
        public double LOS;
        public string Name;
        public static double AzGain;
        public static double ClGain;
        public static double ElGain;

        #endregion Fields

        #region constructors

        public PassData()
        {
            listAzelcl = new List < AzElCl >();
        }

        public PassData(int capacity)
        {
            listAzelcl = new List < AzElCl >(capacity);
        }

        public PassData(IEnumerable < AzElCl > listAzElCls)
        {
            listAzelcl = new List < AzElCl >(listAzElCls);
        }

        #endregion constructors

        #region Helpers

        public static void Serialize(List < AzElCl > aec, string path)
        {
            var writer = new XmlSerializer(typeof ( List < AzElCl > ));
            var file = File.Create(path);
            //            writer.Serialize(file, aec);
            file.Close();
        }

        public static List < AzElCl > Deserialize(string path)
        {
            var reader = new XmlSerializer(typeof ( AzElCl[] ));
            var file = new StreamReader(path);
            //   var aec = (List<AzElCl>)reader.Deserialize(file);
            file.Close();
            return null; // aec;
        }

        #endregion Helpers
    }

    // All values are in Radians

    [Serializable]
    public class AzElCl
    {
        public const string Version = PassData.Version;

        public static int State;

        private static double _lastPrevAzErr;

        public AzElCl()
        {
        }

        public AzElCl(string name, DateTime eciDateTime, double az, double el, double cl)
        {
            this.name = name;
            eciDate = eciDateTime;

            // Radians
            trackAz = az;
            trackEl = el;
            trackCl = cl;

            trackCosAz = Math.Cos(trackAz);
            tractSinAz = Math.Sin(trackAz);
            trackCosEl = Math.Cos(trackEl);
            trackSinEl = Math.Sin(trackEl);

            trackX = trackCosAz * trackCosEl;
            trackY = tractSinAz * trackCosEl;
            trackZ = -trackSinEl;
        }

        /////////////////////////////////////////////////////////////////////////////

        // Called ONCE per line
        public void ProcessRow(AzElCl prev)
        {
            // update pedestal AZ/EL/CL using previous row's track Az/EL/CL and updates

            DeltaXYZ(prev); //  DeltaXYZ = current_row_XYZ  - (previous_row_XYZ)

            switch ( Form1.TrackType )
            {
                case Form1.TrackTYPE.Standard:

                    #region Standard

                    #endregion Standard

                    break;

                case Form1.TrackTYPE.Overhead:

                    #region Overhead

                    PredictOverheadMotion();

                    #endregion Overhead

                    break;

                case Form1.TrackTYPE.Tilt:

                    #region Tilt

                    PredictAzVelocity(prev); // state machine stuff...

                    #endregion Tilt

                    break;

                default:
                    Console.WriteLine("Invalid Track Type {0}", Form1.TrackType);
                    break;
            }

            const int iterations = 5;
            for ( var i = 0; i < iterations; i++ )
            {
                UpdatePedestal(); // Fine Tune El and Cl positions to maintain the same pointing vector
            }
        }

        private void PredictAzVelocity(AzElCl prev)
        {
            var paramAzVelLimit = 1.5.DegreeToRadian() * PassData.sec_EventPeriod;
            var paramAzAcc = .03.DegreeToRadian() * Square(PassData.sec_EventPeriod);
            var paramElStart = 60.0.DegreeToRadian();

            trackAzVel = ( trackAz - prev.trackAz ).CheckDiff();

            switch ( State )
            {
                case 0:

                    // follow az/el until start elevation
                    pedEl = trackEl;
                    azVel = trackAzVel;

                    if ( trackEl > paramElStart )
                    {
                        // Acceleration per update
                        azAcc = paramAzAcc * PassData.Direction;

                        azVel = prev.trackAzVel + azAcc;

                        State = State + 1;
                    }

                    pedAz = ( prev.pedAz + azVel ).CheckRange();

                    break;

                case 1:

                    azAcc = prev.azAcc;

                    azVel = prev.azVel + azAcc;

                    // update current velocity with program acceleration
                    if ( Math.Abs(azVel) >= paramAzVelLimit )
                    {
                        // back off Az Velocity if we went too far
                        //AzVel = paramAzVelLimit * PassData.Direction;
                        azAcc = 0;
                        State = State + 1;
                    }

                    pedAz = ( prev.pedAz + azVel ).CheckRange();

                    break;

                case 2:

                    azVel = prev.azVel;

                    // update position with current velocity
                    pedAz = ( prev.pedAz + azVel ).CheckRange();

                    if ( PassData.Direction == 1 )
                    {
                        // going UP
                        if ( prev.azErr < 0 ) ++State;
                    }
                    else
                    {
                        // going down
                        if ( prev.azErr > 0 ) ++State;
                    }

                    break;

                case 3:

                    azVel = prev.azVel;

                    if ( Math.Abs(prev.azErr) < Math.Abs(_lastPrevAzErr) )
                    {
                        azAcc = Square(prev.azVel - trackAzVel) / prev.azErr;
                        azVel += azAcc; // update Az Velocity

                        State = State + 1;
                    }

                    // update position with current velocity
                    pedAz = ( prev.pedAz + azVel ).CheckRange();

                    break;

                case 4:

                    azAcc = Square( prev.azVel - trackAzVel ) / prev.azErr;

                    if ( PassData.Direction == 1 )
                    {
                        // don't speed up before slowing down
                        if ( azAcc > 0 ) azAcc = 0;
                    }
                    else
                    {
                        // don't speed up before slowing down
                        if ( azAcc < 0 ) azAcc = 0;
                    }

                    azVel = prev.azVel + azAcc; // update Az Velocity

                    pedAz = ( prev.pedAz + azVel ).CheckRange();

                    break;
            }

            _lastPrevAzErr = prev.azErr;

            azErr = ( pedAz - trackAz ).CheckDiff();

            deltaUD1 = -prev.pedSinEl * ( prev.pedCosAz * deltaX + prev.pedSinAz + deltaY );
            deltaUD2 =-prev.pedCosEl * deltaZ;
            UD3 = azErr * prev.pedCosEl;
            pedEl = prev.pedEl + deltaUD1 + deltaUD2 + UD3 * 0.02 * PassData.Direction;

            deltaLR = -( UD3 - prev.UD3 ) / prev.pedSinEl;
            pedCl = prev.pedCl + deltaLR;
        }

        private void UpdatePedestal() // (Update_Pedestal)
        {
            SetPedSinCos(); // set ped Sin/Cos

            var y2 = pedSinCl * pedSinEl;
            pedX = pedCosAz * pedCosEl - pedSinAz * y2;
            pedY = pedSinAz * pedCosEl + pedCosAz * y2;
            pedZ = -pedCosCl * pedSinEl;

            errX = trackX - pedX;
            errY = trackY - pedY;
            errZ = trackZ - pedZ;

            errUpDn = -pedCosEl * errZ - pedSinEl * ( pedCosAz * errX + pedSinAz * errY );
            errLR = pedCosAz * errY - pedSinAz * errX;

            // Update pedestal Az,El,Cl

            pedAz += errLR * pedCosEl * PassData.AzGain; // AZ correction
            pedEl += errUpDn * PassData.ElGain; // EL correction
            pedCl += errLR * pedSinEl * PassData.ClGain; // CL correction
        }

        #region Fields

        // utility
        private double deltaX;
        private double deltaY;
        private double deltaZ;

        private double deltaUD1;
        private double deltaUD2;

        private double deltaLR;
        private double UD3;

        internal DateTime eciDate;

        private double errLR;
        private double errUpDn;
        private double errX;
        private double errY;
        private double errZ;
//        private int it;
        private string name;
//        private PassData passData;

        // pedestal
        internal double pedAz;
        internal double pedCl;

        private double pedCosAz;
        private double pedCosCl;
        private double pedCosEl;
        internal double pedEl;
        private double pedSinAz;
        private double pedSinCl;
        private double pedSinEl;

        private double pedX;
        private double pedY;
        private double pedZ;

        internal double trackAz;
        private readonly double trackCl;
        private readonly double trackCosAz;
        private readonly double trackCosEl;

        //private double trackDeltaX;
//        private double trackDeltaY;
//        private double trackDeltaZ;
        internal double trackEl;
//        private double trackErrLR;

//        private double trackErrUD;

//        private double trackErrX;
//        private double trackErrY;
//        private double trackErrZ;

        private readonly double trackSinEl;

        private readonly double trackX;
        private readonly double trackY;
        private readonly double trackZ;
        private readonly double tractSinAz;

        private double trackAzVel;

        private double azVel;
        private double azAcc;
        private double azErr;

        //private static double paramAzVelLimit = 1.8.DegreeToRadian() * PassData.sec_EventPeriod;
        //private static double paramAzAcc = .04.DegreeToRadian() * PassData.sec_EventPeriod;
        //private static double paramElStart = 45.0.DegreeToRadian();

        //private static readonly double LimitAzVel = .9.DegreeToRadian() * paramAzVelLimit * PassData.sec_EventPeriod;

        #endregion Fields

        #region Helpers

        private void PredictOverheadMotion()
        {
            deltaUD1 = -pedSinEl * ( pedCosAz * deltaX + pedSinAz * deltaY ) - pedCosEl * deltaZ;
            pedEl = pedEl + deltaUD1 * 0.99999;
        }

        public void DeltaXYZ(AzElCl prev)
        {
            deltaX = trackX - prev.trackX;
            deltaY = trackY - prev.trackY;
            deltaZ = trackZ - prev.trackZ;
        }

        public void SetPedSinCos()
        {
            pedCosAz = Math.Cos(pedAz);
            pedSinAz = Math.Sin(pedAz);

            pedCosEl = Math.Cos(pedEl);
            pedSinEl = Math.Sin(pedEl);

            pedCosCl = Math.Cos(pedCl);
            pedSinCl = Math.Sin(pedCl);
        }

        public static double Square(double x)
        {
            return x * x;
        }

        public static void WriteAzElCl(List < AzElCl > listAzelcl, string path)
        {
            var l = new List < string >(listAzelcl.Count);

            // Header
            var header = string.Format(
                "tdegAz, tdegEl, tdegCl, " +
                "tAZ, tEL, tCL, " +
                "tCosAz, tSinAz, tCosEl, tSinEl, " +
                "tX, tY, tZ, " +
                "pdegAZ, pdegEL, pdegCL, " +
                "pAZ, pEL, pCL, " +
                "pCosAz, pSinAz, pCosEl, pSinEl, " +
                "pX, pY, pZ, " +
                "deltaX, deltaY, deltaZ, " +
                "deltaUD1, deltaUD2, " +
                "errX, errY, errZ, " +
                "errUpDn, errLR");

            l.Add(header);

            l.AddRange(from azElCl in listAzelcl
                select
                    string.Format(
                        //track
                        "{30,3:F12},{31,3:F12},{32,3:F12}," +
                        "{00,3:F12},{01,3:F12},{02,3:F12}," +
                        "{03,3:F12},{04,3:F12},{05,3:F12}, {06,3:F12}," +
                        "{07,3:F12},{08,3:F12},{09,3:F12}," +

                        //ped
                        "{10,3:F12},{11,3:F12},{12,3:F12}," +
                        "{33,3:F12},{34,3:F12},{35,3:F12}," +
                        "{13,3:F12},{14,3:F12},{15,3:F12}, {16,3:F12}," +
                        "{17,3:F12},{18,3:F12},{19,3:F12}," +
                        "{20,3:F12},{21,3:F12},{22,3:F12}," +
                        "{23,3:F12},{24,3:F12}," +
                        "{25,3:F12},{26,3:F12},{27,3:F12}," +
                        "{28,3:F12},{29,3:F12}",
                        azElCl.trackAz, azElCl.trackEl, azElCl.trackCl,
                        azElCl.trackCosAz, azElCl.tractSinAz, azElCl.trackCosEl, azElCl.trackSinEl,
                        azElCl.trackX, azElCl.trackY, azElCl.trackZ,
                        azElCl.pedAz.RadianToDegree(), azElCl.pedEl.RadianToDegree(), azElCl.pedCl.RadianToDegree(),
                        azElCl.pedCosAz, azElCl.pedSinAz, azElCl.pedCosEl, azElCl.pedSinEl,
                        azElCl.pedX, azElCl.pedY, azElCl.pedZ,
                        azElCl.deltaX, azElCl.deltaY, azElCl.deltaZ,
                        azElCl.deltaUD1, azElCl.deltaUD2,
                        azElCl.errX, azElCl.errY, azElCl.errZ,
                        azElCl.errUpDn, azElCl.errLR,
                        azElCl.trackAz.RadianToDegree(), azElCl.trackEl.RadianToDegree(), azElCl.trackCl.RadianToDegree(),
                        azElCl.pedAz, azElCl.pedEl, azElCl.pedCl
                        )
                );

            File.WriteAllLines(path, l);
        }

        #endregion Helpers
    }
}