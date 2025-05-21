#!/bin/bash

# Script de construcción para Render
echo "===== Iniciando script de construcción para Render ====="

# Si estamos en entorno de Render, usar archivo de proyecto alternativo
if [ "$RENDER" = "true" ]; then
  echo "Entorno de Render detectado - usando archivo de proyecto compatible"
  if [ -f "TiengAnh.csproj.render" ]; then
    cp TiengAnh.csproj.render TiengAnh.csproj
    echo "Archivo de proyecto reemplazado para compatibilidad con .NET 8.0"
  fi
fi

# Restaurar paquetes con registro detallado
echo "Restaurando paquetes..."
dotnet restore --disable-parallel || { echo "Error durante restauración de paquetes"; exit 1; }

# Compilar proyecto
echo "Compilando proyecto..."
dotnet build --configuration Release || { echo "Error durante compilación"; exit 1; }

# Publicar proyecto
echo "Publicando proyecto..."
dotnet publish -c Release -o /app || { echo "Error durante publicación"; exit 1; }

echo "Construcción completada exitosamente!"
