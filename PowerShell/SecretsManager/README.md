# README

This readme shows you how to set up the user secrets for the development environment.
Instead of having hardcoded password, we use environment variables.

> Note: This is by no means a secure way of storing secrets. It is a step in the right direction for not storing secrets in the code.
> To increment on this, we could use a vault like [Azure Key Vault](https://learn.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-8.0) in combination with `dotnet user-secrets`.
> The downside to that is that some on-premise customers do not have access to internet and thus cannot use Azure Key Vault, maybe there is a way to use a local vault instead.

Table of Contents:

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [QuickStart](#quickstart)
- [Module Reference](#module-reference)
- [Help](#help)

## Prerequisites

- Powershell 5.1 or higher
- Both the legacy PowerShell or the new PowerShell Core 6.0+ are supported and tested.
- Make sure to navigate to the dev folder before running the commands. All the commands are relative to the dev folder.
  - It is possible to run this from any folder, but then you need to specify the full path to the `.env file`.

> **Important!** Make sure to open a PowerShell window with elevated permissions (Administrator rights). The elevated rights are needed to set the
> environment variables in the registry.

## Installation

Open a powershell window and run the following command:

```powershell
# This will import the module in the current session
Import-Module -Name .\SecretsManager -Force
```

## QuickStart

Assuming that this is the first time running this and thus preparing your environment, you can run the following commands:

### 1. Navigate to the dev folder

```powershell
# This will navigate to the dev folder
# assuming that you are in the root folder of the repository
cd dev
```

### 2. First import the module

```powershell
# This will import the module in the current session
Import-Module -Name .\SecretsManager -Force
```

### 3. Copy the example env file

```powershell
# This will copy the .env.example file to the dev folder as .env
New-UserSecretsFile
```

### 4. Replace the values in the .env file

```powershell
# The .env file is a simple text file.
# You can open it in any text editor.
# Replace the SET_A_PASSWORD_HERE_123 values with the correct values.
```

### 5. Set the environment variables

```powershell
# This will set the environment variables using the specified scope, default is Machine.
Set-AllUserSecrets
```

### 6. Verify that the environment variables are set

```powershell
# This will show all the environment variables
Show-AllUserSecrets
```

### 7. IDE

If you have Rider or Visual Studio open, close it first or close it after running these commands.
Otherwise the environment variables will not be read correctly.

## Module Reference

### New-UserSecretsFile

The `New-UserSecretsFile` function creates a new .env file from the .env.example file.

```powershell
New-UserSecretsFile -Path ".env.example" -Destination ".env"
```

### Get-UserSecret

The `Get-UserSecret` function gets the value of a single user secret.

```powershell
Get-UserSecret -Name "SOME_USER_SECRET"
```

### Set-UserSecret

The `Set-UserSecret` function sets the value of a single user secret.

```powershell
Set-UserSecret -Name "SOME_USER_SECRET" -Value "SOME_VALUE"
```

### Remove-UserSecret

The `Remove-UserSecret` function removes a single user secret.

```powershell
Remove-UserSecret -Name "SOME_USER_SECRET"
```

### Remove-AllUserSecrets

The `Remove-AllUserSecrets` function removes all user secrets.
It will read all the keys from the .env file and use those to remove the user secrets.

```powershell
Remove-AllUserSecrets
```

### Set-AllUserSecrets

The `Set-AllUserSecrets` function sets all user secrets.
It will read all the keys from the .env file and use those to set the user secrets.

```powershell
Set-AllUserSecrets
```

### Show-AllUserSecrets

The `Show-AllUserSecrets` function shows all user secrets.
It will read all the keys from the .env file and use those to show the user secrets.

```powershell
Show-AllUserSecrets
```

## Help

Using the `Get-Help` command you can get help on the functions.