Function New-UserSecretsFile
{
    <#
    .SYNOPSIS
        Creates a new .env file from the .env.example file.
    
    .DESCRIPTION
        Function to create a new .env file from the .env.example file.
    
    .PARAMETER Path
        The path to the .env.example file.
    
    .PARAMETER Destination
        The path to the .env file.
    
    .INPUTS
        The function accepts an object which contains the path to the .env.example file.
    
    .OUTPUTS
        None.
    
    .EXAMPLE
        New-UserSecretsFile -Path ".env.example" -Destination ".env"
        
    .EXAMPLE
        ".env.example" | New-UserSecretsFile
    
    .EXAMPLE
        ".env.example" | New-UserSecretsFile -Destination ".env"
    #>
    [CmdletBinding(SupportsShouldProcess)]
    Param
    (
        [Parameter(Mandatory = $false, HelpMessage = "The path to the .env.example file", ValueFromPipeline = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Path = '.env.example',

        [Parameter(Mandatory = $false, HelpMessage = "The path to the .env file")]
        [ValidateNotNullOrEmpty()]
        [String] $Destination = '.env'
    )

    Begin
    {
        # validate the .env.example file exists
        if (-not (Test-Path -Path $Path))
        {
            throw "The .env.example file does not exist at $Path"
        }

        # validate the .env file does not exist and if does, prompt to overwrite
        if (Test-Path -Path $Destination)
        {
            Write-Warning "The .env file already exists at $Destination"
            $overwrite = Read-Host -Prompt "Do you want to overwrite it? (Y/N)"
            if ($overwrite -ne 'Y')
            {
                throw "The .env file already exists at $Destination"
            }
        }
    }

    Process
    {
        if ($PSCmdlet.ShouldProcess($Path, "Creating User Secrets File"))
        {
            Write-Verbose "Copy from $Path to $Destination"
            Copy-Item -Path $Path -Destination $Destination -Force
        }
    }
}