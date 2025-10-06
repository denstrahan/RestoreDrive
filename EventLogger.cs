////////////////////////////////////////////////////////////////////////////////
//// Module Name:  EventLogger.cs                                           ////
//// Project:      Restore Drive Letter.                                    ////
//// Purpose:      A helper class to write to a custom event log.           ////
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EventLoggerHelper
{
    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////
    /// This class writes messages to our custom Event Log...                ///
    /// ////////////////////////////////////////////////////////////////////////
    /// </summary>
    public class EventLogger
    {
        public  EventLogger(string appName, string logName, long evtID)
        { 
            app     = appName;
            name    = logName;
            eventID = evtID;
        }

        private List<string>    strEventList;       // List collection of events

        private string          app;                // App name (i.e., RestoreDrive)
        private string          name;               // Log name (i.e., myApps/RestoreDrive)
        private long            eventID;            // Can be any number..
        private string          strEvents;          // string variable in which all events are concatenated...
        private EventLog        eventLog;           
        private EventInstance   eventInstance;

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// This method creates the event log source and the necessary       ///
        /// member variables...                                              ///
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>
        public void CreateEventLog()
        {
            app = Process.GetCurrentProcess().ProcessName;

            eventLog      = new EventLog(name, Environment.MachineName, app);
            eventInstance = new EventInstance(eventID, 0, EventLogEntryType.Information);

            // Check if the event source exists. If not create it...
            if (!EventLog.SourceExists(app))
            {
                EventLog.CreateEventSource(app, name);
            }

            eventLog.Source = app;
            eventLog.Log    = name;

            // Instantiate the string array...
            strEventList  = new List<string>(20);
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// This method adds to the event list which will later be written   ///
        /// out as one string.  Inherits the Add method from the List class. ///
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>                                                         
        /// <param name="strEvent"></param>

        public void Add(string strEvent) => strEventList.Add(strEvent);

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// This method concatenates all the events in the list into one     ///
        /// string and writes it out to the custom log...                    ///
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>
        public void WriteToEventLog()
        {
            foreach (string strEvent in strEventList)
            {
                // Concatenate all the events into a single string...
                strEvents += strEvent + Environment.NewLine;
            }
                
            // Write to our custom log...
            EventLog.WriteEvent(eventLog.Source, eventInstance, strEvents);

            // Re-initialize the string variables...
            strEvents = string.Empty;
            strEventList.Clear();
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// This method cleans up...                                         ///
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>
        public void CloseEventLog()
        {
            eventLog?.Close();
            eventLog?.Dispose();

            strEventList?.Clear();
            strEvents     = string.Empty;
            eventInstance = null;
        }
    }
}
