# NeoLauncher — Static Analysis and Reverse Engineering

> **Status:** static analysis / client-side reverse engineering.
>
> **Objective:** document the artifacts, architecture, endpoints, build/distribution logic, and other information recovered from `NeoLauncher-win-Setup.exe`.
>
> **Scope:** this repository documents the client and publicly observable interfaces. It does not claim to contain or reconstruct Neo's private server-side source code.
>
> **Safety:** no credentials, tokens, cookies, passwords, or OAuth secrets are included in this README.

## 1. Summary

A Windows installer named:

```text
NeoLauncher-win-Setup.exe
```

The installer contains an application named **Neo Launcher**.

The application is a combination of:

- **.NET 10 / C#**
- **WinUI / WebView2**
- **Tauri 2.x** for the frontend
- A custom HTTP service architecture
- **BuildPatchServices** for distribution/installation of builds
- OAuth and session management
- Friends/XMPP services
- Fortnite/loadout services
- Optional telemetry
- Launcher updates
- An external web store

The analysis recovered a large portion of the client's C# code and identified the domains, routes, and download flow used by the launcher.

---

# 2. File analyzed

## Installer

```text
NeoLauncher-win-Setup.exe
```

SHA-256:

```text
f76baae31d50971e4c2dac9d0146a89a1d37cf2c5bde98954035954234c03689
```

El paquete uses **Velopack**.

Archive:

```text
NeoLauncher.nuspec
```

Relevant contents:

```xml
<id>3dvuut</id>
<title>Neo</title>
<description>Neo</description>
<authors>Neo</authors>
<version>1.1.0</version>
<channel>win</channel>
<mainExe>NeoLauncher.exe</mainExe>
<os>win</os>
<rid>win</rid>
<runtimeDependencies>webview2</runtimeDependencies>
<shortcutLocations>Desktop,StartMenuRoot</shortcutLocations>
<shortcutAmuid>velopack.NeoLauncher</shortcutAmuid>
```

The main executable declared by the package is:

```text
NeoLauncher.exe
```

---

# 3. Extraction

The installer was extracted to:

```text
~/lanzado
```

The main DLL found was:

```text
~/lanzado/lib/app/NeoLauncher.dll
```

The following libraries were also found:

```text
BuildPatchServices.dll
```

along with the dependencies required by WebView2/.NET.

---

# 4. Platform and runtime

The main DLL is:

```text
PE32+ .NET x64
```

Assembland:

```text
NeoLauncher
```

Assembly version:

```text
1.0.0.0
```

No PublicKeyToken is present.

The reconstructed project indicatestes:

```xml
<TargetFramework>net10.0</TargetFramework>
<PlatformTarget>x64</PlatformTarget>
<LangVersion>14.0</LangVersion>
<AllowUnsafeBlocks>True</AllowUnsafeBlocks>
<CheckForOverflowUnderflow>False</CheckForOverflowUnderflow>
```

This corresponds to:

```text
.NET 10
x64
C# 14
```

---

# 5. Decompilation

It was used:

```bash
ilspycmd -p -o ~/NeoLauncher_SRC ~/lanzado/lib/app/NeoLauncher.dll
```

Version used:

```text
ilspycmd 11.0.0.9375
```

El SDK de .NET se encontraba en:

```text
~/.dotnet
```

The reconstructed code was written to:

```text
~/NeoLauncher_SRC
```

Para BuildPatchServices:

```bash
ilspycmd -p -o ~/BuildPatchServices_SRC ~/lanzado/lib/app/BuildPatchServices.dll
```

Result:

```text
~/BuildPatchServices_SRC
```

---

# 6. PDB and compilation tracks

The DLL contains CodeView/RSDS information.

PDB esperado:

```text
C:\Users\Ender\Documents\GitHub\NeoLauncher\NeoLauncher\obj\x64\Release\net10.0-windows10.0.19041.0\win-x64\NeoLauncher.pdb
```

This provides a strong clue about the build environment:

```text
Usuario: Ender
Proyecto: NeoLauncher
Route: Documents\GitHub\NeoLauncher
Configuration: x64 Release
Target: net10.0-windows10.0.19041.0
RID: win-x64
```

RSDS:

```text
1bab27bb20db4ecdb1c6ede6cc8c404e
```

Age:

```text
1
```

There is also reproducible compilation information.

**The PDB was not included in the analyzed package.**

---

# 7. AssemblyInfo

The reconstructed code contains:

```csharp
[assembland: AssemblyCompany("Neo")]
[assembland: AssemblyInformationalVersion("1.0.0+f91d8131e7ff895e1786701afeb4184404b45831")]
[assembland: AssemblyProduct("Neo Launcher")]
[assembland: AssemblyMetadata("Microsoft.Windows.CsWin32", "0.3.269+368685089b.RR")]
```

The string:

```text
f91d8131e7ff895e1786701afeb4184404b45831
```

appears to be a Git commit hash.

Exact searches for this hash did not identify a corresponding public repository.

Therefore:

```text
hash found ≠ repositorio público confirmed
```

---

# 8. Reconstructed Project

The reconstructed `.csproj` contains:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblyName>NeoLauncher</AssemblyName>
    <GenerateAssemblyInfo>False</GenerateAssemblyInfo>
    <TargetFramework>net10.0</TargetFramework>
    <PlatformTarget>x64</PlatformTarget>
  </PropertyGroup>

  <PropertyGroup>
    <LangVersion>14.0</LangVersion>
    <AllowUnsafeBlocks>True</AllowUnsafeBlocks>
    <CheckForOverflowUnderflow>False</CheckForOverflowUnderflow>
  </PropertyGroup>

  <PropertyGroup>
    <ApplicationIcon>app.ico</ApplicationIcon>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <RootNamespace />
  </PropertyGroup>
</Project>
```

This file was **reconstructed by ILSpy** and should not be considered identical to the original `.csproj`.

---

# 9. General architecture

Approximate architecture:

```text
                         Neo Launcher
                              |
             +----------------+----------------+
             |                                 |
       .NET / WinUI                         Tauri
             |                                 |
       services C#                      React/Web frontend
             |
     +-------+-------+-------+-------+
     |       |       |       |       |
 Account Launcher Friends Fortnite Prism
     |       |       |       |       |
     +-------+-------+-------+-------+
                     |
                  Neo API
```

El frontend uses internamente:

```text
window.__TAURI_INTERNALS__.invoke
```

Esto confirma la integración con Tauri.

---

# 10. Frontend

The package contains a web frontend under:

```text
~/lanzado/lib/app/web/assets/
```

References to Tauri 2.x were found.

También aparecen referencias to:

```text
react.dev
```

y otros recursos web estándar.

URLs encontradas directamente en el frontend:

```text
https://store.neofn.dev
https://discord.gg/xSWZqs2Wbc
```

No NeoLauncher GitHub URL was found in the frontend.

---

# 11. Namespaces y services C#

Namespaces/services recuperados:

```text
NeoLauncher.Services.WebHost.NeoWebBridge
NeoLauncher.Services.WebHost.NeoWebEnvironment

NeoLauncher.Services.Updates.NeoUpdateSource
NeoLauncher.Services.Updates.UpdateService

NeoLauncher.Services.Launcher.AnalyticsService
NeoLauncher.Services.Launcher.LauncherService
NeoLauncher.Services.Launcher.LightswitchService

NeoLauncher.Services.Prism.PrismService

NeoLauncher.Services.Account.AccountService

NeoLauncher.Services.Fortnite.LoadoutService

NeoLauncher.Services.Friends.NeoPresenceService
NeoLauncher.Services.Friends.NeoXmppClient

NeoLauncher.Services.Game.GameLauncher
NeoLauncher.Services.Game.GameService
NeoLauncher.Services.Game.InstallService
NeoLauncher.Services.Game.LibraryService

NeoLauncher.Views.WebShellView
NeoLauncher.Views.NativeFriendContextMenuView

NeoLauncher.Services.SystemCapability
```

The following also exist:

```text
Program
App
```

y clases auxiliares/modelos.

---

# 12. NeoWebBridge

One of the central components is:

```text
NeoWebBridge
```

Recovered methods:

```text
LoginDiscordAsync
ExchangeDiscordOAuthCodeAsync
CurrentSessionAsync
BuildSessionAsync
CompleteAccountSetupAsync
GetLauncherNewsAsync
GetNewsAsync
GetServicesStateAsync
CheckUsernameAvailableAsync
GetBuildsAsync
RefreshBuildsCacheAsync
ConnectNeoPresenceAsync
FetchNeoFriendsAsync
SearchNeoAccountsAsync
NeoFriendActionAsync
GetFriendsAccessTokenAsync
GetJsonAsync
TryGetJsonAsync
SendMutationAsync
LaunchBuildAsync
StartInstallAsync
VerifyBuildAsync
GetBuildSplashAsync
CheckDownloadServicesAsync
GetLibraryAsync
ImportBuildAsync
SendNeoMessageAsync
RunLauncherUpdateCheckAsync
CheckLauncherUpdateAsync
ApplyLauncherUpdateAsync
OpenExternalUrlAsync
```

Estos métodos conectan el frontend Tauri con los services nativos C#.

---

# 13. Comandos Tauri founds

Recovered frontend commands/events inclufrom:

```text
launch_neo_build
startInstall
verify_neo_build
getBuilds
getAccountStatus
getAccountTier
loginDiscord
completeAccountSetup
fetch_neo_friends
send_neo_message
set_neo_presence
search_neo_accounts
checkLauncherUpdate
applyLauncherUpdate
get_neo_server_status
```

Esto permite reconstruir parcialmente la interfaz entre frontend y backend local del launcher.

---

# 14. Hosts de Neo

The following hosts were recovered from the cofrom:

```text
neofn.dev
```

Main backend:

```text
https://neofn.dev
```

XMPP:

```text
wss://xmpp-service-prod.neofn.dev
```

XMPP domain:

```text
xmpp-service-prod.neofn.dev
```

Lightswitch:

```text
https://lightswitch-public-service-prod.neofn.dev/lightswitch
```

Analytics:

```text
https://analytics-public-service-prod.neofn.dev/analytics
```

Launcher:

```text
https://launcher-public-service-prod06.neofn.dev/launcher
```

Account:

```text
https://account-public-service-prod.neofn.dev/account
```

Fortnite:

```text
https://fortnite-public-service-prod11.neofn.dev/fortnite
```

Prism:

```text
https://prism-public-service-prod.neofn.dev/prism
```

Friends:

```text
https://friends-public-service-prod.neofn.dev/friends
```

Fortnite content:

```text
https://fortnitecontent-website-prod07.neofn.dev/content/api/pages/fortnite-game
```

Launcher news:

```text
https://fortnitecontent-website-prod07.neofn.dev/content/api/launcher/news
```

Store:

```text
https://store.neofn.dev/api/v1/entitlements/{accountId}
```

---

# 15. Arquitectura de services

```text
NeoLauncher.exe
     |
     +-- AccountService
     |      |
     |      +-- account-public-service-prod.neofn.dev
     |
     +-- LauncherService
     |      |
     |      +-- launcher-public-service-prod06.neofn.dev
     |
     +-- Friends
     |      |
     |      +-- friends-public-service-prod.neofn.dev
     |
     +-- Fortnite
     |      |
     |      +-- fortnite-public-service-prod11.neofn.dev
     |
     +-- Prism
     |      |
     |      +-- prism-public-service-prod.neofn.dev
     |
     +-- Lightswitch
     |      |
     |      +-- lightswitch-public-service-prod.neofn.dev
     |
     +-- Analytics
     |      |
     |      +-- analytics-public-service-prod.neofn.dev
     |
     +-- XMPP
            |
            +-- xmpp-service-prod.neofn.dev
```

---

# 16. LauncherService

Archivo reconstruido:

```text
LauncherService.cs
```

Base URL:

```text
https://launcher-public-service-prod06.neofn.dev/launcher
```

The client uses:

```csharp
HttpClient
```

with:

```text
Timeout = 15 segundos
```

---

## 16.1 Releases

Route:

```http
GET /launcher/api/public/releases
```

Código conceptual:

```csharp
using HttpRequestMessage request =
    new HttpRequestMessage(HttpMethod.Get, "api/public/releases");
```

The request uses a Bearer token obtained through:

```text
GetClientCredentialsAccessTokenAsync()
```

---

# 17. Online count

Route:

```http
GET /launcher/api/public/onlinecount
```

Returns online-user information.

---

# 18. Builds

Route:

```http
GET /launcher/api/public/builds
```

También requiere un Bearer token de client.

The result is converted to:

```text
List<BuildInfo>
```

---

# 19. Distribution Points

Route:

```http
GET /launcher/api/public/distributionpoints
```

Model:

```csharp
public class DistributionPointsResponse
{
    [JsonPropertyName("distributions")]
    public required string[] Distributions { get; set; }
}
```

Conceptually, the expected response is:

```json
{
  "distributions": [
    "...",
    "..."
  ]
}
```

---

# 20. BuildInfo

Modelo recuperado:

```csharp
public class BuildInfo
{
    [JsonPropertyName("version")]
    public required GameVersion Version { get; init; }

    [JsonPropertyName("fileSizeBytes")]
    public required long FileSizeBytes { get; init; }

    [JsonPropertyName("releaseDate")]
    public required DateOnly ReleaseDate { get; init; }

    [JsonPropertyName("isLive")]
    public required bool IsLive { get; init; }

    [JsonPropertyName("splashUrl")]
    public required string SplashUrl { get; init; }

    [JsonPropertyName("manifestPath")]
    public required string ManifestPath { get; init; }

    [JsonIgnore]
    public string Name => Version.Name;
}
```

Important fields:

```text
version
fileSizeBytes
releaseDate
isLive
splashUrl
manifestPath
```

The critical field for downloading is:

```text
manifestPath
```

---

# 21. Flujo de instalación

`InstallService.StartInstallAsync()` hace:

```text
GetDistributionPointsAsync()
        |
        v
distribution[]
        |
        v
distribution[0] + "/" + manifestPath
        |
        v
LoadManifestAsync()
        |
        v
FBuildPatchAppManifest
        |
        v
StartCoreAsync()
```

The client also creates/uses:

```text
<distribution>/Builds/Fortnite/CloudDir
```

as chunk storage directories.

---

# 22. Manifest

The manifest is a BuildPatchServices JSON manifest.

It can contain:

```text
ManifestFileVersion
bIsFileData
AppID
AppNameString
BuildVersionString
LaunchExeString
LaunchCommand
PrereqName
PrereqPath
PrereqArgs
PrereqIds[]
ChunkHashList{}
ChunkShaList{}
DataGroupList{}
ChunkFilesizeList{}
FileManifestList[]
```

Each file can contain:

```text
Filename
FileHash
bIsReadOnly
bIsCompressed
bIsUnixExecutable
SymlinkTarget
InstallTags[]
FileChunkParts[]
```

Each `FileChunkPart` contains:

```text
Guid
Offset
Size
```

---

# 23. BuildPatchServices

Decompiled:

```text
~/lanzado/lib/app/BuildPatchServices.dll
```

to:

```text
~/BuildPatchServices_SRC
```

Main component:

```text
FBuildPatchAppManifest
```

Implementto:

```text
IBuildManifest
```

---

# 24. Métodos del manifest

Recovered methods inclufrom:

```text
GetAppName()
GetVersionString()
GetBuildId()
GetPrereqPath()
GetPrereqArgs()
GetPrereqName()
GetFileList()
GetFileSize()
GetFileHash()
GetChunksRequiredForFiles()
GetDataList()
GetChunkHash()
IsFileDataManifest()
GetDataFilename()
```

---

# 25. Build ID

The reconstructed code returns:

```csharp
public string GetVersionString() => ManifestMeta.BuildVersion;
public string GetBuildId() => ManifestMeta.BuildVersion;
```

Therefore, in this implementation:

```text
BuildId = BuildVersion
```

according to the decompiled code.

---

# 26. Tamaño de file

A file's size is obtained by summing:

```text
FileChunkParts[].Size
```

Conceptualmente:

```text
FileSize = Σ FileChunkPart.Size
```

---

# 27. Chunks

Files do not necessarily have to be downloaded as complete files.

The system uses chunks.

Cada file contains referencias to:

```text
Guid
Offset
Size
```

Chunks are identified by GUIDs.

---

# 28. Nombre de chunk

`CloudChunkSource.GetChunkRelativePath()` contains a default path:

```csharp
string text = chunkGuid.ToString("N").ToUpperInvariant();
string value = text.Substring(0, 2);
return $"ChunksV4/{value}/{text}.chunk";
```

Por ejemplo, parto:

```text
0123456789ABCDEF0123456789ABCDEF
```

la route por defecto seríto:

```text
ChunksV4/01/0123456789ABCDEF0123456789ABCDEF.chunk
```

---

# 29. IMPORTANTE: Neo usa un resolver propio

Aunque `CloudChunkSource` tiene una route por defecto, `FBuildPatchInstaller` configurto:

```text
chunkPathResolver
```

using:

```text
appManifest.GetDataFilename(guid)
```

Therefore, Neo's final path format may be generated band:

```text
FBuildPatchAppManifest.GetDataFilename()
```

rather than the fallback:

```text
ChunksV4/<2 primeros caracteres>/<GUID>.chunk
```

---

# 30. Formato moderno de GetDataFilename

Recovered `FBuildPatchAppManifest` code shows that modern versions may use:

```text
ChunksV4
```

and construct paths based on:

```text
DataGroup
ChunkHash
GUID
```

The observed conceptual form is:

```text
ChunksV4/<grupo>/<hash>_<GUID>.chunk
```

El GUID se formatea en mayúsculas sin guiones.

---

# 31. CRC32

The code contains a function:

```text
Crc32Guid
```

which calculates CRC32 over:

```text
Guid.ToByteArray()
```

This is used by certain chunk organization formats.

---

# 32. CloudChunkSource

`CloudChunkSource` manages chunk downloads.

Flow:

```text
chunkGuid
    |
    v
GetNextCloudRoot()
    |
    v
GetChunkRelativePath()
    |
    v
FChunkUriRequest
    |
    v
ResolveChunkUri()
    |
    v
URL final
    |
    v
DownloadService
    |
    v
HTTP GET
```

---

# 33. CloudRoots

Configuration:

```text
CloudChunkSourceConfig.CloudRoots
```

It is a list of distribution directories/URLs.

The system selects one using:

```csharp
int value = Interlocked.Increment(ref _cloudRootIndex);
return _config.CloudRoots[Math.Abs(value) % _config.CloudRoots.Count];
```

Therefore, when multiple `CloudRoots` exist, they are rotated.

---

# 34. URL del chunk

`DownloadChunkAsync()` construye:

```text
CloudDirectory + RelativePath
```

If `ResolveChunkUri()` does not return a different URL:

```csharp
resolvedUri =
    PathUtils.Combine(
        nextCloudRoot,
        chunkRelativePath
    );
```

Then:

```csharp
_downloadService.RequestFileWithHeaders(
    resolvedUri,
    headers,
    ...
);
```

---

# 35. ResolveChunkUri

The function:

```csharp
private FChunkUriResponse? ResolveChunkUri(FChunkUriRequest request)
```

creates to:

```text
UriResolverHandler
```

and temporarily registers the handler with:

```text
_messagePump
```

Si no puede resolver la URI, utilizto:

```text
CloudDirectory + RelativePath
```

as the fallback.

---

# 36. DownloadService

El service de descarga utilizto:

```text
HttpClient
```

and:

```text
HttpRequestMessage(HttpMethod.Get, uri)
```

The recovered flow is:

```text
RequestFile
RequestFileWithHeaders
       |
       v
ExecuteHttpRequestAsync
       |
       v
HttpClient.SendAsync
```

Therefore, chunk downloads are performed over HTTP.

---

# 37. CloudChunkSource y DownloadService

Arquitecturto:

```text
CloudChunkSource
       |
       | URI
       v
DownloadService
       |
       v
HttpClient
       |
       v
HTTP GET
```

`CloudChunkSource` decide qué chunk pedir.

`DownloadService` realiza la transferencia.

---

# 38. Modos de instalación

El instalador utilizto:

```text
InstallMode = NonDestructiveInstall
```

Verification:

```text
VerifyMode = ShaVerifyAllFiles
```

Política Deltto:

```text
DeltaPolicy = Skip
```

This means the system performs SHA verification of files and uses non-destructive installation.

---

# 39. Estadísticas de instalación

The system records information such as:

```text
manifest.GetVersionString()
manifest.GetBuildId()
```

También maneja estadísticas from:

```text
Downloaded chunks
Recycled chunks
Chunk DB chunks
```

and other installation-process data.

---

# 40. InstallResult

The installer handles results containing information such as:

```text
Success
Cancelled
ErrorCode
ErrorText
InstallPath
Version
BuildId
```

---

# 41. OAuth

Se encontró un sistema OAuth with:

```text
Password
ExchangeCode
DeviceCode
AuthorizationCode
RefreshToken
ClientCredentials
```

Enum:

```csharp
public enum OAuthGrantType
{
    Password,
    ExchangeCode,
    DeviceCode,
    AuthorizationCode,
    RefreshToken,
    ClientCredentials
}
```

---

# 42. OAuth token endpoint

Base:

```text
https://account-public-service-prod.neofn.dev/account/
```

Route:

```http
POST /account/api/oauth/token
```

La solicitud usto:

```text
application/x-www-form-urlencoded
```

and may inclufrom:

```text
grant_type
username
password
exchange_code
authorization_code
refresh_token
```

---

# 43. OAuth responses

`OAuthTokenResponse` contains:

```text
access_token
refresh_token
expires_in
refresh_expires
expires_at
refresh_expires_at
token_type
client_id
client_service
internal_client
account_id
in_app_id
scope
displayName
app
```

---

# 44. Exchange Code

There is:

```http
GET /account/api/oauth/exchange
```

The launcher requests an exchange code using the user's Bearer token.

Respuestto:

```text
code
creatingClientId
expiresInSeconds
```

---

# 45. Discord OAuth

Methods found:

```text
LoginDiscordAsync
ExchangeDiscordOAuthCodeAsync
```

A challenge endpoint also exists:

```text
/account/api/oauth/challenge/{scheme}
```

con parámetros relacionados with:

```text
clientId
redirectUri
```

Observed redirect:

```text
neolauncher://callback/auth
```

**No credentials or secrets are included in this document.**

---

# 46. AccountService

Recovered routes:

```http
POST   /account/api/oauth/token

DELETE /account/api/oauth/sessions/kill?killType=OTHERS_ACCOUNT_CLIENT

GET    /account/api/public/account/{accountId}

GET    /account/api/public/account/setup/status

GET    /account/api/public/account/displayName/{displayName}/available

GET    /account/api/public/account/displayName/{displayName}

POST   /account/api/public/account/setup

GET    /account/api/oauth/exchange
```

---

# 47. Inicio de sesión

Conceptual flow:

```text
Usuario
   |
   v
OAuth
   |
   v
OAuthTokenResponse
   |
   v
AccountId
   |
   v
GetAccountAsync()
   |
   v
LoadoutService
   |
   v
Avatar
   |
   v
UserRecord
```

El refresh token se guarda using:

```text
Windows PasswordVault
```

with resource:

```text
NeoLauncher
```

---

# 48. Refresh de sesión

El launcher utilizto:

```text
RefreshToken
```

to refresh the session.

A timer attempts to refresh approximateland:

```text
5 minutos antes de la expiración
```

---

# 49. Client Credentials

There is:

```text
GetClientCredentialsAccessTokenAsync()
```

This token is used for service operations such as retrieving:

```text
releases
builds
```

The client-credentials token is kept in memory.

---

# 50. Friends

Servicio:

```text
friends-public-service-prod.neofn.dev
```

Rutas encontradas:

```http
GET /friends/api/public/friends/{accountId}?includePending=true

GET /friends/api/public/blocklist/{accountId}
```

The following also exist:

```text
FetchNeoFriendsAsync
SearchNeoAccountsAsync
NeoFriendActionAsync
GetFriendsAccessTokenAsync
```

---

# 51. XMPP

Host:

```text
wss://xmpp-service-prod.neofn.dev
```

Dominio:

```text
xmpp-service-prod.neofn.dev
```

The launcher contains:

```text
NeoXmppClient
NeoPresenceService
```

y métodos relacionados with:

```text
ConnectNeoPresenceAsync
SendNeoMessageAsync
set_neo_presence
```

This indicatestes that the system uses WebSocket/XMPP for presence and messaging.

---

# 52. Fortnite / Loadout

Host:

```text
https://fortnite-public-service-prod11.neofn.dev/fortnite
```

Ruta encontradto:

```http
GET /fortnite/api/game/v2/loadout/{accountId}
```

También aparece una variante from:

```http
GET /fortnite/api/game/v2/loadout
```

`LoadoutService` uses this data to retrieve avatar/loadout-related information.

---

# 53. Prism

Host:

```text
https://prism-public-service-prod.neofn.dev/prism
```

There is:

```text
PrismService
```

The analysis confirms the presence of the service client, but did not reconstruct a private Prism backend.

---

# 54. Lightswitch

Host:

```text
https://lightswitch-public-service-prod.neofn.dev/lightswitch
```

There is:

```text
LightswitchService
```

It is separate from the main launcher service.

---

# 55. Store

The frontend/client contains:

```text
https://store.neofn.dev
```

and an entitlements route:

```http
/api/v1/entitlements/{accountId}
```

También están definidos eventos de tiendto:

```text
StoreOpened
StoreClosed
StoreItemViewed
StoreItemPurchased
```

Not all of these events were observed being called during static inspection.

---

# 56. Noticias

Content endpoints were found:

```text
https://fortnitecontent-website-prod07.neofn.dev/content/api/pages/fortnite-game
```

and:

```text
https://fortnitecontent-website-prod07.neofn.dev/content/api/launcher/news
```

This suggests that the launcher can consume content/news from a service compatible with Fortnite's content structure.

---

# 57. Analytics

Base:

```text
https://analytics-public-service-prod.neofn.dev/analytics
```

Endpoint:

```http
POST /analytics/api/v1/public/event
```

Telemetry is only sent when:

```text
SendDiagnostics == true
```

---

# 58. Datos de Analytics

Each event may inclufrom:

```text
EventType
SessionId
AccountId
AppVersion
OsVersion
Locale
Properties
```

The session is identified using:

```text
Guid.NewGuid().ToString("N")
```

---

# 59. EventType

Eventos definidos:

```text
LauncherFirstRun
LauncherOpened
LauncherClosed
LauncherUpdated
LauncherUpdateFailed
LauncherLogin
LauncherLoginFailed
LauncherLogout

BuildDownloaded
BuildDownloadFailed

GameImported
GameUninstalled
GameRepaired
GameLaunched
GameLaunchFailed
GameClosed

StoreOpened
StoreClosed
StoreItemViewed
StoreItemPurchased
```

Total:

```text
20 eventos definidos
```

---

# 60. Eventos realmente observados

Se encontraron llamadas to:

```text
LauncherClosed
LauncherOpened
LauncherFirstRun

LauncherLogin
LauncherLoginFailed
LauncherLogout

GameLaunched
GameLaunchFailed
GameClosed

BuildDownloaded
BuildDownloadFailed

LauncherUpdateFailed
LauncherUpdated
```

No se observaron llamadas reales, en la inspección realizada, parto:

```text
GameImported
GameUninstalled
GameRepaired
StoreOpened
StoreClosed
StoreItemViewed
StoreItemPurchased
```

This does not prove they are never used; only that they did not appear in the analyzed calls.

---

# 61. Datos de GameLaunchFailed

When a launch fails, the following are recorded:

```text
version
reason
```

Conceptualmente:

```csharp
Analytics.Track(
    EventType.GameLaunchFailed,
    new Dictionary<string, object>
    {
        ["version"] = version.Version.ToString(),
        ["reason"] = failureReason
    }
);
```

---

# 62. Datos de BuildDownloaded

When a normal download finishes:

```text
version
durationSeconds
avgSpeedMbps
```

A repair is not recorded as a normal download.

---

# 63. Datos de BuildDownloadFailed

Cuando falla una descargto:

```text
version
error
```

The `error` comes from:

```text
buildStatistics.ErrorCode
```

---

# 64. GameLauncher

El launcher tiene:

```text
GameLauncher
GameService
LibraryService
InstallService
```

These components manage:

```text
instalación
verificación
biblioteca
lanzamiento
reparación
estadísticas
```

---

# 65. Actualizaciones

Se encontraron:

```text
NeoUpdateSource
UpdateService
```

Bridge methods:

```text
RunLauncherUpdateCheckAsync
CheckLauncherUpdateAsync
ApplyLauncherUpdateAsync
```

Frontend commands:

```text
checkLauncherUpdate
applyLauncherUpdate
```

Analytics events:

```text
LauncherUpdated
LauncherUpdateFailed
```

---

# 66. Seguridad y credenciales

During the analysis, a reference to OAuth client credentials was found in the code/configuration.

**For security, this README does not reproduce any secret.**

If that client/project is controlled by you, any publicly exposed secret should be considered compromised and rotated/revoked.

The code analysis does not require using those secrets.

---

# 67. DNS

Se realizaron consultas DNS parto:

```text
account-public-service-prod.neofn.dev
friends-public-service-prod.neofn.dev
fortnite-public-service-prod11.neofn.dev
launcher-public-service-prod06.neofn.dev
lightswitch-public-service-prod.neofn.dev
analytics-public-service-prod.neofn.dev
prism-public-service-prod.neofn.dev
xmpp-service-prod.neofn.dev
```

Todos resolvieron mediante Cloudflare to:

```text
104.26.11.37
104.26.10.37
172.67.75.117
```

Conclusion:

```text
The services are behind Cloudflare.
```

These IPs do not directly reveal the private origin server.

---

# 68. Certificate Transparency

Certificate transparency results showed:

```text
*.neofn.dev
cdn.neofn.dev
content-cdn.neofn.dev
neofn.dev
```

The wildcard:

```text
*.neofn.dev
```

explains why certificates do not necessarily enumerate every individual service.

---

# 69. CDN

The following was checked:

```text
https://cdn.neofn.dev/
```

Respuesta observadto:

```text
HTTP/2 405
```

and via GET:

```text
HTTP/2 403
```

with Cloudflare.

No attempt was made to bypass the restrictions.

The following also appeared:

```text
content-cdn.neofn.dev
```

in Certificate Transparency.

**No evidence was found in the analyzed code that these two hosts are necessarily the CloudRoots used by the launcher.**

---

# 70. Wayback Machine

The history of the following was checked:

```text
neofn.dev
```

Se encontró una capturto:

```text
20260806191629
```

La página archivada mostrabto:

```text
Neo

Site under construction.

We're building something new.
Join our Discord to be the first to know when it drops.

© 2026 Neo
```

It also contained a Discord link.

Later, an Internet Archive CDX query temporarily returned:

```text
Temporarily Offline
```

---

# 71. Common Crawl

The following index was queried:

```text
CC-MAIN-2026-30
```

parto:

```text
neofn.dev/*
```

Result:

```text
No Captures found for: neofn.dev/
```

Therefore, that index did not provide useful captures of the site.

---

# 72. Proyecto Nova

The relationship between Neo and Project Nova was investigated.

La página oficial de Project Nova indicatesbto:

```text
From the creators of Project Nova

Introducing Neo

The creators of Nova have made a new project named Neo.
```

This publicly establishes a relationship between the projects.

However:

```text
Proyecto Nova ≠ backend Neo confirmed
```

The public Nova repositories should not be assumed to be Neo's current backend.

---

# 73. GitHub de Project Nova

The organization:

```text
ProjectNovaFN
```

had the following public repositories:

```text
Sinum
NovaLauncher.Client
NovaServices.Matchmaker
vivox
Nova
```

These repositories belong to the Nova ecosystem.

---

# 74. Nova

Repositorand:

```text
ProjectNovaFN/Nova
```

README:

```text
Core game server for Project Nova
```

Mentioned period:

```text
2022-2025
```

Mentioned credits:

```text
Ender
Samuel
Milxnor
Jacobb626
```

Licencito:

```text
CC0
```

It was not confirmed to be the backend used by Neo.

---

# 75. NovaLauncher.Client

Repositorand:

```text
ProjectNovaFN/NovaLauncher.Client
```

It is the Project Nova launcher client.

Technologies:

```text
C++
C
```

Créditos:

```text
Ender
Samuel
```

Licencito:

```text
CC0
```

It is not the same client as:

```text
NeoLauncher
```

---

# 76. NovaServices.Matchmaker

Repositorand:

```text
ProjectNovaFN/NovaServices.Matchmaker
```

It is a Nova matchmaking service.

Tecnologíto:

```text
C#
```

Créditos:

```text
Ender
Samuel
Kyiro
```

Licencito:

```text
CC0
```

No se confirmó que sea el matchmaking de Neo.

---

# 77. Sinum

Repositorand:

```text
ProjectNovaFN/Sinum
```

README:

```text
Redirects requests from Epic URLs to api.novafn.dev
```

It is part of the Nova ecosystem.

It should not be confused with the services:

```text
*.neofn.dev
```

---

# 78. Investigación de repositorio Neo

The following were searched:

```text
NeoLauncher
neofn.dev
launcher-public-service-prod06.neofn.dev
prism-public-service-prod.neofn.dev
friends-public-service-prod.neofn.dev
xmpp-service-prod.neofn.dev
store.neofn.dev
```

The following were also searched:

```text
C:\Users\Ender\Documents\GitHub\NeoLauncher
```

y el hash:

```text
f91d8131e7ff895e1786701afeb4184404b45831
```

No confirmed public repository was found for:

```text
NeoLauncher
```

nor for Neo's private backend.

---

# 79. Qué sí podemos reconstruir

From the client, a substantial amount of information can be reconstructed:

```text
Frontend
       ↓
Tauri commands
       ↓
.NET services
       ↓
API endpoints
       ↓
OAuth / sesión
       ↓
Launcher API
       ↓
Build metadata
       ↓
Distribution points
       ↓
Manifest JSON
       ↓
BuildPatchServices
       ↓
Chunk GUID
       ↓
Chunk path
       ↓
CloudRoot
       ↓
HTTP GET
```

---

# 80. Qué NO podemos afirmar

The client alone does not establish:

```text
❌ code fuente privado completo del backend
❌ base de datos de Neo
❌ code fuente del servidor de autenticación
❌ claves privadas
❌ infraestructura/origen detrás de Cloudflare
❌ repositorio privado
❌ implementación interna de todos los services
```

El client contains las interfaces necesarias para comunicarse con esos services, pero no necesariamente su implementación.

---

# 81. Backend vs client

It is important to distinguish:

```text
CLIENTE
NeoLauncher.dll
BuildPatchServices.dll
frontend Tauri
```

from:

```text
SERVIDORES
account-public-service
launcher-public-service
friends-public-service
fortnite-public-service
prism-public-service
lightswitch-public-service
analytics-public-service
xmpp-service
```

The analysis recovered **client** code, not the private implementation of those servers.

---

# 82. Modelo de distribución completo

La arquitectura de distribución recuperada puede representarse así:

```text
                 Launcher API
                      |
                      |
            /api/public/builds
                      |
                      v
                  BuildInfo
                      |
                 manifestPath
                      |
                      v
        /api/public/distributionpoints
                      |
                      v
               distributions[]
                      |
          +-----------+-----------+
          |                       |
          v                       v
     Distribution 1          Distribution 2
          |                       |
          +-----------+-----------+
                      |
                      v
              <distribution>/
                      |
                      +-- manifest
                      |
                      +-- Builds/Fortnite/CloudDir
                              |
                              v
                           chunks
```

---

# 83. Ejemplo conceptual de descarga

Suppose:

```text
CloudRoot:
https://example-distribution/
```

and:

```text
RelativePath:
ChunksV4/01/ABCDEF....chunk
```

The final URL would conceptually be:

```text
https://example-distribution/ChunksV4/01/ABCDEF....chunk
```

Pero en Neo el `RelativePath` real puede venir from:

```text
FBuildPatchAppManifest.GetDataFilename()
```

so the specific manifest must be inspected to determine an actual URL.

---

# 84. Estado actual de la investigación

## Confirmed

```text
✓ Instalador Velopack
✓ Neo Launcher
✓ .NET 10
✓ x64
✓ C# 14
✓ Tauri
✓ WebView2
✓ BuildPatchServices
✓ OAuth
✓ Account API
✓ Launcher API
✓ Friends API
✓ Fortnite API
✓ Prism API
✓ Lightswitch API
✓ Analytics API
✓ XMPP
✓ Store
✓ Distribution Points
✓ Manifest JSON
✓ CloudRoots
✓ Chunk downloads
✓ Cloudflare
✓ PDB path
✓ Assembly informational version
```

## Partially Confirmed

```text
~ relación Neo ↔ Project Nova
~ formato final de todos los chunks
~ función exacta del UriResolverHandler
~ estructura completa de Prism
~ estructura completa de Store
```

## Not Confirmed

```text
? repositorio público NeoLauncher
? backend privado Neo
? origen de infraestructura detrás de Cloudflare
? base de datos
? code fuente privado del backend
```

---

# 85. Archivos locales de trabajo

Extraction:

```text
~/lanzado
```

Reconstructed code:

```text
~/NeoLauncher_SRC
```

Reconstructed BuildPatchServices:

```text
~/BuildPatchServices_SRC
```

DLL principal:

```text
~/lanzado/lib/app/NeoLauncher.dll
```

BuildPatchServices:

```text
~/lanzado/lib/app/BuildPatchServices.dll
```

---

# 86. Comandos útiles useds

Extraction/decompilation:

```bash
ilspycmd -p -o ~/NeoLauncher_SRC ~/lanzado/lib/app/NeoLauncher.dll
```

BuildPatchServices:

```bash
ilspycmd -p -o ~/BuildPatchServices_SRC ~/lanzado/lib/app/BuildPatchServices.dll
```

Inspección de CloudChunkSource:

```bash
sed -n '180,270p' ~/BuildPatchServices_SRC/BuildPatchServices/CloudChunkSource.cs
```

Búsqueda de GetChunkRelativePath:

```bash
grep -n -A35 -B10 'GetChunkRelativePath' \
~/BuildPatchServices_SRC/BuildPatchServices/CloudChunkSource.cs
```

Inspección del resolver:

```bash
sed -n '340,375p' \
~/BuildPatchServices_SRC/BuildPatchServices/CloudChunkSource.cs
```

---

# 87. Próximo paso recomendado

To complete the distribution-system analysis, the remaining task is to determine exactly what:

```text
UriResolverHandler
```

y comprobar si existe alguna implementación específica from:

```text
FChunkUriRequest
FChunkUriResponse
```

en NeoLauncher.

También es útil identificar todos los lugares donde se creto:

```text
CloudDirectories
```

y verificar la implementación final from:

```text
GetDataFilename()
```

Esto permitiría pasar from:

```text
"we know the architecture"
```

to:

```text
"we know exactly how each chunk URL is generated"
```

---

# 88. Nota sobre ingeniería inversa

Most of the C# code in this document comes from **decompilation**.

Therefore:

```text
code decompilado ≠ code fuente original
```

ILSpy can reconstruct:

```text
clases
métodos
tipos
strings
atributos
flujo lógico
```

but may lose:

```text
comentarios
nombres originales de variables locales
formato
estructura exacta del proyecto
scripts de build
files no incluidos
```

---

# 89. Conclusion

The analysis produced a fairly complete view of the **Neo Launcher client**.

The core application is a client built from:

```text
.NET 10 x64
+
WinUI/WebView2
+
Tauri
+
BuildPatchServices
```

that communicates with multiple services under:

```text
*.neofn.dev
```

El sistema de instalación obtiene builds desfrom:

```text
Launcher API
```

and obtains distribution points through:

```text
/api/public/distributionpoints
```

descarga un manifest y posteriormente utilizto:

```text
BuildPatchServices
```

to resolve files → chunks → paths → URLs → HTTP downloads.

The public-facing infrastructure visible from the client is substantial, but the analysis alone does not provide the private source code of the servers.

---

## Security

This README intentionally avoids storing:

- client secrets
- refresh tokens
- access tokens
- contraseñas
- credenciales
- cookies
- datos privados de cuentas

If any secret appeared during the analysis, it should be treated as compromised and rotated if you control the project.

---

## Status

```text
NeoLauncher static analysis
Version observed: 1.1.0 package
Assembly version: 1.0.0.0
Target: .NET 10 / x64
Frontend: Tauri
Installer: Velopack
Status: ongoing reverse engineering
```
