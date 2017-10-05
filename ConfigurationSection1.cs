using System.Configuration;

namespace SatTraxGUI
{
    /// <summary>
    ///     Configuration section ConfigurationSection1
    /// </summary>
    /// <remarks>
    ///     Assign properties to your child class that has the attribute
    ///     <c>[ConfigurationProperty]</c> to store said properties in the xml.
    /// </remarks>
    public sealed class ConfigurationSection1 : ConfigurationSection
    {
        private Configuration _Config;

        #region ConfigurationProperties

        /*
		 *  Uncomment the following section and add a Configuration Collection 
		 *  from the with the file named Class1Section.cs
		 */
        // /// <summary>
        // /// A custom XML section for an application's configuration file.
        // /// </summary>
        // [ConfigurationProperty("customSection", IsDefaultCollection = true)]
        // public Class1SectionCollection Class1Section
        // {
        // 	get { return (Class1SectionCollection) base["customSection"]; }
        // }

        /// <summary>
        ///     Collection of <c>Class1SectionElement(s)</c>
        ///     A custom XML section for an applications configuration file.
        /// </summary>
        [ConfigurationProperty("exampleAttribute", DefaultValue = "exampleValue")]
        public string ExampleAttribute
        {
            get { return (string) this["exampleAttribute"]; }
            set { this["exampleAttribute"] = value; }
        }

        #endregion

        /// <summary>
        ///     Private Constructor used by our factory method.
        /// </summary>
        private ConfigurationSection1()
        {
            // Allow this section to be stored in user.app. By default this is forbidden.
            SectionInformation.AllowExeDefinition =
                ConfigurationAllowExeDefinition.MachineToLocalUser;
        }

        #region Public Methods

        /// <summary>
        ///     Saves the configuration to the config file.
        /// </summary>
        public void Save()
        {
            _Config.Save();
        }

        #endregion

        #region Static Members

        /// <summary>
        ///     Gets the current applications &lt;Class1Section&gt; section.
        /// </summary>
        /// <param name="ConfigLevel">
        ///     The &lt;ConfigurationUserLevel&gt; that the config file
        ///     is retrieved from.
        /// </param>
        /// <returns>
        ///     The configuration file's &lt;Class1Section&gt; section.
        /// </returns>
        public static ConfigurationSection1 GetSection(ConfigurationUserLevel ConfigLevel)
        {
            /* 
             * This class is setup using a factory pattern that forces you to
             * name the section &lt;Class1Section&gt; in the config file.
             * If you would prefer to be able to specify the name of the section,
             * then remove this method and mark the constructor public.
             */
            var Config = ConfigurationManager.OpenExeConfiguration
                (ConfigLevel);
            ConfigurationSection1 oConfigurationSection1;

            oConfigurationSection1 =
                (ConfigurationSection1) Config.GetSection("ConfigurationSection1");
            if (oConfigurationSection1 == null)
            {
                oConfigurationSection1 = new ConfigurationSection1();
                Config.Sections.Add("ConfigurationSection1", oConfigurationSection1);
            }
            oConfigurationSection1._Config = Config;

            return oConfigurationSection1;
        }

        #endregion
    }
}