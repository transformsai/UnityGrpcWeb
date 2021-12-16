using System;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace TransformsAI.Unity.Grpc.Web
{
    public static class UnityGrpcWeb
    {
        public static GrpcChannel MakeChannel(
            string address,
            GrpcWebMode mode = GrpcWebMode.GrpcWebText,
            GrpcChannelOptions options = null,
            bool disposeOnExitPlayMode = true,
            bool useLogger = false) =>
            MakeChannel(new Uri(address), mode, options, disposeOnExitPlayMode, useLogger);

        public static GrpcChannel MakeChannel(
            Uri address,
            GrpcWebMode mode = GrpcWebMode.GrpcWebText,
            GrpcChannelOptions options = null,
            bool disposeOnExitPlayMode = true,
            bool useLogger = false)
        {
            options ??= new GrpcChannelOptions();

            if (options.HttpHandler == null && options.HttpClient == null) options.HttpHandler = UnityGrpcWebHandler.Create(mode);
            if (useLogger && options.LoggerFactory == null) options.LoggerFactory = new UnityLoggerFactory();

            var channel = GrpcChannel.ForAddress(address, options);
#if UNITY_EDITOR
            
            if (disposeOnExitPlayMode)
            {
                UnityEditor.EditorApplication.playModeStateChanged += change =>
                {
                    if (change == UnityEditor.PlayModeStateChange.ExitingPlayMode) channel.Dispose();
                };
            }
#endif
            return channel;
        }

    }
}
