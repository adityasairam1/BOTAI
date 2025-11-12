
#!/bin/bash

# Get migration name from argument or prompt if not provided
scriptName="$1"
while [ -z "$scriptName" ]; do
	read -p "Enter migration name: " scriptName
done

# Generate Migration Script
dotnet ef migrations add "$scriptName" \
	--project .\src\BOTAI ^
	--startup-project .\src\BOTAI ^
	--output-dir Generated\Migrations ^
	--context BOTAIContext

# Find the generated migration file (assumes default EF naming: <timestamp>_<MigrationName>.cs)
migrationDir="./src/BOTAI/Generated/Migrations"
migrationFile=$(find "migrationDir" -type f -name "*_${scriptName}.cs" | head -n 1)

# Create empty SQL file in Data/Scripts with the same name as the migration .cs file (including timestamp)
if [ -f "$migrationFile" ]; then
	sqlBaseName=$(basename "$migrationFile" .cs)
	sqlFile="./src/BOTAI/Generated/Scripts/${sqlBaseName}.sql"
	mkdir -p "src/BOTAI/Generated/Scripts"
	touch "$sqlFile"
	# Set SQL file timestamp to match migration file
	if touch -r "$migrationFile" "$sqlFile" 2>/dev/null; then
		echo "Created SQL file: $sqlFile with matching timestamp."
	else
		modTime=$(stat -f "%m" "$migrationFile")
		touch -t $(date -r $modTime +%Y%m%d%H) "$sqlFile"
	fi

	# Insert 'using System.IO;' as the first line if not present
	if ! grep -q "using System.IO;" "$migrationFile"; then
		awk 'NR==1{print "using System.IO;"}1' "$migrationFile" > "$migrationFile.tmp" && mv "$migrationFile.tmp" "$migrationFile"
	fi

	# Remove BOM if present
	perl -i -pe 's/^\xEF\xBB\xBF//' "$migrationFile"

	# Only if not already present
	upMethodStart=$(grep -n "protected override void Up" "$migrationFile" | cut -d: -f1)
	if [ -n "$upMethodStart" ]; then
		# Find the next '{' after Up method declaration
		braceLine=$(awk "NR>$upMethodStart && /{/{print NR; exit}" "$migrationFile")
		insertLine=$((braceLine + 1))

		# Prepare code to insert
		codeToInsert="            string scriptFilePath = Path.Combine(Directory.GetCurrentDirectory(), \"Data\", \"Scripts\", \"${sqlBaseName}.sql\");"

		# Check if already present
		if ! grep -q "Path.Combine(Directory.GetCurrentDirectory(), \"Data\", \"Scripts\", \"${sqlBaseName}.sql\")" "$migrationFile"; then
			# Insert code
			awk -v insertLine="$insertLine" -v code="$codeToInsert" 'NR==insertLine{print code}1' "$migrationFile" > "$migrationFile.tmp" && mv "$migrationFile.tmp" "$migrationFile"
		fi
	fi
else
	echo "Migration file not found. SQL file not created."
fi