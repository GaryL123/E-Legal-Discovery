# E-Legal-Discovery

updates to the files. currently implemented:
-------------------------------------------------------------------------------------------------------------------------
--users are able to be added to the database

--users are able to login, authentication works

--emails can be uploaded the the database. the metadata is extracted and stored in the db, the original file is saved to a 
  path, and the copy of the original (to be modified) is saved to a different path

--emails can be downloaded from the db. user specifies the download path and the email_id they want, and the file is
  downloaded to the specified path

--UI now includes an option for the user to create a project. it prompts the user for the project details, and stores them 
  in the database. it currently prompts the user about adding emails to the project during creation, but this is not yet implemented
  
--option to view emails pertaining to the open project implemented. button to edit the email sketched out, but commented out for now.

****BUGS****
-projects are only showing based on who is logged in/ which project they created
-project doesnt reset when the user logs out
