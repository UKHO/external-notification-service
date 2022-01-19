$reportFolder=$env:BUILD_ARTIFACTSTAGINGDIRECTORY
$source=$env:BUILD_SOURCESDIRECTORY

$ReportGenerator=dotnet tool list -g | ForEach-Object { if($_ -match "dotnet-reportgenerator-globaltool") {$_}}

if ([string]::IsNullOrWhiteSpace($ReportGenerator))
{
	echo "Report Generator not present Installing it ..."
	dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.8.13
} else
{
	echo "Report Generator present checking version ..."
	$ReportGeneratorVersion = ($ReportGenerator.split() | Where-Object {-not [string]::IsNullOrEmpty($_)})[1]

	echo "Report Generator Version: $ReportGeneratorVersion is Installed ..."

	if ( $ReportGeneratorVersion -ne "4.8.13" )
	{
		echo "Report Generator Version: $ReportGeneratorVersion is Installed required 4.8.13 replacing it ..."
		dotnet tool uninstall -g dotnet-reportgenerator-globaltool
		dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.8.13
	}
}

echo "Generating code coverage report ..."
reportgenerator "-reports:$source\**\coverage.cobertura.xml" "-targetdir:$reportFolder\codecoveragereport" "-reporttypes:HtmlInline_AzurePipelines;Cobertura"
echo "CoverageReport generated and published in $reportFolder\codecoveragereport folder