using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Silgred.ScreenCast.Core.Communication;
using Silgred.ScreenCast.Core.Enums;
using Silgred.ScreenCast.Core.Models;
using Silgred.Shared.Models;

namespace Silgred.ScreenCast.Core.Interfaces
{
    public interface IConductor
    {
        Dictionary<string, string> ArgDict { get; set; }
        CasterSocket CasterSocket { get; }
        string DeviceID { get; }
        string Host { get; }
        bool IsDebug { get; }
        AppMode Mode { get; }
        string OrganizationName { get; }
        string RequesterID { get; }
        string ServiceID { get; }
        ConcurrentDictionary<string, Viewer> Viewers { get; }
        event EventHandler<ScreenCastRequest> ScreenCastRequested;
        event EventHandler<string> SessionIDChanged;
        event EventHandler<Viewer> ViewerAdded;
        event EventHandler<string> ViewerRemoved;
        event EventHandler<(string username, string message)> MessageReceived;
        Task Connect();
        void InvokeScreenCastRequested(ScreenCastRequest viewerIdAndRequesterName);
        void InvokeSessionIDChanged(string sessionID);
        void InvokeViewerAdded(Viewer viewer);
        void InvokeViewerRemoved(string viewerID);
        void InvokeMessageReceived(string userName, string message);
        void ProcessArgs(string[] args);
    }
}