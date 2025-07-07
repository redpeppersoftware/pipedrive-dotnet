using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Pipedrive.Tests.Integration
{
    public class Helper
    {
        private static readonly IConfiguration _config = new ConfigurationBuilder()
            .AddUserSecrets<Helper>()
            .Build();

        public static Uri ApiUrl
        {
            get { return _apiUrl.Value; }
        }

        public static string ApiToken
        {
            get { return _config["PIPEDRIVE_APITOKEN"]; }
        }

        private static readonly Lazy<Uri> _apiUrl = new (() =>
        {
            var uri = _config["PIPEDRIVE_URL"];

            if (uri != null)
            {
                return new Uri(uri);
            }

            return null;
        });

        public static IPipedriveClient GetAuthenticatedClient()
        {
            return new PipedriveClient(new ProductHeaderValue("PipedriveTests"), ApiUrl)
            {
                Credentials = new Credentials(ApiToken, AuthenticationType.ApiToken)
            };
        }

        public static Stream LoadFixture(string fileName)
        {
            var key = "Pipedrive.Tests.Integration.Fixtures." + fileName;
            var stream = typeof(Helper).GetTypeInfo().Assembly.GetManifestResourceStream(key);
            if (stream == null)
            {
                throw new InvalidOperationException(
                    "The file '" + fileName + "' was not found as an embedded resource in the assembly. Failing the test...");
            }

            return stream;
        }
    }
}
