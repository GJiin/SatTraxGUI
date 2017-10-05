/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 12/29/2013
 * Time: 12:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Configuration;

namespace SatTraxGUI
{
    /// <summary>
    ///     A collection of ConfigurationElementCollection1(s).
    /// </summary>
    public sealed class ConfigurationElementCollection1 : ConfigurationElementCollection
    {
        /// <summary>
        ///     Adds a ConfigurationElementCollection1 to the configuration file.
        /// </summary>
        /// <param name="element">The ConfigurationElementCollection1 to add.</param>
        public void Add(ConfigurationElement1 element)
        {
            BaseAdd(element);
        }


        /// <summary>
        ///     Creates a new ConfigurationElementCollection1.
        /// </summary>
        /// <returns>A new <c>ConfigurationElementCollection1</c></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigurationElement1();
        }


        /// <summary>
        ///     Gets the key of an element based on it's Id.
        /// </summary>
        /// <param name="element">Element to get the key of.</param>
        /// <returns>The key of <c>element</c>.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ( (ConfigurationElement1) element ).Name;
        }


        /// <summary>
        ///     Removes a ConfigurationElementCollection1 with the given name.
        /// </summary>
        /// <param name="name">The name of the ConfigurationElementCollection1 to remove.</param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }

        #region Properties

        /// <summary>
        ///     Gets the CollectionType of the ConfigurationElementCollection.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }


        /// <summary>
        ///     Gets the Name of Elements of the collection.
        /// </summary>
        protected override string ElementName
        {
            get { return "ConfigurationElementCollection1"; }
        }


        /// <summary>
        ///     Retrieve and item in the collection by index.
        /// </summary>
        public ConfigurationElement1 this[int index]
        {
            get { return (ConfigurationElement1) BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        #endregion
    }
}