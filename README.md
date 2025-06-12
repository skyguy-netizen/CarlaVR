## Carla VR Streaming in Unity

- CarlaVRController.cs:
  - Handles reading websocket stream at ```ws://\<IP address of machine running Carla API\>:<port default is 8765```
  - The port to listen on is set in the CarlaPythonAPI repo in the script  ```manual_control_streaming.py```
  - Make sure to accordingly change the IP address for the machine as well as the port
 
- Build project and run on Meta Quest to connect to the websocket defined in the code
- Only need to build and run again if the IP address and the port need to be changed

- In the future, can add a screen on startup that lets user choose which machine to connect to (in case there are multiple machines running the Carla Simulator)
