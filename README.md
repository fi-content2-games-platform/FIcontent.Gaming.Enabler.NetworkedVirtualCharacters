FIcontent.Gaming.Enabler.NetworkedVirtualCharacters
===================================================

This repository contains the Networked Virtual Characters (NVC) SE of the FIcontent Pervasive Games Platform.

For documentation please refer to the FIcontent Wiki at http://wiki.mediafi.org/ in particular the documentation of the Pervasive Games Platform at http://wiki.mediafi.org/doku.php/ficontent.gaming.architecture


Modules
=====================

 * **FiVES-Plugin** for the Synchronization Server to handle bone transformations as attributes of entities
 * **WebClient-Plugin** to apply updates of the bone transformations to the 3D-model accordingly
 * **UnityClient-Plugin** to connect to the Synchronization Server and send a stream of bone updates

Installation
=====================

 * Include the different modules into the respective server/client
 * You may need to activate them,  i.e.
   * by including the .csproj file into the solution file
   * amend the client.html with additional script tags to load the plugin
     ''<!-- Networked Virtual Character SE -->''
     ''<script type="text/javascript" src="scripts/plugins/networkedvirtualcharacter/nvc.js"></script>''
   * trigger Unity to determine the additional files

Usage
=====================

 * generate a stream of bone transformations for your virtual character
   * by using some sensor (i.e. motion capturing)
   * by calculating physically-correct animation (i.e. rag-doll)
   * by using a motion synthesis library
 * broadcast this stream for a specific entity via the NVC API
   ''NVC.updateBones(string guid, List<Vector> translations, List<Quat> rotations, int timestamp)''
