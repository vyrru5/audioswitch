[AudioSwitch v2.0 RC4 Source code](https://drive.google.com/file/d/0B8VyHhqTufh1TjNKZ1lfcVcyTlU/view?usp=sharing)

[AudioSwitch v2.0 RC4 Setup](https://drive.google.com/file/d/0B8VyHhqTufh1dnk0Y3NNREVaWUk/view?usp=sharing)

All audio devices related events are now shown in AudioSwitch and I also tracked down a few memory leaks, this version seems to work really well for me!
Some feedback please! :)

AudioSwitch is now installed under user's local AppData folder with the settings file.
Right-click on the icon now shows the menu for settings and exit.

I think I fixed the device hiding, command-line now always exits and never runs GUI since it makes most sense to keep them separate.

Added also scrollbar for device list in settings

Mouse scroll could be working a bit better now

When last settings tab is visible then it's possible to drag the OSD window around the screen and the position is saved.



Please create or update an issue for any possible errors - I'd like to get as much bugs out from the release as possible :)


![https://audioswitch.googlecode.com/svn/wiki/demo.png](https://audioswitch.googlecode.com/svn/wiki/demo.png) ![https://audioswitch.googlecode.com/svn/wiki/demo-mic.png](https://audioswitch.googlecode.com/svn/wiki/demo-mic.png)

See  [changelog](https://audioswitch.googlecode.com/svn/wiki/changelog.txt) for recent updates!

[![](https://www.paypal.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=3X7L4G8Y9CUUG)

### Requirements ###
  * Windows 7 or newer, 32bit or 64bit
  * .NET 4.0 Framework
  * Supports both normal and medium DPI settings

### Installation ###
  1. [Download](https://drive.google.com/file/d/0B8VyHhqTufh1dnk0Y3NNREVaWUk/view?usp=sharing) latest version and run the setup.
  1. If you choose so, AudioSwitch will be launched after the setup

### Usage ###
  * **Left-Click** shows menu with available output devices, current default is the selected one in the list.
  * **Ctrl + Left-Click** shows recording devices and allows to switch between them with same behavior.
  * **Mouse Scroll** changes volume of the current device when the menu is visible.

  * **Right-Click** opens a menu for settings and exiting.
  * **Right-Click on volume track or handle** toggles mute - indicator of enabled mute is red highlight around volume handle and tray icon with a white x.

### Command-line options ###
![https://audioswitch.googlecode.com/svn/wiki/cmd.png](https://audioswitch.googlecode.com/svn/wiki/cmd.png)
  * **-m _key(s)_** -- will set these keys (Control,Alt,Shift) as modifier keys for a hot key.
  * **-k _key_** -- will set this key as hot key (in combination with modifier key(s)).
  * **-l** - 0-based list of available devices with their respective indexes.
  * **-i _n_** - set _n_'th device in list as default output.

Execution sequence is the same as the order of commands.

Examples:
  1. **AudioSwitch.exe -s** -- will run AudioSwitch in silent mode.
  1. **AudioSwitch.exe -i 1** -- will change 1'th device as default.
  1. **AudioSwitch.exe -i 0** -- will change 0'th device as default.
  1. **AudioSwitch.exe -l -i 0** -- will show device list, set 0'th device as default.
  1. **AudioSwitch.exe -i 1 -m control,Alt -k c** -- will set 1'th device as default and set hot key to Control + Alt + C.
  1. **AudioSwitch.exe -m Control,alt -k h** -- will **not** change hot key combination, just exits.

---

AudioSwitch is started from several similar projects and developed further to fit my personal vision of the final solution. Here are the sources which are used:
  1. http://www.codeproject.com/Articles/18520/Vista-Core-Audio-API-Master-Volume-Control - base library to interface between devices and C#. All copyright notices remain in files but I have removed/modified/optimized a lot of the code in some way.
  1. http://hardforum.com/showthread.php?t=1656534 - main start-up ideas and code for the initial GUI. I disassembled the GUI exe to get the initial code for the popup; I hope the original author/OP don't mind ;)

---

![http://www.jetbrains.com/resharper/img/rs179x67.gif](http://www.jetbrains.com/resharper/img/rs179x67.gif)