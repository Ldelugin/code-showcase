Function Set-AllUserSecrets
{
    <#
    .SYNOPSIS
        Sets all user secrets.
    
    .DESCRIPTION
        Function to set all user secrets as environment variables.
    
    .PARAMETER Path
        The path to the .env file.
    
    .PARAMETER Scope
        The scope of the secrets to set. The default scope is Machine.
    
    .INPUTS
        The function accepts an object which contains the path to the .env file.
    
    .OUTPUTS
        None.
    
    .EXAMPLE
        Set-AllUserSecrets -Path ".env"
    
    .EXAMPLE
        $env = @{
            Path = ".env"
        }
        Set-AllUserSecrets @env
    
    .EXAMPLE
        ".env" | Set-AllUserSecrets
    #>
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory = $false, HelpMessage = "The path to the .env file", ValueFromPipeline = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Path = '.env',

        [Parameter(Mandatory = $false, HelpMessage = "The scope of the secrets to get")]
        [System.EnvironmentVariableTarget] $Scope = [System.EnvironmentVariableTarget]::Machine
    )
    
    $Path | Get-AllEnvVariables | Set-UserSecret -Scope $Scope -DoNotShowRestartIdeWarning
    Show-RestartIdeWarning
}