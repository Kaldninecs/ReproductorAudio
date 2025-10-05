# 🎵 Reproductor de Audio By Nicolas Ayala

Un reproductor de audio moderno y elegante para Windows que soporta archivos locales y reproducción/descarga directa desde YouTube.

![Version](https://img.shields.io/badge/version-1.0-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![License](https://img.shields.io/badge/license-MIT-green)

## ✨ Características

- 🎵 **Reproducción de archivos locales**: MP3, WAV, M4A, AAC, WMA, FLAC, WebM, OPUS
- 🌐 **Reproducción desde YouTube**: Carga videos directamente desde YouTube
- ⬇️ **Descarga de audio**: Descarga canciones de YouTube en alta calidad
- 🎨 **Interfaz moderna**: Diseño oscuro y minimalista
- 🔊 **Control de volumen**: Ajuste preciso del volumen
- ⏯️ **Controles completos**: Play, Pause, Stop, Adelantar, Retroceder
- 📊 **Barra de progreso**: Visualiza y navega por la canción

## 📥 Descargar

### [⬇️ Descargar Instalador v1.0](../../releases/download/v1.0/ReproductorAudio_Setup_v1.0.exe)

## 🖼️ Capturas de pantalla

![Interfaz principal](screenshot.png)

## 🔧 Requisitos del sistema

- Windows 10 o superior
- .NET 8.0 (incluido en el instalador)

## 📦 Instalación

1. Descarga el instalador desde [Releases](../../releases)
2. Ejecuta `ReproductorAudio_Setup_v1.0.exe`
3. Sigue el asistente de instalación
4. ¡Disfruta tu música! 🎶

## 🎯 Cómo usar

### Reproducir archivos locales
1. Click en **"📁 Cargar Archivo"**
2. Selecciona tu archivo de audio
3. Click en **▶️** para reproducir

### Reproducir desde YouTube
1. Click en **"🌐 YouTube"**
2. Pega la URL del video
3. Espera a que se descargue
4. Click en **▶️** para reproducir
5. (Opcional) Click en **"⬇️ Descargar Canción"** para guardarla

## 🛠️ Tecnologías utilizadas

- **C# / WPF** - Interfaz de usuario
- **.NET 8.0** - Framework
- **NAudio** - Reproducción de audio
- **yt-dlp** - Descarga de YouTube

  ## 🔧 Solución de problemas

### ❌ Error: "No se encontró yt-dlp.exe"

**Causa:** El programa no puede encontrar el archivo yt-dlp.exe necesario para descargar de YouTube.

**Solución:**
1. Descarga `yt-dlp.exe` desde: https://github.com/yt-dlp/yt-dlp/releases/latest
2. Copia el archivo en la carpeta de instalación del programa (generalmente `C:\Program Files\ReproductorAudio\`)
3. Reinicia el programa

---

### ❌ Error: "Response status code does not indicate success: 403 (Forbidden)"

**Causa:** YouTube está bloqueando las solicitudes o yt-dlp necesita actualización.

**Soluciones:**
1. **Actualizar yt-dlp:**
   - Descarga la última versión de yt-dlp.exe
   - Reemplaza el archivo antiguo en la carpeta del programa
   
2. **Verificar la URL:**
   - Asegúrate de que la URL sea correcta
   - Prueba con otro video
   - Algunos videos tienen restricciones regionales o de edad

3. **Verificar conexión a internet:**
   - Asegúrate de estar conectado a internet
   - Prueba abrir YouTube en tu navegador

---

### ❌ Error: "Error al cargar el archivo"

**Causa:** El archivo de audio está corrupto o en un formato no soportado.

**Solución:**
1. Verifica que el archivo sea un formato soportado: MP3, WAV, M4A, AAC, WMA, FLAC, WebM, OPUS
2. Intenta abrir el archivo con otro reproductor para verificar que funcione
3. Si el archivo está dañado, intenta descargarlo nuevamente

---

### ❌ No se escucha el audio

**Causas posibles:**

1. **Volumen del programa en 0:**
   - Verifica el control de volumen en la parte inferior del reproductor
   - Ajústalo a un nivel audible (50% o más)

2. **Volumen del sistema en silencio:**
   - Verifica el volumen de Windows en la barra de tareas
   - Asegúrate de que no esté en mute

3. **Dispositivo de audio incorrecto:**
   - Verifica que tus altavoces/audífonos estén conectados
   - En Windows, verifica el dispositivo de reproducción predeterminado

---

### ❌ El programa se congela al cargar desde YouTube

**Causa:** La descarga está en progreso o hay problemas de conexión.

**Solución:**
1. **Es normal:** La primera descarga puede tardar 10-30 segundos dependiendo de tu conexión
2. **Espera:** El mensaje "Descargando desde YouTube..." indica que está trabajando
3. **Si tarda demasiado:**
   - Verifica tu conexión a internet
   - Prueba con un video más corto
   - Reinicia el programa e intenta nuevamente

---

### ❌ Error: "El video no se pudo descargar"

**Causas posibles:**

1. **Video privado o eliminado:**
   - El video ya no está disponible en YouTube
   - El video es privado o tiene restricciones

2. **Video con restricción de edad:**
   - Algunos videos requieren verificación de edad
   - yt-dlp podría no poder acceder

3. **Transmisión en vivo:**
   - Las transmisiones en vivo activas pueden causar problemas
   - Espera a que termine y se guarde como video normal

**Solución:** Prueba con otro video o URL diferente.

---

### ❌ El botón de descarga no aparece

**Causa:** Solo aparece cuando cargas música desde YouTube, no para archivos locales.

**Solución:** 
- Carga un video desde YouTube usando el botón "🌐 YouTube"
- El botón "⬇️ Descargar Canción" aparecerá automáticamente después de cargar

---

### ❌ Error: "Access denied" o problemas de permisos

**Causa:** El programa no tiene permisos para escribir en ciertas carpetas.

**Solución:**
1. Ejecuta el programa como Administrador:
   - Click derecho en el acceso directo
   - Selecciona "Ejecutar como administrador"
   
2. Al guardar archivos, elige una ubicación donde tengas permisos (Documentos, Descargas, etc.)

---

### ❌ La barra de progreso no se mueve

**Causa:** El archivo de audio podría no tener información de duración correcta.

**Solución:**
1. El audio seguirá reproduciéndose normalmente
2. Intenta con otro archivo para verificar
3. Algunos formatos (como ciertos WebM) pueden tener este problema

---

### ⚠️ Advertencia de Windows Defender o Antivirus

**Causa:** Windows Defender puede marcar yt-dlp.exe como sospechoso porque descarga contenido de internet.

**Solución:**
1. Es un **falso positivo** - yt-dlp es seguro y de código abierto
2. Agrega una excepción en Windows Defender:
   - Windows Security → Protección contra virus y amenazas
   - Administrar configuración → Exclusiones
   - Agregar la carpeta del programa

---

### 💡 Consejos adicionales

- **Mantén yt-dlp actualizado:** YouTube cambia frecuentemente, descarga la última versión de yt-dlp cuando tengas problemas
- **Formatos recomendados para archivos locales:** MP3 y M4A tienen mejor compatibilidad
- **URLs de YouTube:** Funciona tanto con `youtube.com/watch?v=...` como con `youtu.be/...`
- **Calidad de descarga:** El programa descarga automáticamente la mejor calidad de audio disponible

---

## 📝 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

## 👨‍💻 Autor

**Nicolas Ayala**

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:
1. Haz un Fork del proyecto
2. Crea una rama para tu característica (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## ⭐ Dale una estrella

Si te gusta este proyecto, ¡dale una estrella! ⭐

---

Hecho con ❤️ por Nicolas Ayala
