using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace DotNet.ThirdParty
{
    class SpdxLicenseEntry
    {
        [JsonProperty("isDeprecatedLicenseId")]
        public bool IsDeprecatedLicenseId { get; set; }

        [JsonProperty("licenseText")]
        public string LicenseText { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("licenseId")]
        public string LicenseId { get; set; }

        [JsonProperty("seeAlso")]
        public Collection<string> SeeAlso { get; set; }

        [JsonProperty("isOsiApproved")]
        public bool IsOsiApproved { get; set; }
    }
}
