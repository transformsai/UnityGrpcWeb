using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client.Web;
using UnityEngine;

namespace TransformsAI.Unity.Grpc.Web
{
    public static class UnityGrpcWebHandler
    {
        public static HttpMessageHandler Create(GrpcWebMode mode = GrpcWebMode.GrpcWebText)
        {

#if UNITY_WEBGL
            if (!Application.isEditor)
            {
                var innerHandler = new TransformsAI.Unity.WebGL.UnityWebGlHttpHandler();
                var fixedHandler = new HandlerFix(mode, innerHandler);
                return new GrpcWebHandler(mode, fixedHandler);
            }
#endif
            var fixedStandaloneHandler = new HandlerFix(mode, new HttpClientHandler());
            return new GrpcWebHandler(mode, fixedStandaloneHandler);
        }


        // Todo: temporary fix to address issue here: https://github.com/grpc/grpc-dotnet/issues/1520#issuecomment-991853151
        class HandlerFix : DelegatingHandler
        {
            private readonly GrpcWebMode _mode;
            public HandlerFix(GrpcWebMode mode, HttpMessageHandler innerHandler) : base(innerHandler) => _mode = mode;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (_mode == GrpcWebMode.GrpcWebText)
                    request.Headers.TryAddWithoutValidation("Accept", "application/grpc-web-text");

                return base.SendAsync(request, cancellationToken);
            }
        }
    }    
}
