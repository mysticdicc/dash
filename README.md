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

### Notes about AI Usage and Automation:
I have used Copilot to assist in the creation of some bug reports and enhancements to practice with things like branches and merge requests, I would not endorse doing this to other people's repos but as a creative resource for your own repos I have found it useful. Light AI assistance was used in the form of VS Copilot when creating the application, largely for problem solving and to avoid tabbing out to Google, and never in Agent mode. Less than 200 lines of this project were written by an LLM and 0 without human review.

I have also enabled dependabot and allowed it to create pull requests to ensure my Nuget packages and Dockerfile are kept up to date, the application will be retested before new Docker images are deployed with the updated files. I am experimenting with various GitHub workflows and you may see commits related to their configuration.

## License
Available under the [GPLv3](https://www.gnu.org/licenses/gpl-3.0.en.html#license-text) license
