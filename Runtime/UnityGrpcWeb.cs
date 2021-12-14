using System;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace TransformsAI.UnityGrpcWeb
{
    public static class UnityGrpcWeb
    {
        public static GrpcChannel MakeChannel(
            string address,
            GrpcWebMode mode = GrpcWebMode.GrpcWebText,
            GrpcChannelOptions options = null,
            bool shutdownOnExitPlayMode = true,
            bool useLogger = false) =>
            MakeChannel(new Uri(address), mode, options, shutdownOnExitPlayMode, useLogger);

        public static GrpcChannel MakeChannel(
            Uri address,
            GrpcWebMode mode = GrpcWebMode.GrpcWebText,
            GrpcChannelOptions options = null,
            bool shutdownOnExitPlayMode = true,
            bool useLogger = false)
        {
            options ??= new GrpcChannelOptions();

            if (options.HttpHandler == null && options.HttpClient == null) options.HttpHandler = UnityGrpcWebHandler.Create(mode);
            if (useLogger && options.LoggerFactory == null) options.LoggerFactory = new UnityLoggerFactory();

#if UNITY_EDITOR
            var channel = GrpcChannel.ForAddress(address, options);

            if (shutdownOnExitPlayMode)
            {
                UnityEditor.EditorApplication.playModeStateChanged += change =>
                {
                    if (change == UnityEditor.PlayModeStateChange.ExitingPlayMode) channel.ShutdownAsync();
                };
            }
#endif
            return channel;
        }

    }
}
