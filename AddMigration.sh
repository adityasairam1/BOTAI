#!/bin/bash

# Get migration name
scriptName="$1"
while [ -z "$scriptName" ]; do
    read -p "Enter migration name: " scriptName
done

# Run EF migration
dotnet ef migrations add "$scriptName" \
    --project ./BOTAI \
    --startup-project ./BOTAI \
    --output-dir Generated/Migrations \
    --context BOTAIContext

# Migration folder
migrationDir="./BOTAI/Generated/Migrations"

# Find the generated migration file
migrationFile=$(find "$migrationDir" -type f -name "*_${scriptName}.cs" | head -n 1)

if [ -f "$migrationFile" ]; then
    echo "Migration file found: $migrationFile"

    sqlBaseName=$(basename "$migrationFile" .cs)
    sqlDir="./BOTAI/Generated/Scripts"
    sqlFile="${sqlDir}/${sqlBaseName}.sql"

    mkdir -p "$sqlDir"
    touch "$sqlFile"

    echo "Created SQL file: $sqlFile"

    # Insert using System.IO;
    if ! grep -q "using System.IO;" "$migrationFile"; then
        sed -i '1i using System.IO;' "$migrationFile"
    fi

    # Insert code inside Up() method
    upMethodLine=$(grep -n "protected override void Up" "$migrationFile" | cut -d':' -f1)

    if [ -n "$upMethodLine" ]; then
        braceLine=$(awk "NR>$upMethodLine && /{/{print NR; exit}" "$migrationFile")
        insertLine=$((braceLine + 1))

        lineToInsert="            string scriptFilePath = Path.Combine(Directory.GetCurrentDirectory(), \"Data\", \"Scripts\", \"${sqlBaseName}.sql\");"

        if ! grep -q "${sqlBaseName}.sql" "$migrationFile"; then
            awk -v insertLine="$insertLine" -v new="$lineToInsert" \
                'NR==insertLine{print new}1' "$migrationFile" \
                > "$migrationFile.tmp" && mv "$migrationFile.tmp" "$migrationFile"
        fi
    fi

else
    echo "Migration file not found."
fi
