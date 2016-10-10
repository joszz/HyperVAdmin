Hyper-V Admin
================
Copyright (c), 2016, Jos Nienhuis

A little website to control and check your Hyper-V VMs and IIS websites with.

Screenshots
------------
![Screenshot](https://raw.githubusercontent.com/joszz/HyperVAdmin/master/Content/Images/Screenshots/Home.jpg "Home")

Prerequisites
-------------
- IIS
- .NET Framework 4.5

Installation
------------
Just create a website or application in IIS as you normally would. Point the path to the location of this project's files.

Make sure the Application Pool of that website is running under a user with admin priviliges. The default Application Pool will not have enough rights. Look here for more information;

https://technet.microsoft.com/en-us/library/cc731755(v=ws.10).aspx

Configuration
-------------
In the web.config of this project there are a couple settings you can change;

- **AlertTimeout**

	The time the Bootstrap alert box will be displayed in seconds.

- **RefreshInterval**

	The time between automatic AJAX enabled refresh of list content in seconds.

- **HyperVManagementPath**

	The "\v2" can be stripped of to support older Hyper-V installations.

- **HyperVQueryVMs**

	The WMI query used to retrieve Hyper-V VMs.

- **compilation debug="false"**

	Change this to true to turn of minification (amongst others). False will minify JS and CSS.