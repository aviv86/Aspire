using CommunityToolkit.Aspire.Hosting.RavenDB;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

//----------------------
// with license


//const string licensePath = "C:\\work\\license.json";
//var license = File.ReadAllText(licensePath);

//serverSettings.WithLicense(license, eulaAccepted: true);


//----------------------
// Secured

const string ServerDomain = "https://a.aviv-domain.ravendb.community";
const string ContainerSecurityPath = "/etc/ravendb/security";
const string ContainerCertPath = $"{ContainerSecurityPath}/A/cluster.server.certificate.aviv-domain.pfx";
const string HostSecurityPath = "C:/work/setup_package";
const string InternalSecureUrl = "https://0.0.0.0:443";

const string HostClientCertPath = $"{HostSecurityPath}/admin.client.certificate.aviv-domain.pfx";

var serverSettings = RavenDBServerSettings.SecuredWithLetsEncrypt(
    domainUrl: ServerDomain,
    certificatePath: ContainerCertPath,
    certificatePassword: null,
    serverUrl: InternalSecureUrl,
    clientCertificatePath: HostClientCertPath
);

var ravendb = builder.AddRavenDB("ravendb", serverSettings)
    .WithDataVolume("data")
    .WithBindMount(HostSecurityPath, ContainerSecurityPath, isReadOnly: false);


//----------------------
// Unsecured

//var serverSettings = RavenDBServerSettings.Unsecured();
//var ravendb = builder.AddRavenDB("ravendb", serverSettings);

ravendb.AddDatabase("ravenDatabase63", ensureCreated: true);

builder.AddProject<CommunityToolkit_Aspire_Hosting_RavenDB_ApiService>("apiservice")
    .WithReference(ravendb)
    .WaitFor(ravendb);

 builder.Build().Run();
