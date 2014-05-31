IP Tracker
========================

Console app that makes a web request to a service that provides the computer's public IP address. IP is logged to an IP history file. 

Background
---------------------
The IP address on my cable Internet connection at home kept changing. Usually the IP is fairly static - doesn't change that often - but I went through a period where it was changing all the time. This is a problem because I like to access my home desktop via Windows' Remote Desktop Connection - I've got the port forwarded through my router so I can access the machine from an external network. My quick and easy solution was to have a process log the IP address at home periodically, saving the log file to a Dropbox folder, which obviously syncs to Dropbox, which meant I could view the current IP as long as I had Dropbox access. 

How to Use
---------------------	

1. Build the solution
2. Take a copy of *IpTracker.exe* and *IpTracker.exe.config* from the build output and place them in a location from where you want the application to run
3. Open *IpTracker.exe.config* in a text editor
4. Enter values for *CurrentIpFilePathAndName* and *IpHistoryFilePathAndName* and save the config file. 
5. Create a Windows scheduled task to execute *IpTracker.exe" on the schedule of your choosing
6. Done