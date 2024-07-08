Function Show-RestartIdeWarning
{
    <#
    .SYNOPSIS
        Shows warning about restarting the IDE.
    
    .DESCRIPTION
        Function to show warning about restarting the IDE.
    
    .INPUTS
        None.
    
    .OUTPUTS
        None.
    
    .EXAMPLE
        Show-RestartIdeWarning
    #>

    Write-Warning "Please restart your IDE to pick up the new/updated/removed environment variables."
}