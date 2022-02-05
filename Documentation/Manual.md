## Manual

### Approve the INDM-App from the Microsoft Store
Sign in to the Microsoft Store for Business and approve the Intune Network Drive Mapping App.

<a href="https://businessstore.microsoft.com/store/details/networkdrivemapping/9NWVZR414XS6"><img src="https://developer.microsoft.com/store/badges/images/English_get-it-from-MS.png" alt="Get it from Microsoft" width="280"/></a>

### Assign the App from Microsoft Intune

Once the App is synchronized to the Microsoft Intune Apps you can assign it as required.

### Setup Autostart
The Network Drive Mapping App will register itself as an automatic startup app. However the autostart is disabled as long as the app has not been started at least once or has an explicit permission. To grant the app an explicit permission we need to setup a Restriction profile in Microsoft Intune.

You can create a new restriction profile or use an existing one. To permit the app you have to go into the App Store section and enter the following value to the startup apps textarea: <em>HaukeGtze.IntuneNetworkDriveMapping_6bk20wvc8rfx2</em>
<img src="https://github.com/Weatherlights/Intune-Network-Drive-Mapping-Tool/blob/7279c1542c8f514511c25afbfe58b4bd7061d54c/Documentation/img/configurestartup.png" alt="Configure startup app"/>

### Setup the ADMX-Template
Next you need to download and install the ADMX-Template using ADMX-Ingestion. To do this follow these simple steps:
1. Create a custom Windows 10 profile in Microsoft Intune
2. Add a new entry to this profile and fill in the values as followed:
3. For name use <em>Intune Network Drive Mapping ADMX-Ingestion</em>.
4. For the OMA-URI you enter the value ./Vendor/MSFT/Policy/ConfigOperations/ADMXInstall/weatherlights/Policy/IntuneNetworkDriveMapper
5. As value you copy and paste the content of the <a href="https://github.com/Weatherlights/Intune-Network-Drive-Mapping-Tool/blob/fc2196719d98263e91a426bac497ba04ad8906ee/ADMX-Template.admx">ADMX file</a>.
6. Confirm the entry.

You can now proceed by saving the profile read the next section about configuration and add those values to the profile aswell.

### Configure Intune Network Drive Mapping
To configure the Intune Network Drive Mapping you can create a new Windows 10 custom profile or use the one that ingested the ADMX file. You can add a new entry for each property you want to configure and enter the following values:
<table><tr><th>OMA-URI</th><th>Description</th><th>Default</th><th>Sample</th></tr>
  <tr><td>./User/Vendor/MSFT/Policy/Config/weatherlights~Policy~IntuneNetworkShareMapper~Configuration/Enabled</td><td>Activates the Intune Network Drive Mapping.</td><td>Disabled</td><td>&lt;enabled/&gt; or &lt;disabled/&gt;</td></tr>
   <tr><td>./User/Vendor/MSFT/Policy/Config/weatherlights~Policy~IntuneNetworkShareMapper~Configuration/RefreshInterval</td><td>Sets the refresh interval.</td><td>9000</td><td>&lt;enabled/&gt; or &lt;data id="RefreshInterval" value="9000"/&gt;</td></tr>
   <tr><td>./User/Vendor/MSFT/Policy/Config/weatherlightscom~Policy~IntuneNetworkShareMapper~Configuration/MapPersistent</td><td>Specifies if the network drive should be mapped persistence. Setting this value can increase reliability but is otherwise not needed.</td><td>Disabled</td><td>&lt;enabled/&gt; or &lt;disabled/&gt;</td></tr>
   <tr><td>./User/Vendor/MSFT/Policy/Config/weatherlights~Policy~IntuneNetworkShareMapper~Policy/NetworkDriveMappings</td><td>With this entry you define what network shares you would like to map. You can also provide a username and password (But not recommended since they are not encrypted). You need to specify this as followed: &lt;ENTRY NUMBER&gt;&amp;#xF000;&lt;DRIVE LETTER&gt;;&lt;PATH TO YOUR SHARE&gt;;&lt;Optional: USERNAME&gt;;&lt;Optional: PASSWORD&gt;&amp;#xF000; With &amp;#xF000; a new line can be added.</td><td></td><td>&lt;enabled/&gt;&lt;data id="NetworkDriveMappingsList" value="1&amp;#xF000;P; \\server1.domain.local\share2&amp;#xF000;2&amp;#xF000;F;\\server1.domain.local\share1;Username;P@assword"/&gt;</td></tr></table>
   <img src="https://github.com/Weatherlights/Intune-Network-Drive-Mapping-Tool/blob/27749ebfe8da8cdb6807b0b1c04d4aa1b9abfe4f/Documentation/img/editrow.png" alt="Adding a row"/>

### How does the Tool work
This is how the Intune Network Drive Mapping checks for new drives:
* In Intune Network Drive Mapping tool checks wether the network configuration has changed and if so
* Will try to connect the drives up to 15 times.
* If a networkdrive is already connected the Intune Network Drive Mapping will check wether the drive maps the currently configured network path.
   * If the network path matches the configuration the drive will be ignored.
   * If the network path does not match the currently configured path the drive will be reconnected.
* When 15 tries are used up the Intune Network Drive Mapping will go to sleep and wait until another network configuration change will occur.

### Troubleshooting
#### A network drive does not show up
* Check wether the drive letter is already in use by another drive (USB drive or hard disk).
* The configured path may not be connected or not accessible
* There may be a problem with the permissions on the network path. The user may not have permission to access the location. You can verify this by entering the network path in the explorer address bar and see if it can be accessed.

#### How can I verify that the configuration is already active?
You check the following registry key: HKEY_CURRENT_USER\Software\Policies\weatherlights.com
