# csdbdatagen
dbdatagen rewritten in C#\
En general funciona de la misma forma que el script de Python, con la diferencia que deja a la base de datos manejar las claves primarias. Eso permite acelerar un poco el programa y correr varias instancias simultaneamente. 

# Diferencias con dbdatagen (Python)
- Mejor performance
- Ids recuperados de la base de datos con nextval o INSERT...RETURNING id, permite ejecutar varias instancias del programa simultaneamente.
