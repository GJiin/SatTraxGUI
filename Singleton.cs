namespace SatTraxGUI
{
    /// <summary>
    ///     Description of Singleton
    /// </summary>
    public sealed class Singleton
    {
        private static readonly Singleton _instance = new Singleton();

        public static Singleton Instance
        {
            get { return _instance; }
        }

        private Singleton()
        {
        }
    }
}