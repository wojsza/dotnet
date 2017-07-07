####################################################################################################
# Parametery:
#   $source      - Http z ktrego cigniemy. http://cygwin.mirror.constant.com/x86_64/
#   $target      - Folder docelowy \ e.g. C:\git\Utils\CygWinImage\x86_64
#   $recursive   - True jeli ma uywa rekursywy to znaczy ciaga podkalogi i ich zawarto
####################################################################################################

Function Copy-Folder
{
    Param([string]$source, [string]$target, [string]$all = "true")

    [bool] $recursive = $False
    if($all -eq "true") { 
        $recursive = $True
    }

    if (!$(Test-Path($target))) {
        New-Item $target -type directory -Force
    }

    $webClient = New-Object System.Net.WebClient
    $webString = $webClient.DownloadString($source) 
    $lines = [Regex]::Split($webString, "</a>")

    foreach ($line in $lines) {
        if ($line.ToLower().Contains("href")) {
            if (!$line.ToLower().Contains("../")) {

                $items =[Regex]::Split($line, """")
                $items = [Regex]::Split($items[2], "(>|<)")
                $item = $items[2]

                if ($line.ToLower().Contains("/")) {
                    Write-Host "$item"
                    if ($recursive) {
                        Copy-Folder "$source$item/" "$target$item/" $recursive
                    } 
                } else {
                    Write-Host "$item"
                    $webClient.DownloadFile("$source$item", "$target$item")
                }
            }
        }
    }   
}

Copy-Folder -source "http://cygwin.mirror.constant.com/x86_64/" -target "C:\git\Utils\CygWinImage\x86_64"