namespace AvoCommLib
{
    namespace Protocols
    {
        namespace ADSAP2
        {
            public enum CommandTypes
            {
                SetCertificatesRequest = 1,
                ClearCertificatesRequest,
                PreauthorizeRequest,
                AuthorizeRequest,

                SetCertificatesResponse = 128 | SetCertificatesRequest,
                ClearCertificatesResponse = 128 | ClearCertificatesRequest,
                PreauthorizeResponse = 128 | PreauthorizeRequest,
                AuthorizeResponse = 128 | AuthorizeRequest
            }
        }
    }
}