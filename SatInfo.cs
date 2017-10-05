///////////////////////////////////////////////////////////////
// SatInfo.cs - SatTrax
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

//////////////////////////////////////////////////////////////////////////
//
//  SATCAT Format Documentation
//
//  ------------------------------------------------------------------------------------------------------------------------------------
//  Column
//  000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000111111111111111111111111111111111
//  000000000111111111122222222223333333333444444444455555555556666666666777777777788888888889999999999000000000011111111112222222222333
//  123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012
//  ------------------------------------------------------------------------------------------------------------------------------------
//  yyyy-nnnaaa nnnnn M* O aaaaaaaaaaaaaaaaaaaaaaaa aaaaa  yyyy-mm-dd aaaaa  yyyy-mm-dd nnnnn.n  nnn.n nnnnnn  nnnnnn nnn.nnnn aaa
//  ------------------------------------------------------------------------------------------------------------------------------------
//
//  Columns Description
//  001-011	International Designator
//              Launch Year (001-004)
//              Launch of the Year(006-008)
//              Piece of the Launch(009-011)
//  014-018 NORAD Catalog Number
//  020-020	Multiple Name Flag("M" if multiple names exist; alternate names found in satcat-annex.txt (https://celestrak.com/pub/satcat-annex.txt))
//  021-021	Payload Flag("*" if payload; blank otherwise)
//  022-022	Operational Status Code (https://celestrak.com/satcat/status.asp)
//          Note: The "U" code indicates that the satellite is considered operational according to the Union of Concerned Scientists(UCS) Satellite Database.
//024-047	Satellite Name(s)
//              Standard Nomenclature
//                  R/B(1) = Rocket body, first stage
//                  R/B(2) = Rocket body, second stage
//                  DEB = Debris
//                  PLAT = Platform
//                  Items in parentheses are alternate names
//                  Items in brackets indicate type of object
//                  (e.g., BREEZE-M DEB[TANK] = tank)
//                  An ampersand(&) indicates two or more objects are attached
//  050-054	Source or Ownership (https://celestrak.com/satcat/sources.asp)
//  057-066	Launch Date[year - month - day]
//  069-073	Launch Site
//  076-085	Decay Date, if applicable[year - month - day]
//  088-094	Orbital period[minutes]
//  097-101	Inclination[degrees]
//  104-109	Apogee Altitude[kilometers]
//  112-117	Perigee Altitude[kilometers]
//  120-127	Radar Cross Section[meters2]; N/A if no data available
//  130-132	Orbital Status Code
//            NCE = No Current Elements
//            NIE = No Initial Elements
//            NEA = No Elements Available
//            DOC = Permanently Docked
//            ISS = Docked to International Space Station
//            XXN = Central Body(XX) and Orbit Type(N)
//               AS = Asteroid
//               EA = Earth(default if blank)
//               EL = Earth Lagrange
//               EM = Earth-Moon Barycenter
//               JU = Jupiter
//               MA = Mars
//               ME = Mercury
//               MO = Moon(Earth)
//               NE = Neptune
//               PL = Pluto
//               SA = Saturn
//               SS = Solar System Escape
//               SU = Sun
//               UR = Uranus
//               VE = Venus
//               0 = Orbit
//               1 = Landing
//               2 = Impact
//               3 = Roundtrip
//            (e.g., SU0 = Heliocentric Orbit, MO2 = Lunar Impact)

namespace SatTraxGUI
{
    [Serializable]
    public class SatInfo
    {
        #region Field enum

        public enum Field
        {
            IntlDesc,
            LaunchYear,
            LaunchOfTheYear,
            PieceOfTheLaunch,
            NoradNum, //  NORAD Catalog Number
            MultipleNameFlag, // "M" if multiple names exist; alternate names found in satcat-annex.txt)
            PayloadFlag, //  Payload Flag("*" if payload; blank otherwise)
            OperationalStatusCode, //  Operational Status Code(https://celestrak.com/satcat/status.asp)
            SatelliteName, //  Satellite Name(s)
            SourceOrOwnership, //  Source or Ownership(https://celestrak.com/satcat/sources.asp)
            LaunchDate, //  Launch Date [year-month-day]
            LaunchSite, //  Launch Site
            DecayDate, //  Decay Date, if applicable[year - month - day]
            OrbitalPeriod, //  Orbital period [minutes]
            Inclination, //  Inclination[degrees]
            ApogeeAltitude, //  Apogee Altitude [kilometers]
            PerigeeAltitude, //  Perigee_Altitude [kilometers]
            RadarCrossSection, //  Radar Cross Section[meters2]; N/A if no data available
            OrbitalStatusCode //  Orbital Status Code
        }

        #endregion

        // Key   - Field value
        // Value - data value
        private readonly Dictionary<Field, string> _field = new Dictionary<Field, string>();

        public SatData.SatDataFields sdf;

        /// <summary>
        ///     Returns the requested TLE data field
        /// </summary>
        /// <remarks>
        ///     The numeric return values are cached; requesting the same field
        ///     repeatedly incurs minimal overhead.
        /// </remarks>
        /// <param name="fld">The field to retrieve.</param>
        /// <returns>
        ///     The requested field's value
        /// </returns>
        public string GetFieldStr(Field fld)
        {
            return _field[fld];
        }

        public int GetFieldInt(Field fld)
        {
            try
            {
                if (_field[fld] == string.Empty)
                {
                    return -1;
                }
                return int.Parse(_field[fld]);
            }
            catch (Exception)
            {
                throw new InvalidCastException("GetFieldInt");
            }
        }

        public double GetFieldDouble(Field fld)
        {
            try
            {
                if (_field[fld].Equals("N/A") ||
                    _field[fld] == string.Empty)
                {
                    return double.NaN;
                }
                return double.Parse(_field[fld]);
            }
            catch (Exception)
            {
                throw new InvalidCastException("GetFieldDouble");
            }
        }

        public string GetFieldDateTime(Field fld)
        {
            try
            {
                if (_field[fld] == string.Empty)
                {
                    return "N/A";
                }
                var dt = DateTime.Parse(_field[fld]);
                return dt == DateTime.MinValue ? "N/A" : dt.ToShortDateString();
            }
            catch (Exception)
            {
                throw new InvalidCastException("GetFieldDateTime");
            }
        }

        #region Properties

        public string IntlDesc
        {
            get { return GetFieldStr(Field.IntlDesc); }
        }

        public string LaunchYear
        {
            get { return GetFieldStr(Field.LaunchYear); }
        }

        public string LaunchOfTheYear
        {
            get { return GetFieldStr(Field.LaunchOfTheYear); }
        }

        public string PieceOfTheLaunch
        {
            get { return GetFieldStr(Field.PieceOfTheLaunch); }
        }

        public int NoradNumber
        {
            get { return GetFieldInt(Field.NoradNum); }
            set { }
        }

        public string MultipleNameFlag
        {
            get { return GetFieldStr(Field.MultipleNameFlag); }
        }

        public string PayloadFlag
        {
            get { return GetFieldStr(Field.PayloadFlag); }
        }

        public string OperationalStatusCode
        {
            get { return GetFieldStr(Field.OperationalStatusCode); }
        }

        public string SatelliteName
        {
            get { return GetFieldStr(Field.SatelliteName); }
            set { }
        }

        public string SourceOrOwnership
        {
            get { return GetFieldStr(Field.SourceOrOwnership); }
        }

        public string LaunchDate
        {
            get { return GetFieldDateTime(Field.LaunchDate); }
        }

        public string LaunchSite
        {
            get { return GetFieldStr(Field.LaunchSite); }
        }

        public string DecayDate
        {
            get { return GetFieldDateTime(Field.DecayDate); }
        }

        public double OrbitalPeriod
        {
            get { return GetFieldDouble(Field.OrbitalPeriod); }
        }

        public double Inclination
        {
            get { return GetFieldDouble(Field.Inclination); }
        }

        public int ApogeeAltitude
        {
            get { return GetFieldInt(Field.ApogeeAltitude); }
        }

        public int PerigeeAltitude
        {
            get { return GetFieldInt(Field.PerigeeAltitude); }
        }

        public double RadarCrossSection
        {
            get { return GetFieldDouble(Field.RadarCrossSection); }
        }

        public string OrbitalStatusCode
        {
            get { return GetFieldStr(Field.OrbitalStatusCode); }
        }

        #endregion

        #region Column Offsets

        // Note: The column offsets are zero-based.

        private const int SATINFO_COL_INTLDESC = 0;
        private const int SATINFO_LEN_INTLDESC = 11;
        private const int SATINFO_COL_LAUNCHYEAR = 0;
        private const int SATINFO_LEN_LAUNCHYEAR = 4;
        private const int SATINFO_COL_LAUNCHOFTHEYEAR = 5;
        private const int SATINFO_LEN_LAUNCHOFTHEYEAR = 3;
        private const int SATINFO_COL_PIECEOFTHELAUNCH = 8;
        private const int SATINFO_LEN_PIECEOFTHELAUNCH = 3;
        private const int SATINFO_COL_NORADNUM = 13;
        private const int SATINFO_LEN_NORADNUM = 5;
        private const int SATINFO_COL_MULTIPLENAMEFLAG = 19;
        private const int SATINFO_LEN_MULTIPLENAMEFLAG = 1;
        private const int SATINFO_COL_PAYLOADFLAG = 20;
        private const int SATINFO_LEN_PAYLOADFLAG = 1;
        private const int SATINFO_COL_OPERATIONALSTATUSCODE = 21;
        private const int SATINFO_LEN_OPERATIONALSTATUSCODE = 1;
        private const int SATINFO_COL_SATELLITENAME = 23;
        private const int SATINFO_LEN_SATELLITENAME = 25;
        private const int SATINFO_COL_SOURCEOROWNERSHIP = 49;
        private const int SATINFO_LEN_SOURCEOROWNERSHIP = 4;
        private const int SATINFO_COL_LAUNCHDATE = 56;
        private const int SATINFO_LEN_LAUNCHDATE = 10;
        private const int SATINFO_COL_LAUNCHSITE = 68;
        private const int SATINFO_LEN_LAUNCHSITE = 5;
        private const int SATINFO_COL_DECAYDATE = 75;
        private const int SATINFO_LEN_DECAYDATE = 10;
        private const int SATINFO_COL_ORBITALPERIOD = 87;
        private const int SATINFO_LEN_ORBITALPERIOD = 8;
        private const int SATINFO_COL_INCLINATION = 96;
        private const int SATINFO_LEN_INCLINATION = 5;
        private const int SATINFO_COL_APOGEEALTITUDE = 103;
        private const int SATINFO_LEN_APOGEEALTITUDE = 6;
        private const int SATINFO_COL_PERIGEEALTITUDE = 111;
        private const int SATINFO_LEN_PERIGEEALTITUDE = 6;
        private const int SATINFO_COL_RADARCROSSSECTION = 119;
        private const int SATINFO_LEN_RADARCROSSSECTION = 8;
        private const int SATINFO_COL_ORBITALSTATUSCODE = 129;
        private const int SATINFO_LEN_ORBITALSTATUSCODE = 3;

        #endregion

        #region Construction

        ////////////////////////////////////////////////////////////////////////////
        public SatInfo()
        {
        }

        public SatInfo(string line)
        {
            Initilize(line);
        }

        private void Initilize(string line)
        {
            _field[Field.IntlDesc] = line.Substring(SATINFO_COL_INTLDESC, SATINFO_LEN_INTLDESC).Trim();
            _field[Field.LaunchYear] = line.Substring(SATINFO_COL_LAUNCHYEAR, SATINFO_LEN_LAUNCHYEAR).Trim();
            _field[Field.LaunchOfTheYear] = line.Substring(SATINFO_COL_LAUNCHOFTHEYEAR, SATINFO_LEN_LAUNCHOFTHEYEAR).Trim();
            _field[Field.PieceOfTheLaunch] = line.Substring(SATINFO_COL_PIECEOFTHELAUNCH, SATINFO_LEN_PIECEOFTHELAUNCH).Trim();
            _field[Field.NoradNum] = line.Substring(SATINFO_COL_NORADNUM, SATINFO_LEN_NORADNUM).Trim();
            _field[Field.MultipleNameFlag] = line.Substring(SATINFO_COL_MULTIPLENAMEFLAG, SATINFO_LEN_MULTIPLENAMEFLAG).Trim();
            _field[Field.PayloadFlag] = line.Substring(SATINFO_COL_PAYLOADFLAG, SATINFO_LEN_PAYLOADFLAG).Trim();
            _field[Field.OperationalStatusCode] = line.Substring(SATINFO_COL_OPERATIONALSTATUSCODE, SATINFO_LEN_OPERATIONALSTATUSCODE).Trim();
            _field[Field.SatelliteName] = line.Substring(SATINFO_COL_SATELLITENAME, SATINFO_LEN_SATELLITENAME).Trim();
            _field[Field.SourceOrOwnership] = line.Substring(SATINFO_COL_SOURCEOROWNERSHIP, SATINFO_LEN_SOURCEOROWNERSHIP).Trim();
            _field[Field.LaunchDate] = line.Substring(SATINFO_COL_LAUNCHDATE, SATINFO_LEN_LAUNCHDATE).Trim();
            _field[Field.LaunchSite] = line.Substring(SATINFO_COL_LAUNCHSITE, SATINFO_LEN_LAUNCHSITE).Trim();
            _field[Field.DecayDate] = line.Substring(SATINFO_COL_DECAYDATE, SATINFO_LEN_DECAYDATE).Trim();
            _field[Field.OrbitalPeriod] = line.Substring(SATINFO_COL_ORBITALPERIOD, SATINFO_LEN_ORBITALPERIOD).Trim();
            _field[Field.Inclination] = line.Substring(SATINFO_COL_INCLINATION, SATINFO_LEN_INCLINATION).Trim();
            _field[Field.ApogeeAltitude] = line.Substring(SATINFO_COL_APOGEEALTITUDE, SATINFO_LEN_APOGEEALTITUDE).Trim();
            _field[Field.PerigeeAltitude] = line.Substring(SATINFO_COL_PERIGEEALTITUDE, SATINFO_LEN_PERIGEEALTITUDE).Trim();
            _field[Field.RadarCrossSection] = line.Substring(SATINFO_COL_RADARCROSSSECTION, SATINFO_LEN_RADARCROSSSECTION).Trim();
            _field[Field.OrbitalStatusCode] = line.Substring(SATINFO_COL_ORBITALSTATUSCODE, SATINFO_LEN_ORBITALSTATUSCODE).Trim();
        }

        #endregion
    }
}