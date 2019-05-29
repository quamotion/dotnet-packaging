namespace DotNet.ThirdParty
{
    class ThirdPartyNotice
    {
        public string PackageName { get; set; }
        public string PackageUrl { get; set; }
        public string Copyright { get; set; }
        public License License { get; set; }

        public override string ToString()
        {
            return $"{this.PackageName} - {this.License.Name}";
        }
    }
}
