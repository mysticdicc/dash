# dash
Sinple homelab dashboard and monitoring tool. Created with Blazor and .NET 10.

<img src="/gitimages/dashboard_home.png" style="width: 50%">

## Getting Started
The dashboard is packaged as a docker container, either run the following commands from your preferred terminal or download mysticdicc/homelab-dashboard:latest from docker hub.

### Docker Run
<pre>
docker run -d \
  --name=homelab-dashboard \
  -p 8080:8080 \
  --restart unless-stopped \
  mysticdicc/homelab-dashboard:latest
</pre>
  
### Docker Run (With DB Persistence)
<pre>
docker run -d \
  --name=homelab-dashboard \
  -p 8080:8080 \
  --restart unless-stopped \
  -v /local/path/for/config:/app/data \
  mysticdicc/homelab-dashboard:latest
</pre>

Once running you can access the website at http://dockerip:8080 by default, if you have changed the port you will need to reflect this in the URL.

## Change Log
### **v2 (December 2025)**:
- **GUI Makeover**:
  - All Radzen components removed and replaced with pure html and css components, created css from scratch.
  - Flat modern aesthetic, dark colour scheme.
  - Lots of custom components created.
  - Lots of work to improve scaling across screen sizes.
  - Tool tips added to a lot of buttons.
    
- **Dashboard Rework**:
  - Standardised size of the icons.
  - Added the ability to create folders and assign shortcuts to them.
     
- **Better Error Handling**:
  - Custom HTTP Request Handler class added with retry logic and standardized json handling.
  - Created notification pop up and service that displays errors for api events.

- **Monitoring Rework**:
  - Replaced Radzen Charts with ApexCharts.
  - Added more charts for viewing uptime vs downtime overtime and total uptime.
  
- **Other**:
  - Removed Newtonsoft.Json, replaced with standard .net json implementations.
  - Updated from .NET 9 to 10.
  - Seperation of a lot of code into relevent and maintainable classes and folder structure.
  - Added settings screen to update monitor polling interval.
  - Added "dash" branding.
  - Removed a lot of unused code.

- **Bug Fixes**:
  - Fixed lot of css issues.
  - Fixed issues with charts rendering.


### **v1 (May 2025)**: 
- Dashboard, Subnet Tracker and Monitoring pages and api endpoints created. 
- Implemented with Radzen Blazor components

## Gallery:
### Home:

| ![home](/gitimages/dashboard_home.png) | ![home_folder](/gitimages/dashboard_home_folder.png) |
|:---:|:---:|
| Home Screen - You can create shortcuts and folders here, if you click on the card it will take you to the url saved. There is no limit to the number of either you can have, folders will likely load at the bottom. | Folder View - Just like on the main card screen you can edit cards using the pencil directly from inside the folder view. Folders can have their own icons and descriptions just like shortcuts. |

### Subnet Tracker:
| ![subnets](/gitimages/subnets.png) | ![subnets_open](/gitimages/subnets_open.png) |
|:---:|:---:|
|  Subnets - From here you can create new subnets for monitoring. You can create as many as you like, the subnets can be as large as you like. | You can press the 3 lines button to expand the IP list and scroll through or search the IP addresses for the subnet. |

| ![subnet_creator](/gitimages/subnets_creator.png) | ![ip_editor](/gitimages/subnet_ipeditor.png) | 
|:---:|:---:|
| To generate a new subnet, enter a subnet in CIDR notation format (eg 192.168.0.1/24 will generate IPs 192.168.0.0 - 255). Press the arrow to generate the IPs. If you wish you can open the IP list and delete specific IPs before saving it. You can press the circle arrow icon to start an automated discovery of the subnet which will auto monitor online devices and attempt a DNS lookup for the IP address. | Once you select edit on a specific IP you will be shown this screen where you can toggle various kinds of monitoring and set a hostname which can be used as a friendly display name for the IP across the application. |


### Monitoring:
| ![monitoring](/gitimages/monitoring.png) |
|:---:|
| The monitoring screen has a summary of all of the statistics the monitoring service has gathered over time. It will show you a heat map of online vs offline devices over time, a summary of your uptime across devices and a line chart showing total numbers of devices online over time. You can select an IP from the list and it will open some more information about it, where you can also edit the IP address without needing to go back to the subnets screen. You can also view the monitor history for the device to see any previous times it has been polled and what the status was. |

### Misc:
| ![settings](/gitimages/settings.png) | ![notification](/gitimages/notification.png) 
|:---:|:---:|
| The settings window currently can be used to set the delay between polls from the monitoring service. More functionality planned soon. | This is an example of the notifications you will receive from the application to inform you of any errors etc. |

## License
Available under the [GPLv3](https://www.gnu.org/licenses/gpl-3.0.en.html#license-text) license
