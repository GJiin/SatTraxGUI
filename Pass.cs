///////////////////////////////////////////////////////////////
// Pass.cs - SatTrax
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
using Zeptomoby.OrbitTools;
using Zeptomoby.OrbitTools.Pro;
using Zeptomoby.OrbitTools.Track;

namespace SatTraxGUI
{
    internal class PassInfo
    {
        public string Local;
        public string UTC;

        public PassInfo(string utc, string local)
        {
            UTC = utc;
            Local = local;
        }
    }

    internal class Pass
    {
        public Pass(Tle tle, Site site, DateTime t1, DateTime t2)
        {
            Predict(tle, site, t1, t2);
        }


        /// <summary>
        ///     Calculate satellite pass data for a *** local *** horizon.
        ///     The data includes rise time (AOS), set time (LOS), and optionally, the maximum
        ///     elevation (MAX) above the horizon.
        ///     Acquisition of Signal (AOS) - the time when the satellite rises above the horizon.
        ///     Loss of Signal(AOS) - the time when the satellite sets below the horizon.
        ///     Time of Closest Approach(TCA) - the best time for communication, when Doppler shift is zero and path loss is
        ///     minimized.
        ///     For circular orbits, TCA is usually near the halfway point, and corresponds to the maximum antenna
        ///     elevation(highest point in the sky).
        ///     For elliptical orbits, it could occur at AOS or LOS. WinOrbit actually reports the maximum antenna elevation in
        ///     both cases.
        /// </summary>
        /// <param name="tle">The satellite two-line-entry data</param>
        /// <param name="site">The Earth site for which events are calculated.</param>
        /// <param name="t1">The start time (UTC).</param>
        /// <param name="t2">The stop time (UTC).</param>
        /// <returns></returns>
        /// <returns>A list of PassData strings containing the AOS/MAX/LOS event data.</returns>
        /// <remarks>
        ///     A satellite rising above the horizon, then setting below the horizon, is
        ///     a single pass. This method will calculate all passes of a satellite over
        ///     a given local horizon for the given time period. The method will also
        ///     optionally calculate the maximum elevation above the local horizon that
        ///     the satellite obtains during each pass.
        /// </remarks>
        public static List<PassInfo> Predict(Tle tle, Site site, DateTime t1, DateTime t2)
        {
            var listPass = new List<PassInfo>();

            var sat = new Satellite(tle, new Wgs84());

            // CalcPassData() returns a list of PassData objects. Each object in
            // the list represents a single pass of the satellite.
            // Note: CalcPassData() can throw PropagationException exceptions; for
            // brevity error handling is not shown here.
            var passList = site.CalcPassData(sat, t1, t2, true);

            // Show the calculated passes
            foreach (var pass in passList)
            {
                // Each call to ToStringLocal() prints:
                //    The time that the satellite rises above the horizon (AOS),
                //       and the degrees azimuth;
                //    The time that the satellite obtains its maximum elevation
                //       (MAX) above the horizon, the degrees azimuth, and degrees
                //        elevation;
                //    The time that the satellite sets below the horizon (LOS),
                //       and the degrees azimuth.

                //Console.WriteLine(pass.ToStringLocal(sat.Orbit, "PST", "PDT"));
                //listPass.Add(pass.ToStringLocal(sat.Orbit, "PST", "PDT"));

                var passUTC = pass.ToStringUtc(sat.Orbit);
                var passLOCAL = pass.ToStringLocal(sat.Orbit);

                Console.WriteLine(passUTC);
                Console.WriteLine(passLOCAL);
                Console.WriteLine("-----------------");

                listPass.Add(new PassInfo(passUTC, passLOCAL));
            }

            return listPass;
        }
    }
}