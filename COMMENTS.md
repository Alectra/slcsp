Calculate second lowest cost silver plan (SLCSP)
================================================

Comments
-------

To see my output from this project, please see slcsp\bin\Release\assets\slcsp.csv
It has been updated by the program I have written in C# to complete the objectives.

To see the code for this project, please see slcsp\Program.cs - it is all contained
in one file for easy review.

This project was written in C# and is compilable via Microsoft Visual Studio.
To Compile:
	* Open slcsp.sln in Microsoft Visual Studio
	* Press the '> Start' button OR the F5 key to compile the runtime debug version
	* It will compile into this location: slcsp\bin\Debug\slcsp.exe

Notes
-------

I coppied the original slcsp.cvs to a new file called slcsp-original.cvs for quick reference
in this folder, so the program does not touch that file.

It will write over the file slcsp\bin\Debug\assets\slcsp.csv with the data compiled from 
plans.csv and zips.csv in the same folder.

If you rerun the program with the changed slcsp.csv file, but use a new(changed) plans.csv or
zips.csv, it will overwrite slcsp.csv with new rates based on those files.
