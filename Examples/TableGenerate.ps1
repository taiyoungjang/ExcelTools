# file: test.ps1
# test PowerShell Script
# winrm quickconfig
# Set-Item wsman:\localhost\Client\TrustedHosts -value * 
# new-itemproperty -path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System" -name "LocalAccountTokenFilterPolicy" -propertyType "DWord" -value 1
# enable-psremoting
# read-host -assecurestring | convertfrom-securestring | out-file .\pass.txt
#Register-PSSessionConfiguration -Name DataNoLimits 
#Set-PSSessionConfiguration -Name DataNoLimits -MaximumReceivedDataSizePerCommandMB 500 -MaximumReceivedObjectSizeMB 500

param([string] $ExcelFile = "",[string]$TargetName = "",[string]$CommandName = "table",[string]$PlatformType = "",[int]$CPU_Count=0,[string] $Language = "")

[Environment]::CurrentDirectory=(split-path -parent $MyInvocation.MyCommand.Definition)

# --------------------------------------------------------------------
# Checking Execution Policy
# --------------------------------------------------------------------
#$Policy = "Unrestricted"
$Policy = "RemoteSigned"
If ((Get-ExecutionPolicy) -ne $Policy) {
  Write-Host "Script Execution is disabled. Enabling it now"
  Set-ExecutionPolicy $Policy -Force
  Write-Host "Please Re-Run this script in a new powershell environment"
  Exit
}

filter replace-slash {$_ -replace "\\", "/"}

$CurrentFolder = (get-item ([Environment]::CurrentDirectory) ).Name
$CurrentDirectory = (get-item ([Environment]::CurrentDirectory) ).FullName
# ********************************
#$PlatformType = "Mixed Platforms"
$LangVers = [array] "English", "Korean"
$nl = [Environment]::NewLine
$hostname=[Environment]::MachineName
if($CPU_Count -eq 0)
{
	$CPU_Count = (Get-WmiObject Win32_Processor).NumberOfLogicalProcessors
}

function Building_Table
{
	param($LangVers)

    #if( (test-path $Project_Source_Path) -eq $False )
    #{
    #    Write_Error_Message
    #    return
    #}

    $Project_Table_Path = "${CurrentDirectory}\Excel\"
	$EnumTypesPath = "${CurrentDirectory}\Enum\"

    $TableInput_Path = $Project_Table_Path

    foreach($LangVer in $LangVers) 
    { 
    # set Table {
        $Project_Table_Generater_Path = "${CurrentDirectory}\..\TableGenerateCmd.NetCore\bin\Debug\netcoreapp3.0\" 
        $Project_Table_Generater_File = "${Project_Table_Generater_Path}TableGenerateCmd.dll"
        $Project_Table_Generater_INI_File = "${CurrentDirectory}\TableGenerateCmd.ini" 
        [array] $Project_Table_Generater_Command = "-i", $Project_Table_Generater_INI_File, "-c", "c#", "table", "-l", "$LangVer", "-a", "unity3d", "-p", $CPU_Count
    
        $Project_Table_Binaries_Path = "${CurrentDirectory}\Bytes\${LangVer}\" 
        $Project_CS_DLL_Path = "${Project_Source_Path}BytesDll" 

	    $CShsarp_Table_Path = "${CurrentDirectory}\Scripts\"
	
		if( $ExcelFile -eq "")
		{
			$ExtType = "*.xls*"
		}
		else
		{
			$ExtType = $ExcelFile
		}
	
        $Project_Table_Generater_INI = "[TableGenerate]",
    	    "TableInput=${TableInput_Path}",
			"DllOutputPath=${Project_CS_DLL_Path}",
    	    "ExtType=${ExtType}",
    	    "IgnoreCase=[~*]",
            "Except=design,desc,server",
    	    "[Directory]",
            "TableFile=$Project_Table_Binaries_Path", 
		    "CS=$CShsarp_Table_Path",
		    "CSMGR=$CShsarp_Table_Path",
			"ENUMTYPES=$EnumTypesPath"
    # } // Table

        write-host "`n** Execute TableGenerate ${LangVer} **`n" -foregroundcolor Blue -backgroundcolor White
        out-file -filepath $Project_Table_Generater_INI_File -inputobject $Project_Table_Generater_INI
	    Invoke-Expression "dotnet $Project_Table_Generater_File $Project_Table_Generater_Command"
	    if( $LastExitCode -ne 0 )
	    {
		    write-host "exit_code:"$LastExitCode.ToString()
			pause
		    exit $LastExitCode
	    }
    }
    #Invoke-Expression "./ReferenceTable.ps1"
	#remove-item $Project_Table_Generater_INI_File -Force -Recurse
}

Building_Table -LangVers $LangVers
