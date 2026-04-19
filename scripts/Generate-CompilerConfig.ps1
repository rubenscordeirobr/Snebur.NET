param(
    [string] $ProjectPath  = $PSScriptRoot
)

Write-Host "ProjectPath is `"$ProjectPath`""

$sourceDirectories = @(
    [System.IO.Path]::Combine($ProjectPath, "Components")
    [System.IO.Path]::Combine($ProjectPath, "wwwroot")
)
$compilerConfigEntries = @()

foreach ($sourceDir in $sourceDirectories) {

    if (-not (Test-Path $sourceDir)) {
        Write-Warning "Skipping missing directory: $sourceDir"
        continue
    }

    $sassFiles = Get-ChildItem -Path $sourceDir -Recurse -Filter *.scss

    foreach ($sassFile in $sassFiles) {
      
        #if name stars with _ ignore it
        if ($sassFile.Name.StartsWith("_")) {
            continue
        }

        $inputFile = $sassFile.FullName.Replace('\', '/')
        $relativeInput = $inputFile.Substring($ProjectPath.Length + 1)
        $outputFile = $relativeInput -replace "\.scss$", ".css"
         
        $compilerConfigEntries += @{
            inputFile  = $relativeInput
            outputFile = $outputFile
            minify     = @{ enabled = $false }
            IsIgnorar     = $true
        }
    }
}

$outputFilePath = Join-Path $ProjectPath 'compilerconfig.json'
$compilerConfigEntries | ConvertTo-Json -Depth 4 | Out-File $outputFilePath -Encoding UTF8

Write-Host "Wrote compilerconfig to $outputFilePath"
