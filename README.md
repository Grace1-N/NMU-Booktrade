NMU BookTrade

Recreating the database using SSMS 
---



Use the attached SQL file “NMUBookTrade.sql”. The SQL file contains both the table structure and the data.   



1. Open SSMS: Launch Microsoft SQL Server Management Studio (SSMS). 



&nbsp;-Connect to Local Server.  

-server = localhost 

-authentication = Windows Authentication 

-databaseName = <default> 



2\. Create a New Database:  



-In object explorer, right click Database 



-Click New Database 



-Name it: NMUBookTrade  



-Click OK  



3\. Importing NMUBookTrade.sql Database 



-Click “New Query” 



-Paste contents of the NMUBookTrade.sql into the new query tab 



-Click “Execute” 

‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑

Extra Instructions
-when connecting to the serverName "localhost", make sure to comment out the first connection string of our live database in the web.config file in the solution explorer in VS Code. (this is how you will get the code for convenience.)

-for the live database, you would need to comment out the second connection string, that of the local database.
