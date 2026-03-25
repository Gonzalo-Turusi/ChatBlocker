# ChatBlocker

Una aplicación de overlay simple para cubrir manualmente áreas de chat en juegos y aplicaciones.

## ¿Qué hace?

ChatBlocker es una ventana transparente que puedes posicionar sobre cualquier área de chat para bloquearla visualmente. No detecta nada automáticamente - es una herramienta manual de cobertura.

## Características

- **Overlay Manual**: Ventana que cubre el área que tú decidas
- **Transparente al Mouse**: Puedes hacer clic a través del overlay
- **Siempre Visible**: Se mantiene sobre todas las ventanas y juegos
- **Personalizable**: Color, tamaño, posición y opacidad ajustables
- **Hotkeys Configurables**: Asigna cualquier tecla con modificadores

## Controles

### Teclas por Defecto
- **F7**: Abrir configuración
- **F8**: Modo edición (mover/redimensionar)
- **F9**: Mostrar/ocultar overlay
- **F10**: Salir de la aplicación

### Modo Edición (F8)
- Arrastra el overlay para posicionarlo
- Arrastra las esquinas para redimensionar
- Borde rojo visible cuando estás en modo edición
- Información de posición y tamaño en pantalla

## Uso Básico

1. **Ejecutar**: `ChatBlocker.exe`
2. **Posicionar**: Presiona F8 → arrastra sobre el chat
3. **Ajustar**: Presiona F7 para configurar color, opacidad, teclas
4. **Usar**: Presiona F9 para mostrar/ocultar el overlay

## Configuración (F7)

### Asignación de Teclas
1. Haz clic en cualquier campo de tecla
2. Presiona la tecla que quieres usar
3. Soporta Ctrl, Alt, Shift como modificadores
4. Las teclas se guardan automáticamente

### Personalización Visual
- **Color**: Click en "Choose Color" → selecciona cualquier color RGB
- **Opacidad**: Slider 0-100% para ajustar transparencia
- **Reset**: Botón para restaurar valores por defecto

## Tecnologías Usadas

- **Lenguaje**: C# con .NET 10.0
- **Interfaz**: Windows Forms (Win32)
- **Gráficos**: System.Drawing para renderizado
- **Configuración**: JSON para guardar preferencias
- **Hotkeys**: API de Windows para registro global

## Cómo Compilar y Ejecutar

### Desde Código Fuente
1. Clona o descarga este repositorio
2. Abre terminal en la carpeta del proyecto
3. Ejecuta:
   ```bash
   dotnet build
   ```
4. Ejecuta:
   ```bash
   dotnet run
   ```

### Para Generar Ejecutable
1. Asegúrate de tener .NET 10.0 SDK
2. Ejecuta:
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
   ```
3. El ejecutable se genera en: `bin\Release\net10.0-windows\win-x64\publish\ChatBlocker.exe`

## Configuración por Defecto

```json
{
  "X": 16,
  "Y": 413,
  "Width": 421,
  "Height": 180,
  "OverlayColor": "Black",
  "OpacityWhenDetected": 1.0,
  "EditModeHotkey": { "Key": "F8", "Ctrl": false, "Alt": false, "Shift": false },
  "ToggleHotkey": { "Key": "F9", "Ctrl": false, "Alt": false, "Shift": false },
  "ExitHotkey": { "Key": "F10", "Ctrl": false, "Alt": false, "Shift": false }
}
```

## Problemas Comunes

**El overlay no se ve**:
- Presiona F8 para entrar en modo edición
- Verifica que el toggle esté activado (presiona tu tecla F9)

**Las teclas no funcionan**:
- Presiona F7 para verificar la asignación
- Asegúrate de hacer clic en "Save & Close"

**Problemas de rendimiento**:
- Reduce el tamaño del overlay
- Usa color negro (requiere menos renderizado)
- Cierra otras aplicaciones si es necesario

## Licencia

Este proyecto se proporciona tal cual está para uso educativo y personal.

---

**ChatBlocker** - Simple, directo y funcional.
