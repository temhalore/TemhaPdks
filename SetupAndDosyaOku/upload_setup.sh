#!/bin/bash
# Setup dosyasını API'ye yükleme script'i

SETUP_FILE="d:/Development/ozel/TemhaPdks/SetupAndDosyaOku/Lore.SetupAndDosyaOku/installer/LoreDosyaIzleyici_Setup.exe"
API_URL="https://localhost:44374/Api/DataOkuConsoleSetup/uploadSetupFile"

# cURL ile dosya yükleme
curl -X POST \
  -H "Content-Type: multipart/form-data" \
  -F "file=@$SETUP_FILE" \
  $API_URL

echo "Setup dosyası başarıyla yüklendi!"
