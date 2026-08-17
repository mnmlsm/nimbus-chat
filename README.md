# Nimbus Chat

> [!IMPORTANT]
> This repository's default branch (`main`) contains an older, unmaintained
> architecture that does not work against the live API. The current, working
> version of Nimbus Chat lives **only** on the `memo-update` branch.
>
> Clone it directly on that branch:
> ```
> git clone -b memo-update https://github.com/mnmlsm/nimbus-chat.git
> ```
> Already cloned on `main`? Switch branches before building:
> ```
> git checkout memo-update
> ```

Nimbus Chat is a small WPF desktop application developed in C#, backed by an
ASP.NET Core Web API.

## Features
- Login and registration UI
- Dashboard
- Weather search
- User profile
- Messaging system
- Light/Dark mode

## Technologies
- C# / WPF (.NET Framework 4.7.2) — desktop client
- ASP.NET Core Web API (.NET 8) — `nimbus-chat.Api`
- MySQL — database (accessed only by the API, never directly by the client)
- OpenWeather API — weather data

## Architecture
The WPF client never talks to MySQL directly. It calls the API over HTTPS
(`https://api.memo-dev.uk` by default — see `clientsettings.json`), reachable
through a Cloudflare Tunnel to a locally running `nimbus-chat.Api` instance,
which in turn talks to a local MySQL instance. Building the client "just
works" out of the box: `clientsettings.json` ships in the repo with a working
default, so no machine-specific setup is required to run it. See
`API-Network-setup.txt` for full operational details on the server side.

## Team
- mnmlsm
- memopunkt
