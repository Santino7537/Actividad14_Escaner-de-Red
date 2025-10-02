# Actividad14_Escaner-de-Red
Este repositorio será utilizado para hacer la Actividad 14 de Redes
## Martes 05/08
Se investigó como utilizar C#, y posteriormente se hizo una pequeña interfaz que todavia no cumple todas las funciones que el trabajo pide.
Se hicieron 4 archivos utilizando el modelo vista controlador, en el cual MainForm.cs es la vista, MainFormController es el controlador, IPv4Validator es solamente una clase utilizada por el controlador, y Program es el inicializador del programa.
## Lunes 11/08
Se investigó mucho el como utilizar una librería "System.Diagnostics.Process", que la utilizaré cuando la comprenda del todo y pueda hacer el código por completo.
Además, se intento implementar algunos métodos y una clase para poder llevar a cabo al parte lógica del programa, pero todavia no funciona bien.
## Martes 12/08
Se investigó más a fondo sobre como ejecutar los comandos de cmd sobre C# con librerias (System.Diagnostics.Process es una de ellas).
Además, se trabajó sobre los métodos comentados del Lunes 11/08, y se agregó una nueva vlase que sirve para guardar los resultados de un escaneo de IP.
## Lunes 18/08
Se terminó la aplicación, cumpliendo todos los requisitos del trabajo práctico en cuanto a la parte del programa, solucionando varios errores, añadiendo y eliminando clases, agregando nuevos métodos, y optimizando partes del código en el proceso. Se añadieron 2 pdf al repositorio, uno es el manual para el usuario así sabe utilizar la aplicación, y el otro es la documentación del desarrollo.
## Jueves 25/08
Solamente se agregó un botón.
## Lunes 29/09
Se agregó una clase para comparar distintos tipos de IPs, que son resultado del comando "netstat". Se agregó una vista la cuál está programada
para ejecutar y mostrar el resultado de un netstat con las posibles flags -a, -n, y -o. Se modificó el controlador para que pueda ejecutar el
netstat. Se modificó la vista principal para tener un botón que abra y cierra la ventana de netstat.
Todavía estas clases no son totalmente funcionales, solo son una base para poder hacer el netstat.
## Jueves 2/10
Se siguió trabajando sobre las clases ya creadas, y para poder aplicar cierta lógica en el código, se tuvo
que crear una interfaz. Se logró que la nueva vista muestre la ejecución del comando netstat, y que permita
seleccionar distintas flags para este comando, y también permite ordenar y filtrar la tabla de resultados.
La documentación y el manual de usuario se actualizaron.
## Instrucciones
Para poder abrir el programa se debe:

- Descargar el archivo comprimido "IPScanner".
- Extraiga el contenido en una carpeta de su preferencia.
- Entre a la carpeta "IPScanner", y seguir la ruta "bin/Debug/net8.0-windows".
- Ejecutar el archivo "IPScanner.exe".

Opcional:

- Crear un acceso directo de "IPScanner.exe".
- Mover el acceso directo al Escritorio preferentemente.
- Ejecutar el acceso directo.