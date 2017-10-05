///////////////////////////////////////////////////////////////
// InterThreadMessaging .cs - SatTrax
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

using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SatTraxGUI
{
    public sealed class InterThreadMessaging : ConcurrentQueue < string >, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public static InterThreadMessaging _instance;
        public object data;

        public static InterThreadMessaging Instance
        {
            get
            {
                if ( _instance == null )
                    _instance = new InterThreadMessaging();
                return _instance;
            }
        }

        public string Message
        {
            get { return GetMessage(); }
            set { SendMessage(value); }
        }

        public string AddSatname
        {
            set
            {
                Instance.Enqueue(value);
                Instance.NotifyPropertyChanged("AddSatName", value);
            }
        }

        public int ResetProgress
        {
            get { return 0; }
            set
            {
                data = value;
                Instance.NotifyPropertyChanged("RESET_StepProgressBar1", value);
//                Instance.NotifyCollectionChanged(NotifyCollectionChangedAction.Add, value);
            }
        }

        public int Progress
        {
            set
            {
                Instance.NotifyPropertyChanged("StepProgressBar1", value);
                Instance.NotifyCollectionChanged(NotifyCollectionChangedAction.Add, value);
            }
        }

        public string ProgressText
        {
            set
            {
                Instance.NotifyPropertyChanged("StepProgressBar1", value);
//                Instance.NotifyCollectionChanged(NotifyCollectionChangedAction.Reset, value);
            }
        }

        private InterThreadMessaging()
        {
//                CollectionChanged += (sender, e) =>
            //    {
            //        //this.SomeOtheMember = Messages.Where(c => c.Count_TB == true).Count();
            //        NotifyPropertyChanged("Queue Collection");
            //    };
            //PropertyChanged += (sender, e) =>
            //{
            //    //this.SomeOtheMember = Items.Where(c => c.Count_TB == true).Count();
            //    NotifyPropertyChanged("Queue Property");
            //};
        }

        internal static void SendMessage(string msg)
        {
            Instance.Enqueue(msg);
            Instance.NotifyPropertyChanged("Enqueue", msg);
        }

        internal static string GetMessage()
        {
            string message;
            //if (
            Instance.TryDequeue(out message);
            //)
            //Instance.NotifyPropertyChanged("Dequeue", message);
            return message;
        }

        #region Implementation of INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void NotifyCollectionChanged(NotifyCollectionChangedAction action, int i)
        {
            if ( CollectionChanged != null )
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, this, i));
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string p, object o)
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        #endregion
    }
}