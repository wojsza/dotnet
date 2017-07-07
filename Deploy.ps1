# Parameters

param([string]$release="")

# End Parameters

# Lib

Function CreateProductionFolder
{
    Param($deployFolder, $utilityScriptPath)
    $deployFolderDir = "$($deployFolder)"

    if(!(Test-Path -Path $deployFolderDir)){
        Write-Host $deployFolderDir
        Write-Host "---- Creating new deploy folder ----"
        New-Item -ItemType directory -Path $deployFolderDir
        Write-Host "---- Finished creating new deploy folder ----"
    }
}

Function CopyRelese
{
    Param($solutionPath, $utilityScriptPath, $deployFolder, $binary, $excludeFiles, $excludeFolders)
    
    $deployFolderDir = "$($deployFolder)"
    $binSolutionFolder = "$($solutionPath)$($binary)"

    $excludeFoldersRegex = $excludeFolders -join '|'
	
    Get-ChildItem -Path $binSolutionFolder -Recurse -Exclude $excludeFiles | 
    where { $excludeFolders -eq $null -or $_.FullName.Replace($binSolutionFolder, "") -notmatch $excludeFoldersRegex } |
    Copy-Item -Destination {
        if ($_.PSIsContainer) {
            Join-Path $deployFolderDir $_.Parent.FullName.Substring($binSolutionFolder.length -1)
        }
        else {
            Join-Path $deployFolderDir $_.FullName.Substring($binSolutionFolder.length -1)
        }
    } -Force -Exclude $excludeFiles
}

# End Lib

Function Deploy
{
    Param($excludeFiles, $excludeFolders, $deployFolder, $binary, $manualSolutionPath = 0)

    $utilityScriptPath = (Get-Location)
    $solutionPath = (Get-Item $utilityScriptPath).parent.FullName

    if($manualSolutionPath) {
        $deployFolder = "$($manualSolutionPath)"
    }
    else {
        $deployFolder = "$($utilityScriptPath)$($deployFolder)"
    }
    
    Write-Host "---- Checking deploy folder ----" -ForegroundColor yellow
    CreateProductionFolder -deployFolder $deployFolder -utilityScriptPath $utilityScriptPath
    Write-Host "---- End checking deploy folder ----" -ForegroundColor yellow
    Write-Host "---- Coping new solution ----" -ForegroundColor yellow
    CopyRelese -solutionPath $solutionPath -utilityScriptPath $utilityScriptPath -deployFolder $deployFolder -binary $binary -excludeFiles $excludeFiles -excludeFolders $excludeFolders
    Write-Host "---- End coping new solution ----" -ForegroundColor yellow
	Write-Host (Get-Date) -ForegroundColor cyan
}

#Deploy parameters
$testsExcludeFiles = @('config.xml', 'Config.xml', 'susramerror.xml', 'simics_a.xml', 'MultiConfiguration.dll')
$testsExcludeFolders = @('Tools', 'UnitTests', 'x32', 'x64', 'xOSaClient-x64', 'xOSaClient-x86')
$testsDeployFolder = "\DeployPackage\"
$testsBin = "\Output\Debug\"
Deploy -excludeFiles $testsExcludeFiles -excludeFolders $testsExcludeFolders -deployFolder $testsDeployFolder -binary $testsBin -manualSolutionPath $release
#End Deploy parameters





