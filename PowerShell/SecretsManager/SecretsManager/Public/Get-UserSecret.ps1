Function Get-UserSecret
{
    <#
    .SYNOPSIS
        Gets a user secret.
        
    .DESCRIPTION
        Function to get a user secret as an environment variable.
        
    .PARAMETER Name
        The name of the secret to get.
        
    .PARAMETER Scope
        The scope of the secret to get. The default scope is Machine.
        
    .INPUTS
        The function accepts an object which contains the name of the secret to get.
    
    .OUTPUTS
        None.
        
    .EXAMPLE
        Get-UserSecret -Name "MySecret"
    
    .EXAMPLE
        Get-UserSecret -Name "MySecret" -Scope ([System.EnvironmentVariableTarget]::User)
    
    .EXAMPLE
        $mySecret = @{
            Name = "MySecret"
        }
        $mySecret | Get-UserSecret -Scope ([System.EnvironmentVariableTarget]::User)
    #>
    [CmdletBinding(SupportsShouldProcess)]
    [OutputType([string])]
    Param (
        [Parameter(Mandatory = $true, HelpMessage = "The name of the secret to get", ValueFromPipelineByPropertyName = $true)]
        [string]$Name,

        [Parameter(Mandatory = $false, HelpMessage = "The scope of the secret to get")]
        [System.EnvironmentVariableTarget] $Scope = [System.EnvironmentVariableTarget]::Machine
    )
    
    Process
    {
        if ($PSCmdlet.ShouldProcess($Name, "Getting User Secret"))
        {
            Write-Verbose "Getting User Secret: $Name"
            $value = [System.Environment]::GetEnvironmentVariable($Name, $Scope)
            return [string]$value
        }
    }
}