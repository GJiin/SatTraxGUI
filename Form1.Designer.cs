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


namespace SatTraxGUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listEventScript = new System.Windows.Forms.ListBox();
            this.ConsoleListBox = new System.Windows.Forms.ListBox();
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.ClearConsole = new System.Windows.Forms.MenuItem();
            this.boxSatellite = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tbTrackType = new System.Windows.Forms.MaskedTextBox();
            this.tbThreshold = new System.Windows.Forms.MaskedTextBox();
            this.tbMaxPathDev = new System.Windows.Forms.MaskedTextBox();
            this.label64 = new System.Windows.Forms.Label();
            this.tbSearchType = new System.Windows.Forms.MaskedTextBox();
            this.tbTrackingFreq = new System.Windows.Forms.MaskedTextBox();
            this.label67 = new System.Windows.Forms.Label();
            this.tbStopTimeAzEl = new System.Windows.Forms.MaskedTextBox();
            this.label68 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.tbStartTimeAzEl = new System.Windows.Forms.MaskedTextBox();
            this.tbProgAutoTrack = new System.Windows.Forms.MaskedTextBox();
            this.label54 = new System.Windows.Forms.Label();
            this.tbSampleRate = new System.Windows.Forms.MaskedTextBox();
            this.label55 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.tbPolarity = new System.Windows.Forms.MaskedTextBox();
            this.label51 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.btnAddToFav = new System.Windows.Forms.Button();
            this.SatelliteInfo_MultipleNameFlag = new System.Windows.Forms.Label();
            this.SatelliteInfo_Orbital_Status_Code = new System.Windows.Forms.Label();
            this.SatelliteInfo_Radar_Cross_Section_meters2 = new System.Windows.Forms.Label();
            this.SatelliteInfo_Perigee_Altitude_kilometers = new System.Windows.Forms.Label();
            this.SatelliteInfo_Apogee_Altitude_kilometers = new System.Windows.Forms.Label();
            this.SatelliteInfo_Inclination_degrees = new System.Windows.Forms.Label();
            this.SatelliteInfo_OrbitalPeriod = new System.Windows.Forms.Label();
            this.SatelliteInfo_DecayDate = new System.Windows.Forms.Label();
            this.SatelliteInfo_LaunchSite = new System.Windows.Forms.Label();
            this.SatelliteInfo_LaunchDate = new System.Windows.Forms.Label();
            this.SatelliteInfo_SourceOrOwnership = new System.Windows.Forms.Label();
            this.SatelliteInfo_OperationalStatusCode = new System.Windows.Forms.Label();
            this.SatelliteInfo_PayloadFlag = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.listBoxSatelliteName = new System.Windows.Forms.ComboBox();
            this.SatelliteInfo_NoradNumber = new System.Windows.Forms.TextBox();
            this.SatelliteInfo_Designator = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelEpochInfo = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.boxEarthLocation = new System.Windows.Forms.GroupBox();
            this.cbCanted = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.rbStandard = new System.Windows.Forms.RadioButton();
            this.rbTilt = new System.Windows.Forms.RadioButton();
            this.rbOverhead = new System.Windows.Forms.RadioButton();
            this.LocationElevation = new System.Windows.Forms.TextBox();
            this.LocationLongitude = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label47 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.ipAddressControl1 = new IPAddressControlLib.IPAddressControl();
            this.tbMXPPORT = new System.Windows.Forms.TextBox();
            this.tbMXPMASK = new System.Windows.Forms.TextBox();
            this.LocationLatitude = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.listLocationName = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbLocationName = new System.Windows.Forms.TextBox();
            this.labelEventInterval = new System.Windows.Forms.Label();
            this.EventInterval = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.EventScriptSave = new System.Windows.Forms.Button();
            this.EventScriptFilename = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.TLE_Line1_SatelliteNumber = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.TLE_Line1_Checksum = new System.Windows.Forms.Label();
            this.TLE_Line1_ElementSetNumber = new System.Windows.Forms.Label();
            this.TLE_Line1_EphemerisType = new System.Windows.Forms.Label();
            this.TLE_Line1_BSTAR = new System.Windows.Forms.Label();
            this.TLE_Line1_MeanMotionSecondDt = new System.Windows.Forms.Label();
            this.TLE_Line1_MeanMotionFirstDt = new System.Windows.Forms.Label();
            this.TLE_Line1_EpochDayPart = new System.Windows.Forms.Label();
            this.TLE_Line1_EpochYear = new System.Windows.Forms.Label();
            this.TLE_Line1_LaunchPiece = new System.Windows.Forms.Label();
            this.TLE_Line1_LaunchNum = new System.Windows.Forms.Label();
            this.TLE_Line1_LaunchYear = new System.Windows.Forms.Label();
            this.TLE_Line1_Classification = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.EpochYear = new System.Windows.Forms.Label();
            this.labelXXX = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.TLE_Line2_Checksum = new System.Windows.Forms.Label();
            this.TLE_Line2_MeanMotion = new System.Windows.Forms.Label();
            this.TLE_Line2_RevolutionNum = new System.Windows.Forms.Label();
            this.TLE_Line2_MeanAnomaly = new System.Windows.Forms.Label();
            this.TLE_Line2_PerigeeArg = new System.Windows.Forms.Label();
            this.TLE_Line2_Eccentricity = new System.Windows.Forms.Label();
            this.TLE_Line2_Ascension = new System.Windows.Forms.Label();
            this.TLE_Line2_Inclination = new System.Windows.Forms.Label();
            this.TLE_Line2_SatelliteNumber = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.TLE_SatelliteName = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TLE_Line2 = new System.Windows.Forms.Label();
            this.TLE_Line1 = new System.Windows.Forms.Label();
            this.EpochStartDateTime = new System.Windows.Forms.Label();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnSiteSave = new System.Windows.Forms.Button();
            this.btnSiteRemove = new System.Windows.Forms.Button();
            this.btnSiteNew = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.btnGetSatInfo = new System.Windows.Forms.Button();
            this.btnRemoveFav = new System.Windows.Forms.Button();
            this.Fav_ListBox1 = new System.Windows.Forms.ListBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.GenerateScriptButton = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.boxPasses = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbLeadTime_ms = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.dateTimePicker_UTC_End = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_UTC_Start = new System.Windows.Forms.DateTimePicker();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dateTimePicker_LOCAL_Start = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_LOCAL_End = new System.Windows.Forms.DateTimePicker();
            this.txtWindowSize = new System.Windows.Forms.TextBox();
            this.chkIgnoreStartTimer = new System.Windows.Forms.CheckBox();
            this.cbVisibleOnly = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnBuildScript = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.chkIgnoreIntervalTimer = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.boxSatellite.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.boxEarthLocation.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.Controls.Add(this.listEventScript);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.Controls.Add(this.ConsoleListBox);
            // 
            // listEventScript
            // 
            resources.ApplyResources(this.listEventScript, "listEventScript");
            this.listEventScript.FormattingEnabled = true;
            this.listEventScript.Name = "listEventScript";
            this.listEventScript.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // ConsoleListBox
            // 
            this.ConsoleListBox.ContextMenu = this.contextMenu;
            resources.ApplyResources(this.ConsoleListBox, "ConsoleListBox");
            this.ConsoleListBox.FormattingEnabled = true;
            this.ConsoleListBox.Name = "ConsoleListBox";
            this.ConsoleListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // contextMenu
            // 
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ClearConsole});
            // 
            // ClearConsole
            // 
            this.ClearConsole.Index = 0;
            resources.ApplyResources(this.ClearConsole, "ClearConsole");
            this.ClearConsole.Click += new System.EventHandler(this.ClearConsole_Click);
            // 
            // boxSatellite
            // 
            this.boxSatellite.Controls.Add(this.groupBox5);
            this.boxSatellite.Controls.Add(this.btnAddToFav);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_MultipleNameFlag);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_Orbital_Status_Code);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_Radar_Cross_Section_meters2);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_Perigee_Altitude_kilometers);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_Apogee_Altitude_kilometers);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_Inclination_degrees);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_OrbitalPeriod);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_DecayDate);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_LaunchSite);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_LaunchDate);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_SourceOrOwnership);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_OperationalStatusCode);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_PayloadFlag);
            this.boxSatellite.Controls.Add(this.label43);
            this.boxSatellite.Controls.Add(this.label42);
            this.boxSatellite.Controls.Add(this.label41);
            this.boxSatellite.Controls.Add(this.label40);
            this.boxSatellite.Controls.Add(this.label39);
            this.boxSatellite.Controls.Add(this.label38);
            this.boxSatellite.Controls.Add(this.label33);
            this.boxSatellite.Controls.Add(this.label32);
            this.boxSatellite.Controls.Add(this.label27);
            this.boxSatellite.Controls.Add(this.label23);
            this.boxSatellite.Controls.Add(this.label22);
            this.boxSatellite.Controls.Add(this.label21);
            this.boxSatellite.Controls.Add(this.label20);
            this.boxSatellite.Controls.Add(this.listBoxSatelliteName);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_NoradNumber);
            this.boxSatellite.Controls.Add(this.SatelliteInfo_Designator);
            this.boxSatellite.Controls.Add(this.label3);
            this.boxSatellite.Controls.Add(this.label2);
            this.boxSatellite.Controls.Add(this.label1);
            resources.ApplyResources(this.boxSatellite, "boxSatellite");
            this.boxSatellite.Name = "boxSatellite";
            this.boxSatellite.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox5.Controls.Add(this.tbTrackType);
            this.groupBox5.Controls.Add(this.tbThreshold);
            this.groupBox5.Controls.Add(this.tbMaxPathDev);
            this.groupBox5.Controls.Add(this.label64);
            this.groupBox5.Controls.Add(this.tbSearchType);
            this.groupBox5.Controls.Add(this.tbTrackingFreq);
            this.groupBox5.Controls.Add(this.label67);
            this.groupBox5.Controls.Add(this.tbStopTimeAzEl);
            this.groupBox5.Controls.Add(this.label68);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label56);
            this.groupBox5.Controls.Add(this.label58);
            this.groupBox5.Controls.Add(this.tbStartTimeAzEl);
            this.groupBox5.Controls.Add(this.tbProgAutoTrack);
            this.groupBox5.Controls.Add(this.label54);
            this.groupBox5.Controls.Add(this.tbSampleRate);
            this.groupBox5.Controls.Add(this.label55);
            this.groupBox5.Controls.Add(this.label52);
            this.groupBox5.Controls.Add(this.tbPolarity);
            this.groupBox5.Controls.Add(this.label51);
            this.groupBox5.Controls.Add(this.label48);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // tbTrackType
            // 
            resources.ApplyResources(this.tbTrackType, "tbTrackType");
            this.tbTrackType.Name = "tbTrackType";
            this.tbTrackType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTrackType_KeyPress);
            this.tbTrackType.Leave += new System.EventHandler(this.tbTrackType_Leave);
            // 
            // tbThreshold
            // 
            resources.ApplyResources(this.tbThreshold, "tbThreshold");
            this.tbThreshold.Name = "tbThreshold";
            this.tbThreshold.ValidatingType = typeof(int);
            this.tbThreshold.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbThreshold_KeyPress);
            this.tbThreshold.Leave += new System.EventHandler(this.tbThreshold_Leave);
            // 
            // tbMaxPathDev
            // 
            resources.ApplyResources(this.tbMaxPathDev, "tbMaxPathDev");
            this.tbMaxPathDev.Name = "tbMaxPathDev";
            this.tbMaxPathDev.ValidatingType = typeof(int);
            this.tbMaxPathDev.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMaxPathDev_KeyPress);
            this.tbMaxPathDev.Leave += new System.EventHandler(this.tbMaxPathDev_Leave);
            // 
            // label64
            // 
            resources.ApplyResources(this.label64, "label64");
            this.label64.Name = "label64";
            // 
            // tbSearchType
            // 
            resources.ApplyResources(this.tbSearchType, "tbSearchType");
            this.tbSearchType.Name = "tbSearchType";
            this.tbSearchType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSearchType_KeyPress);
            this.tbSearchType.Leave += new System.EventHandler(this.tbSearchType_Leave);
            // 
            // tbTrackingFreq
            // 
            resources.ApplyResources(this.tbTrackingFreq, "tbTrackingFreq");
            this.tbTrackingFreq.Name = "tbTrackingFreq";
            this.tbTrackingFreq.ValidatingType = typeof(int);
            this.tbTrackingFreq.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTrackingFreq_KeyPress);
            this.tbTrackingFreq.Leave += new System.EventHandler(this.tbTrackingFreq_Leave);
            // 
            // label67
            // 
            resources.ApplyResources(this.label67, "label67");
            this.label67.Name = "label67";
            // 
            // tbStopTimeAzEl
            // 
            resources.ApplyResources(this.tbStopTimeAzEl, "tbStopTimeAzEl");
            this.tbStopTimeAzEl.Name = "tbStopTimeAzEl";
            this.tbStopTimeAzEl.ValidatingType = typeof(System.DateTime);
            this.tbStopTimeAzEl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbStopTimeAzEl_KeyPress);
            this.tbStopTimeAzEl.Leave += new System.EventHandler(this.tbStopTimeAzEl_Leave);
            // 
            // label68
            // 
            resources.ApplyResources(this.label68, "label68");
            this.label68.Name = "label68";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label56
            // 
            resources.ApplyResources(this.label56, "label56");
            this.label56.Name = "label56";
            // 
            // label58
            // 
            resources.ApplyResources(this.label58, "label58");
            this.label58.Name = "label58";
            // 
            // tbStartTimeAzEl
            // 
            resources.ApplyResources(this.tbStartTimeAzEl, "tbStartTimeAzEl");
            this.tbStartTimeAzEl.Name = "tbStartTimeAzEl";
            this.tbStartTimeAzEl.ValidatingType = typeof(System.DateTime);
            this.tbStartTimeAzEl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbStartTimeAzEl_KeyPress);
            this.tbStartTimeAzEl.Leave += new System.EventHandler(this.tbStartTimeAzEl_Leave);
            // 
            // tbProgAutoTrack
            // 
            resources.ApplyResources(this.tbProgAutoTrack, "tbProgAutoTrack");
            this.tbProgAutoTrack.Name = "tbProgAutoTrack";
            this.tbProgAutoTrack.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbProgAutoTrack_KeyPress);
            this.tbProgAutoTrack.Leave += new System.EventHandler(this.tbProgAutoTrack_Leave);
            // 
            // label54
            // 
            resources.ApplyResources(this.label54, "label54");
            this.label54.Name = "label54";
            // 
            // tbSampleRate
            // 
            resources.ApplyResources(this.tbSampleRate, "tbSampleRate");
            this.tbSampleRate.Name = "tbSampleRate";
            this.tbSampleRate.ValidatingType = typeof(int);
            this.tbSampleRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSampleRate_KeyPress);
            this.tbSampleRate.Leave += new System.EventHandler(this.tbSampleRate_Leave);
            // 
            // label55
            // 
            resources.ApplyResources(this.label55, "label55");
            this.label55.Name = "label55";
            // 
            // label52
            // 
            resources.ApplyResources(this.label52, "label52");
            this.label52.Name = "label52";
            // 
            // tbPolarity
            // 
            resources.ApplyResources(this.tbPolarity, "tbPolarity");
            this.tbPolarity.Name = "tbPolarity";
            this.tbPolarity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPolarity_KeyPress);
            this.tbPolarity.Leave += new System.EventHandler(this.tbPolarity_Leave);
            // 
            // label51
            // 
            resources.ApplyResources(this.label51, "label51");
            this.label51.Name = "label51";
            // 
            // label48
            // 
            resources.ApplyResources(this.label48, "label48");
            this.label48.Name = "label48";
            // 
            // btnAddToFav
            // 
            resources.ApplyResources(this.btnAddToFav, "btnAddToFav");
            this.btnAddToFav.Name = "btnAddToFav";
            this.btnAddToFav.UseVisualStyleBackColor = true;
            this.btnAddToFav.Click += new System.EventHandler(this.btnSiteSave_Click);
            // 
            // SatelliteInfo_MultipleNameFlag
            // 
            resources.ApplyResources(this.SatelliteInfo_MultipleNameFlag, "SatelliteInfo_MultipleNameFlag");
            this.SatelliteInfo_MultipleNameFlag.Name = "SatelliteInfo_MultipleNameFlag";
            // 
            // SatelliteInfo_Orbital_Status_Code
            // 
            resources.ApplyResources(this.SatelliteInfo_Orbital_Status_Code, "SatelliteInfo_Orbital_Status_Code");
            this.SatelliteInfo_Orbital_Status_Code.Name = "SatelliteInfo_Orbital_Status_Code";
            // 
            // SatelliteInfo_Radar_Cross_Section_meters2
            // 
            resources.ApplyResources(this.SatelliteInfo_Radar_Cross_Section_meters2, "SatelliteInfo_Radar_Cross_Section_meters2");
            this.SatelliteInfo_Radar_Cross_Section_meters2.Name = "SatelliteInfo_Radar_Cross_Section_meters2";
            // 
            // SatelliteInfo_Perigee_Altitude_kilometers
            // 
            resources.ApplyResources(this.SatelliteInfo_Perigee_Altitude_kilometers, "SatelliteInfo_Perigee_Altitude_kilometers");
            this.SatelliteInfo_Perigee_Altitude_kilometers.Name = "SatelliteInfo_Perigee_Altitude_kilometers";
            // 
            // SatelliteInfo_Apogee_Altitude_kilometers
            // 
            resources.ApplyResources(this.SatelliteInfo_Apogee_Altitude_kilometers, "SatelliteInfo_Apogee_Altitude_kilometers");
            this.SatelliteInfo_Apogee_Altitude_kilometers.Name = "SatelliteInfo_Apogee_Altitude_kilometers";
            // 
            // SatelliteInfo_Inclination_degrees
            // 
            resources.ApplyResources(this.SatelliteInfo_Inclination_degrees, "SatelliteInfo_Inclination_degrees");
            this.SatelliteInfo_Inclination_degrees.Name = "SatelliteInfo_Inclination_degrees";
            // 
            // SatelliteInfo_OrbitalPeriod
            // 
            resources.ApplyResources(this.SatelliteInfo_OrbitalPeriod, "SatelliteInfo_OrbitalPeriod");
            this.SatelliteInfo_OrbitalPeriod.Name = "SatelliteInfo_OrbitalPeriod";
            // 
            // SatelliteInfo_DecayDate
            // 
            resources.ApplyResources(this.SatelliteInfo_DecayDate, "SatelliteInfo_DecayDate");
            this.SatelliteInfo_DecayDate.Name = "SatelliteInfo_DecayDate";
            // 
            // SatelliteInfo_LaunchSite
            // 
            resources.ApplyResources(this.SatelliteInfo_LaunchSite, "SatelliteInfo_LaunchSite");
            this.SatelliteInfo_LaunchSite.Name = "SatelliteInfo_LaunchSite";
            // 
            // SatelliteInfo_LaunchDate
            // 
            resources.ApplyResources(this.SatelliteInfo_LaunchDate, "SatelliteInfo_LaunchDate");
            this.SatelliteInfo_LaunchDate.Name = "SatelliteInfo_LaunchDate";
            // 
            // SatelliteInfo_SourceOrOwnership
            // 
            resources.ApplyResources(this.SatelliteInfo_SourceOrOwnership, "SatelliteInfo_SourceOrOwnership");
            this.SatelliteInfo_SourceOrOwnership.Name = "SatelliteInfo_SourceOrOwnership";
            // 
            // SatelliteInfo_OperationalStatusCode
            // 
            resources.ApplyResources(this.SatelliteInfo_OperationalStatusCode, "SatelliteInfo_OperationalStatusCode");
            this.SatelliteInfo_OperationalStatusCode.Name = "SatelliteInfo_OperationalStatusCode";
            // 
            // SatelliteInfo_PayloadFlag
            // 
            resources.ApplyResources(this.SatelliteInfo_PayloadFlag, "SatelliteInfo_PayloadFlag");
            this.SatelliteInfo_PayloadFlag.Name = "SatelliteInfo_PayloadFlag";
            // 
            // label43
            // 
            resources.ApplyResources(this.label43, "label43");
            this.label43.Name = "label43";
            // 
            // label42
            // 
            resources.ApplyResources(this.label42, "label42");
            this.label42.Name = "label42";
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.Name = "label41";
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.Name = "label40";
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Name = "label39";
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.Name = "label38";
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.Name = "label32";
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // listBoxSatelliteName
            // 
            resources.ApplyResources(this.listBoxSatelliteName, "listBoxSatelliteName");
            this.listBoxSatelliteName.Name = "listBoxSatelliteName";
            this.listBoxSatelliteName.SelectedIndexChanged += new System.EventHandler(this.listboxSatellite_Name_SelectedIndexChanged);
            this.listBoxSatelliteName.Leave += new System.EventHandler(this.boxSatellite_Name_Leave);
            // 
            // SatelliteInfo_NoradNumber
            // 
            resources.ApplyResources(this.SatelliteInfo_NoradNumber, "SatelliteInfo_NoradNumber");
            this.SatelliteInfo_NoradNumber.Name = "SatelliteInfo_NoradNumber";
            // 
            // SatelliteInfo_Designator
            // 
            resources.ApplyResources(this.SatelliteInfo_Designator, "SatelliteInfo_Designator");
            this.SatelliteInfo_Designator.Name = "SatelliteInfo_Designator";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // labelEpochInfo
            // 
            resources.ApplyResources(this.labelEpochInfo, "labelEpochInfo");
            this.labelEpochInfo.Name = "labelEpochInfo";
            // 
            // label44
            // 
            resources.ApplyResources(this.label44, "label44");
            this.label44.Name = "label44";
            // 
            // boxEarthLocation
            // 
            resources.ApplyResources(this.boxEarthLocation, "boxEarthLocation");
            this.boxEarthLocation.Controls.Add(this.cbCanted);
            this.boxEarthLocation.Controls.Add(this.groupBox9);
            this.boxEarthLocation.Controls.Add(this.LocationElevation);
            this.boxEarthLocation.Controls.Add(this.LocationLongitude);
            this.boxEarthLocation.Controls.Add(this.groupBox7);
            this.boxEarthLocation.Controls.Add(this.LocationLatitude);
            this.boxEarthLocation.Controls.Add(this.label9);
            this.boxEarthLocation.Controls.Add(this.label6);
            this.boxEarthLocation.Controls.Add(this.label5);
            this.boxEarthLocation.Controls.Add(this.listLocationName);
            this.boxEarthLocation.Controls.Add(this.label4);
            this.boxEarthLocation.Controls.Add(this.tbLocationName);
            this.boxEarthLocation.Name = "boxEarthLocation";
            this.boxEarthLocation.TabStop = false;
            // 
            // cbCanted
            // 
            resources.ApplyResources(this.cbCanted, "cbCanted");
            this.cbCanted.Name = "cbCanted";
            this.cbCanted.UseVisualStyleBackColor = true;
            this.cbCanted.CheckedChanged += new System.EventHandler(this.cbCanted_CheckedChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.rbStandard);
            this.groupBox9.Controls.Add(this.rbTilt);
            this.groupBox9.Controls.Add(this.rbOverhead);
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // rbStandard
            // 
            resources.ApplyResources(this.rbStandard, "rbStandard");
            this.rbStandard.Checked = true;
            this.rbStandard.Name = "rbStandard";
            this.rbStandard.TabStop = true;
            this.rbStandard.UseVisualStyleBackColor = true;
            this.rbStandard.CheckedChanged += new System.EventHandler(this.radioButton5_CheckedChanged);
            // 
            // rbTilt
            // 
            resources.ApplyResources(this.rbTilt, "rbTilt");
            this.rbTilt.Name = "rbTilt";
            this.rbTilt.UseVisualStyleBackColor = true;
            this.rbTilt.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // rbOverhead
            // 
            resources.ApplyResources(this.rbOverhead, "rbOverhead");
            this.rbOverhead.Name = "rbOverhead";
            this.rbOverhead.TabStop = true;
            this.rbOverhead.UseVisualStyleBackColor = true;
            this.rbOverhead.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // LocationElevation
            // 
            resources.ApplyResources(this.LocationElevation, "LocationElevation");
            this.LocationElevation.Name = "LocationElevation";
            // 
            // LocationLongitude
            // 
            resources.ApplyResources(this.LocationLongitude, "LocationLongitude");
            this.LocationLongitude.Name = "LocationLongitude";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label47);
            this.groupBox7.Controls.Add(this.label46);
            this.groupBox7.Controls.Add(this.label45);
            this.groupBox7.Controls.Add(this.ipAddressControl1);
            this.groupBox7.Controls.Add(this.tbMXPPORT);
            this.groupBox7.Controls.Add(this.tbMXPMASK);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // label47
            // 
            resources.ApplyResources(this.label47, "label47");
            this.label47.Name = "label47";
            // 
            // label46
            // 
            resources.ApplyResources(this.label46, "label46");
            this.label46.Name = "label46";
            // 
            // label45
            // 
            resources.ApplyResources(this.label45, "label45");
            this.label45.Name = "label45";
            // 
            // ipAddressControl1
            // 
            this.ipAddressControl1.AllowInternalTab = false;
            this.ipAddressControl1.AutoHeight = true;
            this.ipAddressControl1.BackColor = System.Drawing.SystemColors.Window;
            this.ipAddressControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ipAddressControl1.Cursor = System.Windows.Forms.Cursors.IBeam;
            resources.ApplyResources(this.ipAddressControl1, "ipAddressControl1");
            this.ipAddressControl1.Name = "ipAddressControl1";
            this.ipAddressControl1.ReadOnly = false;
            this.ipAddressControl1.Leave += new System.EventHandler(this.ipAddressControl1_Leave);
            // 
            // tbMXPPORT
            // 
            resources.ApplyResources(this.tbMXPPORT, "tbMXPPORT");
            this.tbMXPPORT.Name = "tbMXPPORT";
            // 
            // tbMXPMASK
            // 
            resources.ApplyResources(this.tbMXPMASK, "tbMXPMASK");
            this.tbMXPMASK.Name = "tbMXPMASK";
            // 
            // LocationLatitude
            // 
            resources.ApplyResources(this.LocationLatitude, "LocationLatitude");
            this.LocationLatitude.Name = "LocationLatitude";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // listLocationName
            // 
            this.listLocationName.FormattingEnabled = true;
            this.listLocationName.Items.AddRange(new object[] {
            resources.GetString("listLocationName.Items")});
            resources.ApplyResources(this.listLocationName, "listLocationName");
            this.listLocationName.Name = "listLocationName";
            this.listLocationName.SelectedIndexChanged += new System.EventHandler(this.listLocationName_SelectedIndexChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tbLocationName
            // 
            resources.ApplyResources(this.tbLocationName, "tbLocationName");
            this.tbLocationName.Name = "tbLocationName";
            // 
            // labelEventInterval
            // 
            resources.ApplyResources(this.labelEventInterval, "labelEventInterval");
            this.labelEventInterval.Name = "labelEventInterval";
            // 
            // EventInterval
            // 
            resources.ApplyResources(this.EventInterval, "EventInterval");
            this.EventInterval.Name = "EventInterval";
            this.EventInterval.Leave += new System.EventHandler(this.EventInterval_Leave);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.EventScriptSave);
            this.groupBox1.Controls.Add(this.EventScriptFilename);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // EventScriptSave
            // 
            resources.ApplyResources(this.EventScriptSave, "EventScriptSave");
            this.EventScriptSave.Name = "EventScriptSave";
            this.EventScriptSave.UseVisualStyleBackColor = true;
            this.EventScriptSave.Click += new System.EventHandler(this.EventScriptSave_Click);
            // 
            // EventScriptFilename
            // 
            resources.ApplyResources(this.EventScriptFilename, "EventScriptFilename");
            this.EventScriptFilename.Name = "EventScriptFilename";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.TLE_Line1_SatelliteNumber);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this.TLE_Line1_Checksum);
            this.tabPage1.Controls.Add(this.TLE_Line1_ElementSetNumber);
            this.tabPage1.Controls.Add(this.TLE_Line1_EphemerisType);
            this.tabPage1.Controls.Add(this.TLE_Line1_BSTAR);
            this.tabPage1.Controls.Add(this.TLE_Line1_MeanMotionSecondDt);
            this.tabPage1.Controls.Add(this.TLE_Line1_MeanMotionFirstDt);
            this.tabPage1.Controls.Add(this.TLE_Line1_EpochDayPart);
            this.tabPage1.Controls.Add(this.TLE_Line1_EpochYear);
            this.tabPage1.Controls.Add(this.TLE_Line1_LaunchPiece);
            this.tabPage1.Controls.Add(this.TLE_Line1_LaunchNum);
            this.tabPage1.Controls.Add(this.TLE_Line1_LaunchYear);
            this.tabPage1.Controls.Add(this.TLE_Line1_Classification);
            this.tabPage1.Controls.Add(this.label24);
            this.tabPage1.Controls.Add(this.label25);
            this.tabPage1.Controls.Add(this.label26);
            this.tabPage1.Controls.Add(this.label28);
            this.tabPage1.Controls.Add(this.label29);
            this.tabPage1.Controls.Add(this.label30);
            this.tabPage1.Controls.Add(this.label31);
            this.tabPage1.Controls.Add(this.EpochYear);
            this.tabPage1.Controls.Add(this.labelXXX);
            this.tabPage1.Controls.Add(this.label34);
            this.tabPage1.Controls.Add(this.label35);
            this.tabPage1.Controls.Add(this.label36);
            this.tabPage1.Controls.Add(this.label37);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // TLE_Line1_SatelliteNumber
            // 
            resources.ApplyResources(this.TLE_Line1_SatelliteNumber, "TLE_Line1_SatelliteNumber");
            this.TLE_Line1_SatelliteNumber.Name = "TLE_Line1_SatelliteNumber";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // TLE_Line1_Checksum
            // 
            resources.ApplyResources(this.TLE_Line1_Checksum, "TLE_Line1_Checksum");
            this.TLE_Line1_Checksum.Name = "TLE_Line1_Checksum";
            // 
            // TLE_Line1_ElementSetNumber
            // 
            resources.ApplyResources(this.TLE_Line1_ElementSetNumber, "TLE_Line1_ElementSetNumber");
            this.TLE_Line1_ElementSetNumber.Name = "TLE_Line1_ElementSetNumber";
            // 
            // TLE_Line1_EphemerisType
            // 
            resources.ApplyResources(this.TLE_Line1_EphemerisType, "TLE_Line1_EphemerisType");
            this.TLE_Line1_EphemerisType.Name = "TLE_Line1_EphemerisType";
            // 
            // TLE_Line1_BSTAR
            // 
            resources.ApplyResources(this.TLE_Line1_BSTAR, "TLE_Line1_BSTAR");
            this.TLE_Line1_BSTAR.Name = "TLE_Line1_BSTAR";
            // 
            // TLE_Line1_MeanMotionSecondDt
            // 
            resources.ApplyResources(this.TLE_Line1_MeanMotionSecondDt, "TLE_Line1_MeanMotionSecondDt");
            this.TLE_Line1_MeanMotionSecondDt.Name = "TLE_Line1_MeanMotionSecondDt";
            // 
            // TLE_Line1_MeanMotionFirstDt
            // 
            resources.ApplyResources(this.TLE_Line1_MeanMotionFirstDt, "TLE_Line1_MeanMotionFirstDt");
            this.TLE_Line1_MeanMotionFirstDt.Name = "TLE_Line1_MeanMotionFirstDt";
            // 
            // TLE_Line1_EpochDayPart
            // 
            resources.ApplyResources(this.TLE_Line1_EpochDayPart, "TLE_Line1_EpochDayPart");
            this.TLE_Line1_EpochDayPart.Name = "TLE_Line1_EpochDayPart";
            // 
            // TLE_Line1_EpochYear
            // 
            resources.ApplyResources(this.TLE_Line1_EpochYear, "TLE_Line1_EpochYear");
            this.TLE_Line1_EpochYear.Name = "TLE_Line1_EpochYear";
            // 
            // TLE_Line1_LaunchPiece
            // 
            resources.ApplyResources(this.TLE_Line1_LaunchPiece, "TLE_Line1_LaunchPiece");
            this.TLE_Line1_LaunchPiece.Name = "TLE_Line1_LaunchPiece";
            // 
            // TLE_Line1_LaunchNum
            // 
            resources.ApplyResources(this.TLE_Line1_LaunchNum, "TLE_Line1_LaunchNum");
            this.TLE_Line1_LaunchNum.Name = "TLE_Line1_LaunchNum";
            // 
            // TLE_Line1_LaunchYear
            // 
            resources.ApplyResources(this.TLE_Line1_LaunchYear, "TLE_Line1_LaunchYear");
            this.TLE_Line1_LaunchYear.Name = "TLE_Line1_LaunchYear";
            // 
            // TLE_Line1_Classification
            // 
            resources.ApplyResources(this.TLE_Line1_Classification, "TLE_Line1_Classification");
            this.TLE_Line1_Classification.Name = "TLE_Line1_Classification";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.Name = "label31";
            // 
            // EpochYear
            // 
            resources.ApplyResources(this.EpochYear, "EpochYear");
            this.EpochYear.Name = "EpochYear";
            // 
            // labelXXX
            // 
            resources.ApplyResources(this.labelXXX, "labelXXX");
            this.labelXXX.Name = "labelXXX";
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.Name = "label36";
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.Name = "label37";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.TLE_Line2_Checksum);
            this.tabPage2.Controls.Add(this.TLE_Line2_MeanMotion);
            this.tabPage2.Controls.Add(this.TLE_Line2_RevolutionNum);
            this.tabPage2.Controls.Add(this.TLE_Line2_MeanAnomaly);
            this.tabPage2.Controls.Add(this.TLE_Line2_PerigeeArg);
            this.tabPage2.Controls.Add(this.TLE_Line2_Eccentricity);
            this.tabPage2.Controls.Add(this.TLE_Line2_Ascension);
            this.tabPage2.Controls.Add(this.TLE_Line2_Inclination);
            this.tabPage2.Controls.Add(this.TLE_Line2_SatelliteNumber);
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.label10);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // TLE_Line2_Checksum
            // 
            resources.ApplyResources(this.TLE_Line2_Checksum, "TLE_Line2_Checksum");
            this.TLE_Line2_Checksum.Name = "TLE_Line2_Checksum";
            // 
            // TLE_Line2_MeanMotion
            // 
            resources.ApplyResources(this.TLE_Line2_MeanMotion, "TLE_Line2_MeanMotion");
            this.TLE_Line2_MeanMotion.Name = "TLE_Line2_MeanMotion";
            // 
            // TLE_Line2_RevolutionNum
            // 
            resources.ApplyResources(this.TLE_Line2_RevolutionNum, "TLE_Line2_RevolutionNum");
            this.TLE_Line2_RevolutionNum.Name = "TLE_Line2_RevolutionNum";
            // 
            // TLE_Line2_MeanAnomaly
            // 
            resources.ApplyResources(this.TLE_Line2_MeanAnomaly, "TLE_Line2_MeanAnomaly");
            this.TLE_Line2_MeanAnomaly.Name = "TLE_Line2_MeanAnomaly";
            // 
            // TLE_Line2_PerigeeArg
            // 
            resources.ApplyResources(this.TLE_Line2_PerigeeArg, "TLE_Line2_PerigeeArg");
            this.TLE_Line2_PerigeeArg.Name = "TLE_Line2_PerigeeArg";
            // 
            // TLE_Line2_Eccentricity
            // 
            resources.ApplyResources(this.TLE_Line2_Eccentricity, "TLE_Line2_Eccentricity");
            this.TLE_Line2_Eccentricity.Name = "TLE_Line2_Eccentricity";
            // 
            // TLE_Line2_Ascension
            // 
            resources.ApplyResources(this.TLE_Line2_Ascension, "TLE_Line2_Ascension");
            this.TLE_Line2_Ascension.Name = "TLE_Line2_Ascension";
            // 
            // TLE_Line2_Inclination
            // 
            resources.ApplyResources(this.TLE_Line2_Inclination, "TLE_Line2_Inclination");
            this.TLE_Line2_Inclination.Name = "TLE_Line2_Inclination";
            // 
            // TLE_Line2_SatelliteNumber
            // 
            resources.ApplyResources(this.TLE_Line2_SatelliteNumber, "TLE_Line2_SatelliteNumber");
            this.TLE_Line2_SatelliteNumber.Name = "TLE_Line2_SatelliteNumber";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // TLE_SatelliteName
            // 
            resources.ApplyResources(this.TLE_SatelliteName, "TLE_SatelliteName");
            this.TLE_SatelliteName.Name = "TLE_SatelliteName";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TLE_Line2);
            this.groupBox2.Controls.Add(this.TLE_Line1);
            this.groupBox2.Controls.Add(this.TLE_SatelliteName);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // TLE_Line2
            // 
            resources.ApplyResources(this.TLE_Line2, "TLE_Line2");
            this.TLE_Line2.Name = "TLE_Line2";
            // 
            // TLE_Line1
            // 
            resources.ApplyResources(this.TLE_Line1, "TLE_Line1");
            this.TLE_Line1.Name = "TLE_Line1";
            // 
            // EpochStartDateTime
            // 
            resources.ApplyResources(this.EpochStartDateTime, "EpochStartDateTime");
            this.EpochStartDateTime.Name = "EpochStartDateTime";
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage6);
            resources.ApplyResources(this.tabControl2, "tabControl2");
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.boxSatellite);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnSiteSave);
            this.tabPage4.Controls.Add(this.btnSiteRemove);
            this.tabPage4.Controls.Add(this.btnSiteNew);
            this.tabPage4.Controls.Add(this.boxEarthLocation);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnSiteSave
            // 
            resources.ApplyResources(this.btnSiteSave, "btnSiteSave");
            this.btnSiteSave.Name = "btnSiteSave";
            this.btnSiteSave.UseVisualStyleBackColor = true;
            this.btnSiteSave.Click += new System.EventHandler(this.btnSiteSave_Click);
            // 
            // btnSiteRemove
            // 
            resources.ApplyResources(this.btnSiteRemove, "btnSiteRemove");
            this.btnSiteRemove.Name = "btnSiteRemove";
            this.btnSiteRemove.UseVisualStyleBackColor = true;
            this.btnSiteRemove.Click += new System.EventHandler(this.BTNSiteRemove_Click);
            // 
            // btnSiteNew
            // 
            resources.ApplyResources(this.btnSiteNew, "btnSiteNew");
            this.btnSiteNew.Name = "btnSiteNew";
            this.btnSiteNew.UseVisualStyleBackColor = true;
            this.btnSiteNew.Click += new System.EventHandler(this.BTNSiteNew_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.btnGetSatInfo);
            this.tabPage5.Controls.Add(this.btnRemoveFav);
            this.tabPage5.Controls.Add(this.Fav_ListBox1);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // btnGetSatInfo
            // 
            resources.ApplyResources(this.btnGetSatInfo, "btnGetSatInfo");
            this.btnGetSatInfo.Name = "btnGetSatInfo";
            this.btnGetSatInfo.UseVisualStyleBackColor = true;
            // 
            // btnRemoveFav
            // 
            resources.ApplyResources(this.btnRemoveFav, "btnRemoveFav");
            this.btnRemoveFav.Name = "btnRemoveFav";
            this.btnRemoveFav.UseVisualStyleBackColor = true;
            this.btnRemoveFav.Click += new System.EventHandler(this.btnRemoveFav_Click);
            // 
            // Fav_ListBox1
            // 
            resources.ApplyResources(this.Fav_ListBox1, "Fav_ListBox1");
            this.Fav_ListBox1.Items.AddRange(new object[] {
            resources.GetString("Fav_ListBox1.Items")});
            this.Fav_ListBox1.Name = "Fav_ListBox1";
            this.Fav_ListBox1.Sorted = true;
            this.Fav_ListBox1.SelectedIndexChanged += new System.EventHandler(this.listBoxFavorites_SelectedIndexChanged);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.GenerateScriptButton);
            this.tabPage6.Controls.Add(this.groupBox1);
            this.tabPage6.Controls.Add(this.groupBox8);
            this.tabPage6.Controls.Add(this.boxPasses);
            resources.ApplyResources(this.tabPage6, "tabPage6");
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // GenerateScriptButton
            // 
            resources.ApplyResources(this.GenerateScriptButton, "GenerateScriptButton");
            this.GenerateScriptButton.Name = "GenerateScriptButton";
            this.GenerateScriptButton.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.radioButton2);
            this.groupBox8.Controls.Add(this.radioButton1);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // radioButton2
            // 
            resources.ApplyResources(this.radioButton2, "radioButton2");
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            resources.ApplyResources(this.radioButton1, "radioButton1");
            this.radioButton1.Checked = true;
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.TabStop = true;
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // boxPasses
            // 
            resources.ApplyResources(this.boxPasses, "boxPasses");
            this.boxPasses.FormattingEnabled = true;
            this.boxPasses.Name = "boxPasses";
            this.boxPasses.SelectedIndexChanged += new System.EventHandler(this.listBoxPasses_SelectedIndexChanged);
            this.boxPasses.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.boxPasses_MouseDoubleClick);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbLeadTime_ms);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label49);
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.txtWindowSize);
            this.groupBox3.Controls.Add(this.label44);
            this.groupBox3.Controls.Add(this.labelEventInterval);
            this.groupBox3.Controls.Add(this.EventInterval);
            this.groupBox3.Controls.Add(this.labelEpochInfo);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // tbLeadTime_ms
            // 
            resources.ApplyResources(this.tbLeadTime_ms, "tbLeadTime_ms");
            this.tbLeadTime_ms.Name = "tbLeadTime_ms";
            this.tbLeadTime_ms.TextChanged += new System.EventHandler(this.LeadTime_ms_TextChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label49
            // 
            resources.ApplyResources(this.label49, "label49");
            this.label49.Name = "label49";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.dateTimePicker_UTC_End);
            this.groupBox6.Controls.Add(this.dateTimePicker_UTC_Start);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // dateTimePicker_UTC_End
            // 
            this.dateTimePicker_UTC_End.Checked = false;
            resources.ApplyResources(this.dateTimePicker_UTC_End, "dateTimePicker_UTC_End");
            this.dateTimePicker_UTC_End.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_UTC_End.Name = "dateTimePicker_UTC_End";
            this.dateTimePicker_UTC_End.ShowUpDown = true;
            this.dateTimePicker_UTC_End.Value = new System.DateTime(2001, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_UTC_End.ValueChanged += new System.EventHandler(this.dateTimePicker_LOCAL_End_ValueChanged);
            // 
            // dateTimePicker_UTC_Start
            // 
            this.dateTimePicker_UTC_Start.Checked = false;
            resources.ApplyResources(this.dateTimePicker_UTC_Start, "dateTimePicker_UTC_Start");
            this.dateTimePicker_UTC_Start.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_UTC_Start.Name = "dateTimePicker_UTC_Start";
            this.dateTimePicker_UTC_Start.ShowUpDown = true;
            this.dateTimePicker_UTC_Start.Value = new System.DateTime(2001, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_UTC_Start.ValueChanged += new System.EventHandler(this.dateTimePicker_LOCAL_Start_ValueChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dateTimePicker_LOCAL_Start);
            this.groupBox4.Controls.Add(this.dateTimePicker_LOCAL_End);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // dateTimePicker_LOCAL_Start
            // 
            this.dateTimePicker_LOCAL_Start.Checked = false;
            resources.ApplyResources(this.dateTimePicker_LOCAL_Start, "dateTimePicker_LOCAL_Start");
            this.dateTimePicker_LOCAL_Start.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_LOCAL_Start.Name = "dateTimePicker_LOCAL_Start";
            this.dateTimePicker_LOCAL_Start.ShowUpDown = true;
            this.dateTimePicker_LOCAL_Start.Value = new System.DateTime(2001, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_LOCAL_Start.ValueChanged += new System.EventHandler(this.dateTimePicker_UTC_Start_ValueChanged);
            // 
            // dateTimePicker_LOCAL_End
            // 
            this.dateTimePicker_LOCAL_End.Checked = false;
            resources.ApplyResources(this.dateTimePicker_LOCAL_End, "dateTimePicker_LOCAL_End");
            this.dateTimePicker_LOCAL_End.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_LOCAL_End.Name = "dateTimePicker_LOCAL_End";
            this.dateTimePicker_LOCAL_End.ShowUpDown = true;
            this.dateTimePicker_LOCAL_End.Value = new System.DateTime(2001, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_LOCAL_End.ValueChanged += new System.EventHandler(this.dateTimePicker_UTC_End_ValueChanged);
            // 
            // txtWindowSize
            // 
            resources.ApplyResources(this.txtWindowSize, "txtWindowSize");
            this.txtWindowSize.Name = "txtWindowSize";
            this.txtWindowSize.TextChanged += new System.EventHandler(this.txtWindowSize_TextChanged);
            this.txtWindowSize.Leave += new System.EventHandler(this.txtWindowSize_Leave);
            this.txtWindowSize.Validating += new System.ComponentModel.CancelEventHandler(this.txtWindowSize_Validating);
            this.txtWindowSize.Validated += new System.EventHandler(this.txtWindowSize_Validated);
            // 
            // chkIgnoreStartTimer
            // 
            resources.ApplyResources(this.chkIgnoreStartTimer, "chkIgnoreStartTimer");
            this.chkIgnoreStartTimer.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkIgnoreStartTimer.Name = "chkIgnoreStartTimer";
            this.chkIgnoreStartTimer.UseVisualStyleBackColor = true;
            this.chkIgnoreStartTimer.CheckedChanged += new System.EventHandler(this.chkIgnoreStartTimer_CheckedChanged);
            // 
            // cbVisibleOnly
            // 
            resources.ApplyResources(this.cbVisibleOnly, "cbVisibleOnly");
            this.cbVisibleOnly.Checked = true;
            this.cbVisibleOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVisibleOnly.Name = "cbVisibleOnly";
            this.cbVisibleOnly.UseVisualStyleBackColor = true;
            this.cbVisibleOnly.CheckedChanged += new System.EventHandler(this.cbVisibleOnly_CheckedChanged);
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Maximum = 4000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // btnBuildScript
            // 
            resources.ApplyResources(this.btnBuildScript, "btnBuildScript");
            this.btnBuildScript.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnBuildScript.Name = "btnBuildScript";
            this.btnBuildScript.UseVisualStyleBackColor = true;
            this.btnBuildScript.Click += new System.EventHandler(this.btnBuildScript_Click);
            // 
            // btnPause
            // 
            resources.ApplyResources(this.btnPause, "btnPause");
            this.btnPause.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPause.Name = "btnPause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.btnRun, "btnRun");
            this.btnRun.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRun.Name = "btnRun";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // chkIgnoreIntervalTimer
            // 
            resources.ApplyResources(this.chkIgnoreIntervalTimer, "chkIgnoreIntervalTimer");
            this.chkIgnoreIntervalTimer.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkIgnoreIntervalTimer.Name = "chkIgnoreIntervalTimer";
            this.chkIgnoreIntervalTimer.UseVisualStyleBackColor = true;
            this.chkIgnoreIntervalTimer.CheckedChanged += new System.EventHandler(this.chkIgnoreIntervalTimer_CheckedChanged);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.chkIgnoreIntervalTimer);
            this.Controls.Add(this.btnBuildScript);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.chkIgnoreStartTimer);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.cbVisibleOnly);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.EpochStartDateTime);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Form1";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.boxSatellite.ResumeLayout(false);
            this.boxSatellite.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.boxEarthLocation.ResumeLayout(false);
            this.boxEarthLocation.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox boxSatellite;
        private System.Windows.Forms.ComboBox listBoxSatelliteName;
        private System.Windows.Forms.TextBox SatelliteInfo_NoradNumber;
        private System.Windows.Forms.TextBox SatelliteInfo_Designator;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox boxEarthLocation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox listLocationName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelEventInterval;
        private System.Windows.Forms.TextBox EventInterval;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button EventScriptSave;
        private System.Windows.Forms.TextBox EventScriptFilename;
        private System.Windows.Forms.ListBox listEventScript;
        private System.Windows.Forms.TextBox LocationElevation;
        private System.Windows.Forms.TextBox LocationLongitude;
        private System.Windows.Forms.TextBox LocationLatitude;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListBox ConsoleListBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label TLE_Line1_Checksum;
        private System.Windows.Forms.Label TLE_Line1_ElementSetNumber;
        private System.Windows.Forms.Label TLE_Line1_EphemerisType;
        private System.Windows.Forms.Label TLE_Line1_BSTAR;
        private System.Windows.Forms.Label TLE_Line1_MeanMotionSecondDt;
        private System.Windows.Forms.Label TLE_Line1_MeanMotionFirstDt;
        private System.Windows.Forms.Label TLE_Line1_EpochDayPart;
        private System.Windows.Forms.Label TLE_Line1_EpochYear;
        private System.Windows.Forms.Label TLE_Line1_LaunchPiece;
        private System.Windows.Forms.Label TLE_Line1_LaunchNum;
        private System.Windows.Forms.Label TLE_Line1_LaunchYear;
        private System.Windows.Forms.Label TLE_Line1_Classification;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label EpochYear;
        private System.Windows.Forms.Label labelXXX;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label TLE_SatelliteName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label TLE_Line2;
        private System.Windows.Forms.Label TLE_Line1;
        private System.Windows.Forms.Label TLE_Line1_SatelliteNumber;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label TLE_Line2_Checksum;
        private System.Windows.Forms.Label TLE_Line2_MeanMotion;
        private System.Windows.Forms.Label TLE_Line2_RevolutionNum;
        private System.Windows.Forms.Label TLE_Line2_MeanAnomaly;
        private System.Windows.Forms.Label TLE_Line2_PerigeeArg;
        private System.Windows.Forms.Label TLE_Line2_Eccentricity;
        private System.Windows.Forms.Label TLE_Line2_Ascension;
        private System.Windows.Forms.Label TLE_Line2_Inclination;
        private System.Windows.Forms.Label TLE_Line2_SatelliteNumber;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label SatelliteInfo_Orbital_Status_Code;
        private System.Windows.Forms.Label SatelliteInfo_Radar_Cross_Section_meters2;
        private System.Windows.Forms.Label SatelliteInfo_Perigee_Altitude_kilometers;
        private System.Windows.Forms.Label SatelliteInfo_Apogee_Altitude_kilometers;
        private System.Windows.Forms.Label SatelliteInfo_Inclination_degrees;
        private System.Windows.Forms.Label SatelliteInfo_OrbitalPeriod;
        private System.Windows.Forms.Label SatelliteInfo_DecayDate;
        private System.Windows.Forms.Label SatelliteInfo_LaunchSite;
        private System.Windows.Forms.Label SatelliteInfo_LaunchDate;
        private System.Windows.Forms.Label SatelliteInfo_SourceOrOwnership;
        private System.Windows.Forms.Label SatelliteInfo_OperationalStatusCode;
        private System.Windows.Forms.Label SatelliteInfo_PayloadFlag;
        private System.Windows.Forms.Label SatelliteInfo_MultipleNameFlag;
        private System.Windows.Forms.Label EpochStartDateTime;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label labelEpochInfo;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.ListBox Fav_ListBox1;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.ListBox boxPasses;
        private System.Windows.Forms.Button btnAddToFav;
        private System.Windows.Forms.Button btnRemoveFav;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DateTimePicker dateTimePicker_LOCAL_Start;
        private System.Windows.Forms.DateTimePicker dateTimePicker_LOCAL_End;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.DateTimePicker dateTimePicker_UTC_End;
        private System.Windows.Forms.DateTimePicker dateTimePicker_UTC_Start;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbVisibleOnly;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox tbMXPPORT;
        private System.Windows.Forms.TextBox tbMXPMASK;
        internal System.Windows.Forms.ProgressBar progressBar1;
        private IPAddressControlLib.IPAddressControl ipAddressControl1;
        private System.Windows.Forms.CheckBox chkIgnoreStartTimer;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.TextBox txtWindowSize;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.TextBox tbLeadTime_ms;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox tbLocationName;
        private System.Windows.Forms.Button btnSiteRemove;
        private System.Windows.Forms.Button btnSiteNew;
        private System.Windows.Forms.Button btnSiteSave;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnBuildScript;
        private System.Windows.Forms.CheckBox chkIgnoreIntervalTimer;
        private System.Windows.Forms.Button btnGetSatInfo;
        private System.Windows.Forms.Button GenerateScriptButton;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.MaskedTextBox tbStartTimeAzEl;
        private System.Windows.Forms.MaskedTextBox tbProgAutoTrack;
        private System.Windows.Forms.MaskedTextBox tbSampleRate;
        private System.Windows.Forms.MaskedTextBox tbPolarity;
        private System.Windows.Forms.MaskedTextBox tbSearchType;
        private System.Windows.Forms.MaskedTextBox tbTrackingFreq;
        private System.Windows.Forms.MaskedTextBox tbStopTimeAzEl;
        public System.Windows.Forms.MaskedTextBox tbThreshold;
        private System.Windows.Forms.MaskedTextBox tbMaxPathDev;
        private System.Windows.Forms.MaskedTextBox tbTrackType;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.RadioButton rbTilt;
        private System.Windows.Forms.RadioButton rbOverhead;
        private System.Windows.Forms.RadioButton rbStandard;
        private System.Windows.Forms.CheckBox cbCanted;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem ClearConsole;
    }
}

