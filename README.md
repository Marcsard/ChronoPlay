# ChronoPlay

**ChronoPlay 1.0.0** es una aplicación de escritorio para administrar la operación diaria de un centro de entretenimiento con consolas.

## Funciones

- Administración de consolas, sesiones, tiempos de juego y tarifas.
- Inicio de sesión y administración de usuarios.
- Inventario de productos, proveedores, compras y ventas.
- Registro de movimientos de caja, gastos y cortes diarios.
- Historial de actividad y reportes mensuales.
- Base de datos local SQLite en la versión publicada.

## Requisitos

- Windows 10 o Windows 11 de 64 bits.
- Para ejecutar la publicación autónoma, no hace falta instalar .NET por separado.
- Para compilar desde el código fuente: .NET 10 SDK y Visual Studio 2026 con las herramientas de escritorio .NET/WPF.

## Descargar e instalar

Descarga el instalador minimalista para Windows x64 desde [Releases](https://github.com/KrizMet66/ChronoPlay/releases/latest). La instalación es por usuario y no requiere privilegios de administrador ni instalar .NET. También se publica el ejecutable portátil independiente y los hashes SHA-256.

El instalador crea un acceso directo en el menú Inicio; el acceso directo del escritorio es opcional. Al desinstalar se quita la aplicación, pero se conservan los datos del usuario.

## Ejecutar y conservar los datos

La aplicación publicada guarda su base de datos en:

```text
%LOCALAPPDATA%\ChronoPlay\ChronoPlay.db
```

Esta ruta pertenece al usuario de Windows que inicia ChronoPlay. Para abrir una instalación y sus datos existentes, ejecuta la aplicación con la misma cuenta de Windows. Cambiar de cuenta cambia la carpeta `AppData\Local` utilizada.

Los registros no se guardan junto al ejecutable ni se eliminan al desinstalar. Antes de reemplazar una instalación, cierra ChronoPlay y conserva una copia de `ChronoPlay.db` y de la carpeta `%LOCALAPPDATA%\ChronoPlay`.

En el primer inicio de una instalación sin base de datos se crea la base local y se solicita crear la cuenta administradora inicial. No se configura una contraseña predeterminada.

> La configuración de desarrollo (`Debug`) usa SQL Server LocalDB. La publicación (`Release`) usa SQLite. No se asume que los datos de LocalDB se migran automáticamente a SQLite.

## Compilar

Desde la raíz del repositorio:

```powershell
dotnet restore .\ChronoPlay\ChronoPlay.csproj
dotnet build .\ChronoPlay\ChronoPlay.csproj --configuration Release
```

Para publicar un ejecutable autónomo de Windows x64 con el icono de ChronoPlay:

```powershell
dotnet publish .\ChronoPlay\ChronoPlay.csproj --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

El archivo generado queda en:

```text
ChronoPlay\bin\Release\net10.0-windows\win-x64\publish\ChronoPlay.exe
```

Para crear localmente el instalador (requiere [Inno Setup 6](https://jrsoftware.org/isinfo.php)):

```powershell
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /DAppVersion=1.0.0 .\installer\ChronoPlay.iss
```

El instalador se genera en `artifacts\ChronoPlay-Setup-1.0.0-win-x64.exe`. El paquete se crea con [Inno Setup](https://jrsoftware.org/isinfo.php).

## Publicar una actualización

La rama `main` es la rama estable. Para cada versión:

1. Actualiza `Version`, `AssemblyVersion`, `FileVersion` e `InformationalVersion` en `ChronoPlay/ChronoPlay.csproj`.
2. Integra los cambios y envíalos a `main`.
3. Crea y sube una etiqueta que coincida exactamente con la versión, por ejemplo `v1.0.1`:

   ```powershell
   git tag -a v1.0.1 -m "ChronoPlay 1.0.1"
   git push origin v1.0.1
   ```

GitHub Actions compila el ejecutable autónomo, lo empaqueta en el instalador y publica el instalador, el ejecutable portátil y sus sumas SHA-256 en GitHub Releases. El flujo valida que la etiqueta y la versión del proyecto coincidan.

## Estructura

- `ChronoPlay/Views`: ventanas y pantallas WPF.
- `ChronoPlay/Controls`: controles reutilizables.
- `ChronoPlay/Services`: lógica de aplicación y acceso a datos.
- `ChronoPlay/Data`: configuración de base de datos y rutas locales.
- `ChronoPlay/Models`: entidades de la aplicación.
- `ChronoPlay/Migrations`: historial de cambios del modelo de datos.
- `ChronoPlay/Themes`: estilos, colores e iconos de interfaz.

## Fuentes

Las fuentes Orbitron, Chakra Petch y Share Tech Mono son opcionales. Consulta [`ChronoPlay/Fonts/README.txt`](ChronoPlay/Fonts/README.txt) para los requisitos de licencia e integración. Si no están instaladas, la interfaz utiliza fuentes de respaldo.
