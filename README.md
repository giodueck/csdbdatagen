# csdbdatagen
dbdatagen rewritten in C#\
En general funciona de la misma forma que el script de Python, con la diferencia que deja a la base de datos manejar las claves primarias. Eso permite acelerar un poco el programa y correr varias instancias simultaneamente. 

# Diferencias con dbdatagen (Python)
- Mejor performance
- Ids recuperados de la base de datos con nextval o INSERT...RETURNING id, permite ejecutar varias instancias del programa simultaneamente.

# Construccion del proyecto
Para mantener el repositorio ordenado y legible no incluyo archivos como .csproj o directorios como /bin u /obj. Para contruir el proyecto se puede crear un nuevo proyecto en Visual Studio, o crear uno manualmente (opcionalmente con Visual Studio Code) siguiendo algunas de la instrucciones de este video: https://youtu.be/r5dtl9Uq9V0 \
Una vez generadas las carpetas, se debe usar NuGet para incluir el paquete Npgsql, que es la interfaz con PostgreSQL
