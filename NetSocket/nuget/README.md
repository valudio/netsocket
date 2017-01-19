# How to generete a nuget package for .NET Core projects

You have to follow these steps. You can find more accurate information [here](https://docs.microsoft.com/es-es/nuget/quickstart/create-and-publish-a-package):

1. Check if the `nuspec file` already exists. It has to be located inside the `nuget folder`. In case it doesn't then:

``` sh
nuget spec 
```

2. Open the `nuspec file` and edit what you needed. Don't forget to add the project `dependencies`. You can find this information by opening the `NetSocket.deps.json` file which can be found inside `bin\Release\netstandard1.5`. Take a look at `targets > ... > netsocket, Version=v*.*` and copy the `dependencies` node into your `nuspec` file. Another thing that you have to look at is that no `$*$` is present. You will have to replace this placeholders with actual information.
3. Now, we're going to create the `package` which is going to be published. But before that, we have to prepare nuget for this so we must put inside a `lib` folder all the content of the `bin\Release` folder.

4. After that, we create the package. Take a look at the console and be sure that you don't receive any message stating that there's something out of the `lib` folder. If that's the case, please review the process because you've certainly not put all the material to be released inside the `lib` folder.

``` sh
nuget pack <name_of_your_nuspec_file> 
```

5. Finally, before you publish it, it would be nice to check if all the dependencies are being taken into account. You can access the `.nugpk` file using `7zip` or `WinRar` and take a look at the generated `.nuspec`. If you can't see any `<dependencies>` node there, then you'll have to manually generate all the dependencies.