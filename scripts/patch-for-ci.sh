#!/usr/bin/env bash
set -euo pipefail

# Patch Unity project for CI compatibility
# Removes Tuanjie Engine specific packages and fixes version number

PROJECT_DIR="${1:-.}"

echo "[CI Patch] Patching project for standard Unity CI..."

# 1. Fix ProjectVersion.txt: 2022.3.61t14 -> 2022.3.61f1
VERSION_FILE="$PROJECT_DIR/ProjectSettings/ProjectVersion.txt"
if [ -f "$VERSION_FILE" ]; then
    sed -i 's/2022\.3\.61t14/2022.3.61f1/g' "$VERSION_FILE"
    sed -i '/m_TuanjieEditorVersion/d' "$VERSION_FILE"
    echo "[CI Patch] ProjectVersion.txt patched to standard Unity 2022.3.61f1"
fi

# 2. Remove Tuanjie-specific packages from manifest.json
MANIFEST_FILE="$PROJECT_DIR/Packages/manifest.json"
if [ -f "$MANIFEST_FILE" ]; then
    cp "$MANIFEST_FILE" "$MANIFEST_FILE.bak"
    python3 -c "
import json, sys
with open('$MANIFEST_FILE', 'r') as f:
    manifest = json.load(f)

removed = []
for key in list(manifest['dependencies'].keys()):
    if key.startswith('cn.tuanjie') or key == 'com.coplaydev.unity-mcp':
        del manifest['dependencies'][key]
        removed.append(key)

with open('$MANIFEST_FILE', 'w') as f:
    json.dump(manifest, f, indent=2)

print(f'[CI Patch] Removed {len(removed)} Tuanjie/MCP packages: {', '.join(removed)}')
"
fi

echo "[CI Patch] Done. Project is now compatible with standard Unity 2022.3 LTS."
