# vs-mono

A plugin that provides a custom C# project flavor that allows you to build and remote-debug mono programs hosted on a non-windows machine.  

## Download



## Intended Audience

Those who prefer to develop C# programs in the comfort of Visual Studio, but need to deploy their code to a Linux-based machine.  Of particular use to people using mono on embedded systems (such as Rasberry Pi or Lego EV3) on which debugging on the target itself is unfeasible.

## Challenges When not Using vs-mono

Out of the box, there is no way to debug a program running under mono on a Linux machine from Visual Studio.  However, mono does provide a means for remote-debugging.  When running the program on Linux, you can specify a debug port to which a debugger could attach.  But:

* By default, Visual Studio does not know how to talk to the mono debug engine
* The mono debug engine *requires* `.mdb` files which are produced by `xbuild` and as far as I can tell are impossible to generate on Windows.  (mono on windows will just use msbuild and produce `.pdb` files, which don't help)  

So: this plugin does a number of things:

* It allows you to specify a remote build server (running Linux) that will run `xbuild` so that you can generate `.mdb` files.  
    * Probably most convenient is to use Virtual Box with Ubuntu installed.  You can fit the VM out quite modestly as you're not going to tax it very hard.
    * Alternatively, you can use Windows Bash, which is a feature in beta that should be released to Windows 10 later this year.
* It will take the output from the build server and upload it to the host on which you want your program to run.  This may or may not be your build server — most likely not, as the main use for this plugin is when you want to debug programs on embedded devices.  In that scenario, you would specify the embedded device as your host.
* It provides a Visual Studio Debugger integration so that Visual Studio can communicate with the mono debugger. This means you can step-over/into/out, set breakpoints, view threads and callstacks, etc., just as you would always do when debugging a C# program.
* Finally, it creates a special C# project flavor that weaves all these tasks together in a seamless way.  You specify a handful of settings in a new *Mono* project property page, and can simply press the *Start* button as usual to begin debugging your program.  The custom project flavor intercepts the Visual Studio build command to delegate that work to the build serer.  And when pressing the start button, it will upload the program to your host, start it with the debug options enabled, and attach Visual Studio to the debugger.

