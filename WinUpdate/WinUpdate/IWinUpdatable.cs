using System;
using System.Reflection;

namespace WinUpdate
{
    public interface IWinUpdatable
    {
        /// <summary>
        /// The name of your application as you want it displayed on the update window.
        /// </summary>
        string ApplicationName { get; }
        /// <summary>
        /// An identifier string to use to identify your application in the update.xml
        /// Should be the same as your appId in the update.xml
        /// </summary>
        string ApplicationId { get; }
        /// <summary>
        /// The application executable folder path (the folder path where the exe file is placed)
        /// </summary>
        string ApplicationExeFolderPath { get; }   
        /// <summary>
        /// The current assembly
        /// </summary>
        Assembly ApplicationAssembly { get; }
        /// <summary>
        /// The location of the update.xml on the server
        /// </summary>
        Uri UpdateXmlLocation { get; }
        /// <summary>
        /// The program main window.
        /// </summary>
        IContext Context { get; }
    }
}
