Function Remove-UserSecret
{
    <#
    .SYNOPSIS
        Removes a user secret.
        
    .DESCRIPTION
        Function to remove a user secret as an environment variable.
        The function will prompt for confirmation before removing the secret.
        
    .PARAMETER Name
        The name of the secret to remove.
        
    .PARAMETER Scope
        The scope of the secret to remove. The default scope is Machine.
        
    .PARAMETER DoNotShowRestartIdeWarning
        Do not show the restart IDE warning.
        
    .INPUTS
        The function accepts an object which contains the name of the secret to remove.
    
    .OUTPUTS
        None.
        
    .EXAMPLE
        Remove-UserSecret -Name "MySecret"
    
    .EXAMPLE
        Remove-UserSecret -Name "MySecret" -Scope ([System.EnvironmentVariableTarget]::User)
    
    .EXAMPLE
        $mySecret = @{
            Name = "MySecret"
        }
        $mySecret | Remove-UserSecret -Scope ([System.EnvironmentVariableTarget]::User)
    #>
    [CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'High')]
    Param
    (
        [Parameter(Mandatory = $true, HelpMessage = "The name of the secret to remove", ValueFromPipelineByPropertyName = $true)]
        [String] $Name,
        
        [Parameter(Mandatory = $false, HelpMessage = "The scope of the secret to remove")]
        [System.EnvironmentVariableTarget] $Scope = [System.EnvironmentVariableTarget]::Machine,

        [Parameter(Mandatory = $false, HelpMessage = "Do not show the restart IDE warning")]
        [Switch] $DoNotShowRestartIdeWarning
    )
    
    Process
    {
        if ($PSCmdlet.ShouldProcess($Name, "Removing User Secret"))
        {
            Write-Verbose "Removing User Secret: $Name"
            if ($null -ne [System.Environment]::GetEnvironmentVariable($Name, $Scope))
            {
                if ($Confirm)
                {
                    Write-Verbose "Confirmed removing User Secret: $Name"
                    [System.Environment]::SetEnvironmentVariable($Name, $null, $Scope)
                    if (-not $DoNotShowRestartIdeWarning)
                    {
                        Show-RestartIdeWarning
                    }
                }
            }
        }
    }
}