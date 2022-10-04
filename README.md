# Leave Management System
A Project limited in functionality to demonstrate a three-tier system architecture.
## Installation and Usage
For the system to function It requires three things:
-	Microsoft SQL database should be running. 
-	The business logic and console application should be running.
-	The frontend nodejs server should be started.

The database can be created by running the SQL script in DataTier\DatabaseCreation.sql

Before it is run, ensure that the two paths in the file are correct, the first one, is the location where the database will be created. It should be inside the DATA folder of microsoft sql.
And the second is the location of the CSV folder, the location does not matter.

Make sure to run the console application as an Administrator (Either by starting visual studio as Administrator or by just running the app after its build as administrator)

Lastly, ensure nodejs and npm are installed on your system.
Change the directory to Frontend and run:
```shell
 npm i
```
Then start the node server by running:
```shell
node server.cjs
```
Or
```shell
nodemon
```
#### The app can create events in google calendar. To do so it requires a .env file in the root directory: FrontEnd.

The .env file should look like this:
```.env
CREDENTIALS = #Paste the contents of the json file created by google calendar api here
CALENDAR_ID = #your calendar id in the form: id@group.calendar.google.com
```
Change line 5 in FrontEnd->calendar.cjs to this:
```js
const CREDENTIALS = JSON.parse(process.env.CREDENTIALS);
```
#

The web app will then be hosted at port 3000 and it communicates with the business logic app that is listening on port 8080

Open with: http://localhost:3000
