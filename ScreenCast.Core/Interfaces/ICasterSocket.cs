using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Silgred.Shared.Models;

namespace Silgred.ScreenCast.Core.Interfaces
{
    public interface ICasterSocket
    {
        bool IsConnected { get; }
        IScreenCaster ScreenCaster { get; }
        IAudioCapturer AudioCapturer { get; }
        IClipboardService ClipboardService { get; }
        HubConnection Connection { get; }
        HubConnection MessagingHub { get; }
        IKeyboardMouseInput KeyboardMouseInput { get; }
        Task Connect(string host);
        Task Disconnect();
        Task GetSessionID();
        Task NotifyRequesterUnattendedReady(string requesterID);
        Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs);
        Task SendAudioSample(byte[] buffer, List<string> viewerIDs);
        Task SendClipboardText(string clipboardText, List<string> viewerIDs);
        Task SendConnectionFailedToViewers(List<string> viewerIDs);
        Task SendCursorChange(CursorInfo cursor, List<string> viewerIDs);
        Task SendDeviceInfo(string serviceID, string machineName, string deviceID);
        Task SendIceCandidateToBrowser(string candidate, int sdpMlineIndex, string sdpMid, string viewerConnectionID);
        Task SendMachineName(string machineName, string viewerID);
        Task SendRtcOfferToBrowser(string sdp, string viewerID);
        Task SendAudioEndpoint(byte[] endPoint);
        Task SendWebcamVideo(byte[] buffer, List<string> viewerIDs);

        Task SendScreenCapture(byte[] captureBytes, string viewerID, int left, int top, int width, int height,
            DateTime captureTime);

        Task SendScreenCount(int primaryScreenIndex, int screenCount, string viewerID);
        Task SendScreenSize(int width, int height, string viewerID);
        Task SendViewerRemoved(string viewerID);
        void ApplyConnectionHandlers();
        void ClipboardService_ClipboardTextChanged(object sender, string clipboardText);
    }
}