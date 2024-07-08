Function Set-UserSecret
{
    <#
    .SYNOPSIS
        Sets a user secret.
        
    .DESCRIPTION
        Function to set a user secret as an environment variable.
        
    .PARAMETER Name
        The name of the secret to set.
        
    .PARAMETER Value
        The value of the secret to set.
        
    .PARAMETER Scope
        The scope of the secret to set. The default scope is Machine.
        
    .PARAMETER DoNotShowRestartIdeWarning
        Do not show the restart IDE warning.
        
    .INPUTS
        The function accepts an object which contains the name and value of the secret to set.
    
    .OUTPUTS
        None.
        
    .EXAMPLE
        Set-UserSecret -Name "MySecret" -Value "MyValue"
    
    .EXAMPLE
        Set-UserSecret -Name "MySecret" -Value "MyValue" -Scope ([System.EnvironmentVariableTarget]::User)
    
    .EXAMPLE
        $mySecret = @{
            Name = "MySecret"
            Value = "MyValue"
        }
        $mySecret | Set-UserSecret -Scope ([System.EnvironmentVariableTarget]::User)
    #>
    [CmdletBinding(SupportsShouldProcess)]
    Param
    (
        [Parameter(Mandatory = $true, HelpMessage = "The name of the secret to set", ValueFromPipelineByPropertyName = $true)]
        [String] $Name,
        
        [Parameter(Mandatory = $true, HelpMessage = "The value of the secret to set", ValueFromPipelineByPropertyName = $true)]
        [String] $Value,
    
        [Parameter(Mandatory = $false, HelpMessage = "The scope of the secret to set")]
        [System.EnvironmentVariableTarget] $Scope = [System.EnvironmentVariableTarget]::Machine,
    
        [Parameter(Mandatory = $false, HelpMessage = "Do not show the restart IDE warning")]
        [Switch] $DoNotShowRestartIdeWarning
    )
    
    Process
    {
        if ($PSCmdlet.ShouldProcess($Name, "Setting User Secret"))
        {
            Write-Verbose "Setting User Secret: $Name"
            if ($null -ne [System.Environment]::GetEnvironmentVariable($Name, $Scope))
            {
                Write-Warning "Confirm to override the existing value" -WarningAction Inquire
                [System.Environment]::SetEnvironmentVariable($Name, $Value, $Scope)
                if (-not $DoNotShowRestartIdeWarning)
                {
                    Show-RestartIdeWarning
                }
            }
            else 
            {
                [System.Environment]::SetEnvironmentVariable($Name, $Value, $Scope)
                if (-not $DoNotShowRestartIdeWarning)
                {
                    Show-RestartIdeWarning
                }
            }
        }
    }
}