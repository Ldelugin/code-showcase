Function Show-AllUserSecrets
{
    <#
    .SYNOPSIS
        Shows all user secrets.
    
    .DESCRIPTION
        Function to show all user secrets as environment variables.
    
    .PARAMETER Path
        The path to the .env file.
    
    .PARAMETER Scope
        The scope of the secrets to show. The default scope is Machine.
    
    .INPUTS
        The function accepts an object which contains the path to the .env file.
    
    .OUTPUTS
        None.
    
    .EXAMPLE
        Show-AllUserSecrets -Path ".env"
    
    .EXAMPLE
        $env = @{
            Path = ".env"
        }
        Show-AllUserSecrets @env
    
    .EXAMPLE
        ".env" | Show-AllUserSecrets
    #>
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory = $false, HelpMessage = "The path to the .env file", ValueFromPipeline = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Path = '.env',

        [Parameter(Mandatory = $false, HelpMessage = "The scope of the secrets to show")]
        [System.EnvironmentVariableTarget] $Scope = [System.EnvironmentVariableTarget]::Machine
    )

    $Path | Get-AllEnvVariables | ForEach-Object {
        $name = $_.Name
        $value = $_ | Get-UserSecret -Scope $Scope
        Write-Host "$name=$value"
    }
}