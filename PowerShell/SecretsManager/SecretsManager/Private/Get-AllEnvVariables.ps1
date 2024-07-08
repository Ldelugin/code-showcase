Function Get-AllEnvVariables
{
    <#
    .SYNOPSIS
        Gets all environment variables from a .env file.
    
    .DESCRIPTION
        Function to get all environment variables from a .env file.
    
    .PARAMETER Path
        The path to the .env file.
    
    .INPUTS
        The function accepts an object which contains the path to the .env file.
    
    .OUTPUTS
        The function returns an object which contains all environment variables from a .env file.
    
    .EXAMPLE
        Get-AllEnvVariables -Path ".env"
        
    .EXAMPLE
        $env = @{
            Path = ".env"
        }
        Get-AllEnvVariables @env
        
    .EXAMPLE
        ".env" | Get-AllEnvVariables
    #>
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory = $false, HelpMessage = "The path to the .env file", ValueFromPipeline = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Path = '.env'
    )
    
    $env = Get-Content -Raw $Path | ConvertFrom-StringData
    Write-Verbose "Importing Environment Variables"
    
    $secrets = $env.Keys | ForEach-Object {
        $properties = @{
            Name = $_
            Value = $env[$_]
        }
        return New-Object -TypeName PSObject -Property $properties
    }

    Write-Verbose "Done importing Environment Variables"
    
    return $secrets
}