Function Remove-AllUserSecrets
{
    <#
    .SYNOPSIS
        Removes all user secrets.
    
    .DESCRIPTION
        Function to remove all user secrets as environment variables.
        The function will prompt for confirmation before removing the secrets.
    
    .PARAMETER Path
        The path to the .env file.
    
    .PARAMETER Scope
        The scope of the secrets to remove. The default scope is Machine.
    
    .INPUTS
        The function accepts an object which contains the path to the .env file.
    
    .OUTPUTS
        None.
    
    .EXAMPLE
        Remove-AllUserSecrets -Path ".env"
    
    .EXAMPLE
        $env = @{
            Path = ".env"
        }
        Remove-AllUserSecrets @env
    
    .EXAMPLE
        ".env" | Remove-AllUserSecrets
    #>
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory = $false, HelpMessage = "The path to the .env file", ValueFromPipeline = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Path = '.env',

        [Parameter(Mandatory = $false, HelpMessage = "The scope of the secrets to remove")]
        [System.EnvironmentVariableTarget] $Scope = [System.EnvironmentVariableTarget]::Machine
    )

    $Path | Get-AllEnvVariables | Remove-UserSecret -Scope $Scope -DoNotShowRestartIdeWarning
    Show-RestartIdeWarning
}