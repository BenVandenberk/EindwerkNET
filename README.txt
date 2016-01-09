Enkele opmerkingen:

1. Ik heb de hele projectfolder opgestuurd. Ik wist niet goed welke delen eventueel niet nodig waren (vs config? dependencies?)
2. De connection string is ingesteld op LocalDB. De database komt in de App_data folder. De databank het ik verwijderd (App_data is leeg)
3. De reeks stappen om de databank aan te maken:
	3.1 update-database uitvoeren. Dit zou de database 'UurFacDb.mdf' moeten aanmaken in App_data
	3.2 In de SQL server explorer new query -> bijgeleverde script uitvoeren om de testdata in te laden
4. Het klasse diagram is te bekijken via de applicatie zelf onder 'Developer', of in ~/Content/images